namespace SuaveOnKestrel

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Suave
open Suave.Operators
open Suave.Successful
open Suave.AspNetCore

module App =
    let catchAll =
        fun (ctx : HttpContext) ->
            OK "Hello world from Suave on Kestrel" 
            <| ctx

    type Startup () =
        member __.Configure (app : IApplicationBuilder) =
            app.UseSuave(catchAll) |> ignore

    [<EntryPoint>]
    let main _ =
        WebHostBuilder()
            .UseUrls("http://localhost:8083")
            .UseKestrel()
            .UseStartup<Startup>()
            .Build()
            .Run()
        0