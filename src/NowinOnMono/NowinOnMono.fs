namespace App
open System
open System.Threading
open System
open Owin
open Microsoft.Owin.Hosting
module Main =
    type Startup () =
        member this.Configuration(app :IAppBuilder) =
            app.Run(
                fun context -> 
                    context.Response.ContentType <- "text/plain"
                    context.Response.WriteAsync("hello from nowin plain!")
            )
            ()

    [<EntryPoint>]
    let main argv =
        let port = 8083
        let url = sprintf "http://127.0.0.1:%d" port;
        let startOption = StartOptions()
        startOption.Port <- Nullable<_> (port)
        startOption.ServerFactory <- "Nowin"
        printfn "Listening on %s" url
        use app = WebApp.Start<Startup>(startOption)
        let quitEvent = new ManualResetEvent(false)
        Console.CancelKeyPress.Add(fun (args) -> 
            quitEvent.Set() |> ignore
            args.Cancel <- true)
        quitEvent.WaitOne() |> ignore
        0 // return an integer exit code
