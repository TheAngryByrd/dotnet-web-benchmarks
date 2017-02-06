namespace FreyaOnNowin

open Freya.Core
open Freya.Machines.Http
open Freya.Routers.Uri.Template
open Owin
open Microsoft.Owin.Hosting
open System
open System.Threading

module App =
  let hello =
    freya {
      return Represent.text "Hello World from Freya"
      }

  let machine =
    freyaMachine {
      handleOk hello
      }

  let router =
    freyaRouter {
      resource "/" machine
      }

  type Startup () =
      member __.Configuration() =
        OwinAppFunc.ofFreya router

  [<EntryPoint>]
  let main _ =
        let startOption = StartOptions("http://127.0.0.1:8083", ServerFactory = "Nowin")
        printfn "Listening on %A" startOption.Urls
        use app = WebApp.Start<Startup>(startOption)
        let quitEvent = new ManualResetEvent(false)
        Console.CancelKeyPress.Add(fun (args) -> 
            quitEvent.Set() |> ignore
            args.Cancel <- true)
        quitEvent.WaitOne() |> ignore
        0 // return an integer exit code
