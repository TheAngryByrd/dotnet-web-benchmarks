namespace SuaveOnKestrel

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Suave
open Suave.Http
open Suave.Operators
open Suave.Successful
open Suave.AspNetCore

module App =
    open Suave.Filters
    let suaveApp = GET >=> path "/" >=> OK "Hello World!"

    type Startup () =
        member __.Configure (app : IApplicationBuilder) =
            app.UseSuave(suaveApp) |> ignore

    [<EntryPoint>]
    let main _ =
        WebHostBuilder()
            .UseUrls("http://localhost:8083")
            .UseKestrel()
            .UseStartup<Startup>()
            .Build()
            .Run()
        0