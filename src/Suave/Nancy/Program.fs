namespace App 
open System
open System.Threading
open Owin
open Microsoft.Owin.Hosting


module App =
    open Nancy
    type HelloWorldModule() as this =
        inherit NancyModule()
        do
            this.Get("/", fun _ -> "Hello World!" :> obj )


module Main = 

    type Startup () =
        member this.Configuration(app :IAppBuilder) =
            app.UseNancy() |> ignore
            ()

    [<EntryPoint>]
    let main argv =
        let url = "http://127.0.0.1:8083";
        let options = StartOptions()
        options.ServerFactory <- "Suave.Owin+OwinServerFactory"
        options.Urls.Add(url)
        printfn "Listening on %s" url
        use app = WebApp.Start<Startup>(options)
        let quitEvent = new ManualResetEvent(false)
        Console.CancelKeyPress.Add(fun (args) -> 
            quitEvent.Set() |> ignore
            args.Cancel <- true)
        quitEvent.WaitOne() |> ignore
        0 // return an integer exit code
