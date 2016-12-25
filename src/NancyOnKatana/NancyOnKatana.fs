namespace App 
open Owin
open Microsoft.Owin.Hosting
module NancyOnMono =

    open Nancy
    type HelloWorldModule() as this =
        inherit NancyModule()
        do
            this.Get.["/"] <- fun _ -> "Hello From Nancy on Mono!" :> obj 


module Main = 

    type Startup () =
        member this.Configuration(app :IAppBuilder) =
            app.UseNancy() |> ignore
            ()

    [<EntryPoint>]
    let main argv =
        let url = "http://+:8083";
        printfn "Listening on %s" url
        use app = WebApp.Start<Startup>(url)
        System.Console.ReadLine() |> ignore
        0 // return an integer exit code
