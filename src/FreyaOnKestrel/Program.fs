namespace Quatro

open Freya.Core
open Freya.Machines.Http
open Freya.Routers.Uri.Template
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting

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
    member __.Configure (app : IApplicationBuilder) =
      let freyaOwin = OwinMidFunc.ofFreya (UriTemplateRouter.Freya router)
      app.UseOwin (fun p -> p.Invoke freyaOwin) |> ignore

  [<EntryPoint>]
  let main _ =
    WebHostBuilder()
        .UseUrls("http://localhost:8083")
        .UseKestrel()
        .UseStartup<Startup>()
        .Build()
        .Run()
    0