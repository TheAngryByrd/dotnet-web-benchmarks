// include Fake libs
#r "./packages/FAKE/tools/FakeLib.dll"
#load "./packages/build/FsLab/FsLab.fsx"

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
open System.Diagnostics
open System.Net.Sockets
// Directories
let srcDir = "./src"
let reportDir = "./reports"

//Helpers
let waitForExit ( proc : Process) = proc.WaitForExit()
let startProc fileName args workingDir=
    printfn "Starting %s %s" fileName args
    let proc = 
        ProcessStartInfo(FileName = fileName, Arguments = args, WorkingDirectory = workingDir, UseShellExecute = false) 
        |> Process.Start

    proc 

let getProcessIdByPort port =
    let result =
        ExecProcessAndReturnMessages 
                        (fun psi ->
                            psi.FileName <- "lsof"
                            psi.Arguments <-(sprintf "-ti tcp:%d" port)
                        ) (TimeSpan.FromMinutes(1.))
    result.Messages |> Seq.tryHead |> Option.map int

let rec waitForPortInUse port =
    use client = new TcpClient()
    try
        client.Connect("127.0.0.1",port)

        while client.Connected |> not do
            client.Connect("127.0.0.1",port)
    with e -> 
        client.Close()
        waitForPortInUse port
 
let kill procId =
    printfn "killing process id %d" procId
    startProc "kill" (procId |> sprintf "-9 %d") ""

let killProcessOnPort port =
    getProcessIdByPort port |> Option.iter(kill >> waitForExit)



let stringJoin (separator : string) (strings : string seq) =
    String.Join(separator, strings)

// Filesets
let appReferences  =
    !! "/**/*.csproj"
    ++ "/**/*.fsproj"
    ++ "/**/project.json"

let getProjFile proj  =
    !! (sprintf "src/%s/*.csproj" proj)
    ++ (sprintf "src/%s/*.fsproj" proj)
    ++ (sprintf "src/%s/project.json" proj)
    |> Seq.head


let getExe proj  =
    !! (sprintf "src/**/%s.exe" proj)
    |> Seq.head

// version info
let version = "0.1"  // or retrieve from CI server

// Targets
Target "Clean" (fun _ ->
    (appReferences
    |> Seq.map(fun f -> (System.IO.Path.GetDirectoryName f) @@ "bin")
    |> Seq.toList)
    @
    (appReferences
    |> Seq.map(fun f -> (System.IO.Path.GetDirectoryName f) @@ "obj")
    |> Seq.toList)
    |> CleanDirs
)

let msbuild projFile =
    MSBuildDebug "" "Build" [projFile]
    |> ignore


let msbuildAndRun projName =
    projName |> getProjFile |> msbuild 
    startProc (getExe projName) "" ""
    

let dotnetRestore projFile =    
    DotNetCli.Restore (fun c ->
        { c with
            Project = projFile
        })
let dotnetBuild projFile =
    dotnetRestore projFile
    DotNetCli.Build (fun c ->
        { c with
            Project = projFile
        })


let dotnetrun project =
    let args = sprintf "run --configuration Release --project %s"  project
    startProc "dotnet" args ""

let dotnetBuildAndRun projName =
    projName |> getProjFile |> dotnetBuild
    projName |> getProjFile |> dotnetrun



// --------------------------------------------------------------------------------------

//https://github.com/wg/wrk/blob/50305ed1d89408c26067a970dcd5d9dbea19de9d/SCRIPTING
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
    duration : int // in microseconds
    requests : int
    errors : Error
}


let projects =
    [
        "KatanaPlain", msbuildAndRun
        "NowinOnMono", msbuildAndRun
        "NancyOnKatana", msbuildAndRun
        "SuaveOnMono", msbuildAndRun
        "KestrelPlain", dotnetBuildAndRun
        "SuaveOnCoreCLR", dotnetBuildAndRun
        "MvcOnKestrel", dotnetBuildAndRun
    ]
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
                    ) (TimeSpan.FromMinutes(1.))
    JsonConvert.DeserializeObject<Summary>(result.Messages |> Seq.last)
    
let port = 8083

let createPage body =
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
            </head>
            <body>
                <div>%s</div>
            </body>
        </html>
    """ body

let runBenchmark (projectName, runner) =   
    Async.Sleep(5000) |> Async.RunSynchronously
    use proc = runner projectName
    waitForPortInUse port
    let summary = wrk 8 400 10 "./scripts/reportStatsViaJson.lua" "http://localhost:8083/"
    //Have to kill process by port because dotnet run calls dotnet exec which has a different process id
    killProcessOnPort port 
    (projectName, summary)

let mutable results = null

Target "Benchmark" (fun _ ->
    //Make sure nothing is on this port
    killProcessOnPort port
    results <-
        projects 
        |> Seq.map (runBenchmark)
        |> Seq.toList
        |> Seq.cache
)

let tee f x = f x; x

let createReport (results : seq<string * Summary>) =    
    results |> Seq.iter (printfn "%A")
    let firstResult = results |> Seq.head |> snd
    let duration = (firstResult.duration / 1000000)
    let reportPath = reportDir @@ "report.html"

    let totalRequests =
        results
        |> Seq.map(fun (proj,summary) -> [(proj,summary.requests)])
        |> Chart.Column
        |> Chart.WithLabels (results |> Seq.map(fst))
        |> Chart.WithTitle (sprintf "Total Requests over %d seconds" duration)
    
    let requestsPerSecond =
        results
        |> Seq.map(fun (proj,summary) -> [(proj, summary.requests/(summary.duration / 1000000))])
        |> Chart.Column
        |> Chart.WithLabels (results |> Seq.map(fst))
        |> Chart.WithTitle (sprintf "Requests per second over %d seconds" duration)
    
    
    [totalRequests;requestsPerSecond]
    |> Seq.map(tee Chart.Show)
    |> Seq.map getHtml
    |> stringJoin ""
    |> createPage
    |> writeToFile reportPath



    

Target "GenerateReport" (fun _ ->
    createReport results
)

// Build order
"Benchmark"
  ==> "GenerateReport"


// start build
RunTargetOrDefault "GenerateReport"
