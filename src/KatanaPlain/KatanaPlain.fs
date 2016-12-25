namespace App
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
        let url = "http://+:8083";
        printfn "Listening on %s" url
        use app = WebApp.Start<Startup>(url)
        System.Console.ReadLine() |> ignore
        0 // return an integer exit code
