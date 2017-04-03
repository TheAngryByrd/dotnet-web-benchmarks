namespace App
open System
open System.Threading
open Owin
open Microsoft.Owin.Hosting
module Main =
    type Startup () =
        member this.Configuration(app :IAppBuilder) =
            app.Run(
                fun context -> 
                    context.Response.ContentType <- "text/plain"
                    context.Response.WriteAsync("hello from katana plain!")
            )
            ()

    [<EntryPoint>]
    let main argv =
        let url = "http://127.0.0.1:8083";
        printfn "Listening on %s" url
        use app = WebApp.Start<Startup>(url)
        let quitEvent = new ManualResetEvent(false)
        Console.CancelKeyPress.Add(fun (args) -> 
            quitEvent.Set() |> ignore
            args.Cancel <- true)
        quitEvent.WaitOne() |> ignore
        0 // return an integer exit code
