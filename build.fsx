// include Fake libs
#r "./packages/build/FAKE/tools/FakeLib.dll"
#load "./packages/build/FsLab/FsLab.fsx"
#r "./packages/build/MarkdownLog/lib/portable-net45+win+wp8+wpa81/MarkdownLog.dll"
open Deedle
open FSharp.Data
open XPlot.GoogleCharts
open XPlot.GoogleCharts.Deedle

#r "./packages/build/Newtonsoft.Json/lib/net45//Newtonsoft.Json.dll"
open Newtonsoft.Json

open Fake
open Fake.EnvironmentHelper
open System
open System.IO
open System.Text
open System.Diagnostics
open System.Net.Sockets


#if MONO
let inferFrameworkPathOverride () =
    let mscorlib = "mscorlib.dll"
    let possibleFrameworkPaths =
        [ 
            "/Library/Frameworks/Mono.framework/Versions/Current/lib/mono/4.5/"
            "/usr/local/Cellar/mono/4.6.12/lib/mono/4.5/"
            "/usr/lib/mono/4.5/"
        ]

    possibleFrameworkPaths
    |> Seq.find (fun p ->IO.File.Exists(p @@ mscorlib))

Environment.SetEnvironmentVariable("FrameworkPathOverride", inferFrameworkPathOverride ())
#endif

// Directories
let srcDir = "./src"
let srcGlob = "src/**/**/*.fsproj"

let reportDir = "./reports"

let tee f x = f x; x

module Seq =
    let tee (f : 'a -> unit) (s : seq<'a>) =
        s |> Seq.map(tee f)

let stringJoin (separator : string) (strings : string seq) =
    String.Join(separator, strings)

[<Measure>] type ms
[<Measure>] type s

let convertMStoS (ms : int<ms>) =
    ms / 1000000<ms/s>

[<Measure>] type req


//Proc helpers
let waitForExit ( proc : Process) = proc.WaitForExit()
let startProc fileName args workingDir=
    let proc = 
        ProcessStartInfo(FileName = fileName, Arguments = args, WorkingDirectory = workingDir, UseShellExecute = true) 
        |> Process.Start

    proc 

let startProc' fileName (args : #seq<string>) workingDir =
    startProc fileName (String.Join(" ",args)) workingDir

let execProcAndReturnMessages filename args =
    ExecProcessAndReturnMessages 
                    (fun psi ->
                        psi.FileName <- filename
                        psi.Arguments <-args
                    ) (TimeSpan.FromMinutes(1.))

let getProcessMessages (procResult : ProcessResult) =
    procResult.Messages

//System Helpers
let lsof args = execProcAndReturnMessages "lsof" args
 
let kill procId = 
    execProcAndReturnMessages "kill" (procId |> sprintf "-9 %d") |> ignore

let mono args = execProcAndReturnMessages "mono" args

let dotnet args = execProcAndReturnMessages "dotnet" args


let getProcessor () =
    let result =
        if EnvironmentHelper.isMacOS
        then execProcAndReturnMessages "sysctl" "-n machdep.cpu.brand_string" |> getProcessMessages 
        elif EnvironmentHelper.isLinux 
        then execProcAndReturnMessages "cat" "/proc/cpuinfo | grep 'model name' | uniq" |> getProcessMessages 
        else failwith "Not supported OS"
    result |> Seq.head

let getProcessIdByPort port =
    let result = lsof (sprintf "-ti tcp:%d" port)
    result.Messages |> Seq.tryHead |> Option.map int

let waitForPortInUse  port =
    let mutable portInUse = false
    let sw = System.Diagnostics.Stopwatch.StartNew()
    while not portInUse do  
        if sw.ElapsedMilliseconds > 20000L then failwith "waited enough, must have failed"
        match getProcessIdByPort port with
        | Some _ -> portInUse <- true
        | _ -> ()
        Async.Sleep(1000) |> Async.RunSynchronously
        


let killProcessOnPort port =
    for i in 1 .. 10 do
        getProcessIdByPort port |> Option.iter kill 



// Targets
Target "Clean" (fun _ ->
    !! srcGlob
    |> Seq.collect(fun p -> 
        ["bin";"obj"] 
        |> Seq.map(fun sp ->
             IO.Path.GetDirectoryName p @@ sp)
        )
    |> CleanDirs
)


let dotnetRestore projFile =    
    DotNetCli.Restore (fun c ->
        { c with
            Project = projFile
        })

// --------------------------------------------------------------------------------------

type SystemInfo = {
    CPUModel : string
    ProcessorCount : int
    MonoVersion : string seq
    DotnetVersion : string seq
    OperatingSystem : string
}

let getSystemInfo () =
    let machineInfo = EnvironmentHelper.getMachineEnvironment ()
    let monoVersion = mono "--version" |> getProcessMessages 
    let dotnetVersion = dotnet "--version" |> getProcessMessages 
    {
        CPUModel =  getProcessor ()
        ProcessorCount = machineInfo.ProcessorCount
        MonoVersion = monoVersion
        DotnetVersion = dotnetVersion
        OperatingSystem = machineInfo.OperatingSystem
    }

let td inner = 
    sprintf "<td>%s</td>" inner
let tr tds=
    tds 
    |> stringJoin ""
    |> sprintf "<tr>%s</tr>"
let table trs =
    trs 
    |> stringJoin ""
    |> sprintf "<table>%s</table>"

let systemInfoToHtmlTable (sysInfo:SystemInfo) =
    let cpu = sysInfo.CPUModel |> td
    let os = sysInfo.OperatingSystem |> td
    let proc = sysInfo.ProcessorCount |> string |> td
    let monoV = sysInfo.MonoVersion |> Seq.head|> td
    let dotnetV = sysInfo.DotnetVersion|> Seq.head |> td

    table
        [
            tr [td "CPU"; cpu]
            tr [td "Operating System" ;os;]
            tr [td "Processor Count" ;proc;]
            tr [td "Mono Version" ;monoV;]
            tr [td "Dotnet Version" ;dotnetV;]
        ]

type ProjectInfo = {
    ProjectFile : string
    TargetFramework : string
    IsMono : bool
    WebServer : string
    WebFramework : string
}
    with member x.FriendlyName = sprintf "%s/%s on %s" x.WebServer x.WebFramework x.TargetFramework



//https://github.com/wg/wrk/blob/master/SCRIPTING
//{"bytes":217834904,"duration":30099407,"errors":{"connect":0,"read":182,"status":0,"timeout":0,"write":0},"requests":1785532}
type Error = {
    connect : int
    read : int
    status : int
    timeout : int
    write : int
}

type Summary = {
    bytes : int
    duration : int<ms> // in microseconds
    requests : int<req>
    errors : Error
}
    with 
        member this.RequestsPerSecond () =         
            this.requests/( convertMStoS this.duration )


type Latency = {
    min : float
    max : float
    mean : float
    stdev : float
}
type ProjectName = string

let writeToFile filePath str =
    System.IO.File.WriteAllText(filePath, str)
let getHtml (chart : GoogleChart) =
    chart.GetInlineHtml()

let wrk threads connections duration script url=
    let args = sprintf "-t%d -c%d -d%d -s %s %s" threads connections duration script url
    let result = ExecProcessAndReturnMessages 
                    (fun psi ->
                        psi.FileName <- "wrk"
                        psi.Arguments <-args
                    ) (TimeSpan.FromMinutes(5.))
    if result.OK
    then
        let revResults = 
            result.Messages
            |> Seq.rev
            |> Seq.cache
        (
            JsonConvert.DeserializeObject<Summary>(revResults |> Seq.head), 
            (JsonConvert.DeserializeObject<Latency>(revResults |> Seq.skip 1 |> Seq.head))
        )
    else result.Errors |> stringJoin "" |> failwith 
    
let port = 8083

let createPage systemInfo charts =
    sprintf
        """
        <!DOCTYPE html>
        <html>
            <head>
                <meta charset="UTF-8">
                <meta http-equiv="X-UA-Compatible" content="IE=edge" />
                <title>Google Chart</title>
                <script type="text/javascript" src="https://www.google.com/jsapi"></script>
                <script type="text/javascript">
                    google.load('visualization', '1', { 'packages': ['corechart'] });
                </script>
                <style>
                    table {
                        font-family: arial, sans-serif;
                        border-collapse: collapse;
                    }
                    td, th {
                        border: 1px solid #dddddd;
                        text-align: left;
                        padding: 8px;
                    }

                    tr:nth-child(even) {
                        background-color: #dddddd;
                    }
                </style>

            </head>
            <body>
                <h3> SystemInfo </h3>
                    %s
                <br>
                <h3> Results </h3>
                <div>%s</div>
            </body>
        </html>
    """ systemInfo charts



let IsRunningOnMono tf =
    match tf with
    | "net45" | "net451" | "net452" 
    | "net46" | "net461" | "net462" -> isMono
    | _ -> false

let selectRunner (projectInfo : ProjectInfo) =
    let fi = fileInfo projectInfo.ProjectFile
    fi.Directory.ToString()
    |>
    match projectInfo.TargetFramework with
    | "net45" | "net451" | "net452" 
    | "net46" | "net461" | "net462" -> 
        if isMono then
            [
                "mono"
                sprintf "-f %s" projectInfo.TargetFramework
                "-mo=\"--arch=64\""
                "--restore"
                "-c Release"
                sprintf "-p %s" projectInfo.ProjectFile
            ]
            |> startProc' "dotnet"
        else 
            failwithf "lol who uses windows"
        
    | "netcoreapp1.0" | "netcoreapp1.1" -> 
        [
            "run"
            "-c Release"
            sprintf "-f %s" projectInfo.TargetFramework
            sprintf "-p %s" projectInfo.ProjectFile
        ]
        |> startProc' "dotnet"
    | _ -> failwithf "Unknown targetframework %s" projectInfo.TargetFramework


let runBenchmark (projectInfo : ProjectInfo) =   
    try
        
        logfn "---------------> Starting %s <---------------" projectInfo.FriendlyName
        killProcessOnPort port 
        use proc = selectRunner projectInfo
        waitForPortInUse port
        let (summary, latency) = wrk 8 400 30 "./scripts/reportStatsViaJson.lua" "http://127.0.0.1:8083/"
        proc.Kill()
        //Have to kill process by port because dotnet run calls dotnet exec which has a different process id
        killProcessOnPort port 


        logfn "---------------> Finished %s <---------------" projectInfo.FriendlyName
        Some (projectInfo, summary, latency)
    with e -> 
        eprintfn "%A" e
        None

let mutable (results : seq<ProjectInfo * Summary * Latency>) = null



let gatherProjectInfo (projFile : string) =
    let doc = Xml.XmlDocument()
    doc.Load(projFile)
    let targetFrameworks = doc.GetElementsByTagName("TargetFrameworks").[0].InnerText.Split(';')
    let parts = 
        projFile.Split(IO.Path.DirectorySeparatorChar)
        |> Array.rev

    let webframework = parts.[1]
    let webserver = parts.[2]

    targetFrameworks
    |> Seq.map(fun tf -> 
    {
        ProjectFile = projFile
        TargetFramework = tf
        WebServer = webserver
        WebFramework = webframework
        IsMono = IsRunningOnMono tf
    })
 

Target "Benchmark" (fun _ ->
    //Make sure nothing is on this port
    killProcessOnPort port
    results <-
        !! srcGlob
        |> Seq.tee dotnetRestore
        |> Seq.toList
        |> Seq.cache
        |> Seq.collect gatherProjectInfo
        |> Seq.choose runBenchmark
        |> Seq.toList
        |> Seq.cache
)




let dumpAllDataToConsole (results : seq<ProjectInfo * Summary * Latency>)  =  
    results |> Seq.iter (printfn "%A")



type TextReport = {
    WebServer : ProjectName
    WebFramework: string
    IsMono : bool
    TargetFramework : string
    TotalRequests : int<req>
    Duration: int<s>
    RequestsPerSecond : int<req/s>
}


let createTextReport (results : seq<ProjectInfo * Summary * Latency>) =
    results 
    |> Seq.map(fun (proj, sum, lat) -> 
    {
        WebServer = proj.WebServer
        WebFramework = proj.WebFramework
        IsMono = proj.IsMono
        TargetFramework = proj.TargetFramework
        TotalRequests = sum.requests
        Duration = sum.duration |> convertMStoS
        RequestsPerSecond = sum.RequestsPerSecond()
    })
    |> Seq.sortByDescending(fun tr -> tr.RequestsPerSecond)
    |> MarkdownLog.MarkDownBuilderExtensions.ToMarkdownTable
    |> string
    |> printfn "%s"


let createHtmlReport (results : seq<ProjectInfo * Summary * Latency>) =  
    let ( _,firstResult,_) = results |> Seq.head 
    let duration = (firstResult.duration |> convertMStoS)
    let reportPath = reportDir @@ (sprintf "report-%s.html" (DateTimeOffset.UtcNow.ToString("o")))
    let labels = results |> Seq.map(fun (proj,_,_) -> proj.FriendlyName)



    let totalRequestsChart =
        results
        |> Seq.map(fun (_,summary,latency) -> [("Total",summary.requests)])
        |> Chart.Bar
        |> Chart.WithLabels (labels)
        |> Chart.WithTitle (sprintf "Total Requests over %d seconds" duration)
    
    let requestsPerSecondChart =
        results
        |> Seq.map(fun (_,summary,_) -> [("Req/s", summary.RequestsPerSecond())])
        |> Chart.Bar
        |> Chart.WithLabels (labels)
        |> Chart.WithTitle (sprintf "Requests per second over %d seconds" duration)

        
    let meanLatencyChart =
        results
        |> Seq.map(fun (_,_,latency) -> 
            [
                // ("Min", latency.min/1000.); 
                // ("Max", latency.max/1000.); 
                ("Mean", latency.mean/1000.); 
            ])
        |> Chart.Bar
        |> Chart.WithLabels (labels)
        |> Chart.WithTitle (sprintf "Mean latency in milliseconds over %d seconds" duration)
    
    
    [totalRequestsChart;requestsPerSecondChart;meanLatencyChart]
    |> Seq.map getHtml
    |> stringJoin ""
    |> createPage (systemInfoToHtmlTable(getSystemInfo ()))
    |> writeToFile reportPath
    |> fun _ ->   System.Diagnostics.Process.Start((Path.GetFullPath(reportPath)))  
    |> ignore

let invoke f = f()


Target "GenerateReport" (fun _ ->
    [ 
        dumpAllDataToConsole
        createTextReport
        createHtmlReport
    ]
    |> Seq.map(fun f -> fun () -> f results)
    |> Seq.iter invoke
)


// Build order
"Clean"
  ==> "Benchmark"
  ==> "GenerateReport"


// start build
RunTargetOrDefault "GenerateReport"
