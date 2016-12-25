// include Fake libs
#r "./packages/FAKE/tools/FakeLib.dll"

#r "./packages/build/Newtonsoft.Json/lib/net45//Newtonsoft.Json.dll"
open Newtonsoft.Json

open Fake
open System
open System.IO
open System.Diagnostics
open System.Net.Sockets
// Directories

let srcDir = "./src"

//Helpers

let waitForExit ( proc : Process) = proc.WaitForExit()
let startProc fileName args workingDir=
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
    with e -> waitForPortInUse port
 
let kill procId =
    printfn "killing process id %d" procId
    startProc "kill" (procId |> sprintf "-9 %d") ""

let killProcessOnPort port =
    getProcessIdByPort port |> Option.iter(kill >> waitForExit)


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
    let args = sprintf "run --project %s"  project
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
    bytes : int64
    duration : int64
    requests : int64
    errors : Error
}


let projects =
    [
        "SuaveOnMono", msbuildAndRun
        "SuaveOnCoreCLR", dotnetBuildAndRun
    ]


let wrk threads connections duration script url=
    let args = sprintf "-t%d -c%d -d%d -s %s %s" threads connections duration script url
    let result = ExecProcessAndReturnMessages 
                    (fun psi ->
                        psi.FileName <- "wrk"
                        psi.Arguments <-args
                    ) (TimeSpan.FromMinutes(1.))
    JsonConvert.DeserializeObject<Summary>(result.Messages |> Seq.last)
    
let port = 8083

let runBenchmark (projectName, runner) =   
    use proc = runner projectName
    waitForPortInUse port
    let summary = wrk 8 400 30 "./scripts/reportStatsViaJson.lua" "http://localhost:8083/"
    killProcessOnPort port 
    (projectName, summary)

Target "Benchmark" (fun _ ->
    //Make sure nothing is on this port
    killProcessOnPort port
    projects 
    |> Seq.map (runBenchmark)
    |> Seq.iter (printfn "%A")

)

// Build order
"Clean"
  ==> "Benchmark"


// start build
RunTargetOrDefault "Benchmark"
