namespace App
open System
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
        let port = 8083
        let url = sprintf "http://+:%d" port;
        let startOption = StartOptions()
        startOption.Port <- Nullable<_> (port)
        startOption.ServerFactory <- "Nowin"
        printfn "Listening on %s" url
        use app = WebApp.Start<Startup>(startOption)
        System.Console.ReadLine() |> ignore
        0 // return an integer exit code
