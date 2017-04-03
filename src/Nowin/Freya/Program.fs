module FreyaOnMono
open System
open System.Threading
open Freya.Core
open Freya.Machines.Http
open Freya.Routers.Uri.Template


let hello =
    freya {

        return Represent.text (sprintf "Hello %s!" "from freya on Katana!") }

let machine =
    freyaMachine {
        handleOk hello }

let router =
    freyaRouter {
        resource "/" machine }

type HelloWorld () =
    member __.Configuration () =
        OwinAppFunc.ofFreya (router)

open System
open Microsoft.Owin.Hosting

[<EntryPoint>]
let main _ =


    let port = 8083
    let url = sprintf "http://127.0.0.1:%d" port;
    let startOption = StartOptions()
    startOption.Port <- Nullable<_> (port)
    startOption.ServerFactory <- "Nowin"
    printfn "Listening on %s" url
    use app = WebApp.Start<HelloWorld>(startOption)
    
    let quitEvent = new ManualResetEvent(false)
    Console.CancelKeyPress.Add(fun (args) -> 
        quitEvent.Set() |> ignore
        args.Cancel <- true)
    quitEvent.WaitOne() |> ignore
    0
