module WebApiOnNowin
open System
open System.Threading
open Owin
open Microsoft.Owin.Hosting
open System.Web.Http

type HomeController() =
    inherit ApiController()
    member this.Get() =
        "Hello from WebApi on Nowin!"
                
type HttpRouteDefaults = { Controller : string; Id : obj }
type Startup () =
    member this.Configuration(app :IAppBuilder) =
        let config = new HttpConfiguration();
        config.Routes.MapHttpRoute(
            "DefaultAPI",
            "{controller}/{id}",
            { Controller = "Home"; Id = RouteParameter.Optional }) |> ignore

        app.UseWebApi(config) |> ignore
        ()
[<EntryPoint>]
let main argv =
    let startOptions = StartOptions("http://127.0.0.1:8083", ServerFactory = "Nowin")
    printfn "Listening on %A" startOptions.Urls
    use app = WebApp.Start<Startup> startOptions
    let quitEvent = new ManualResetEvent(false)
    Console.CancelKeyPress.Add(fun (args) -> 
        quitEvent.Set() |> ignore
        args.Cancel <- true)
    quitEvent.WaitOne() |> ignore
    0 // return an integer exit code
