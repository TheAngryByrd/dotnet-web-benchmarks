module Main

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.DependencyInjection


type Startup() =
    member this.ConfigureServices(services : IServiceCollection) : unit =
        services.AddMvc() |> ignore
    member this.Configure (app : IApplicationBuilder) : unit=
        app.UseMvc(fun routes ->
            routes.MapRoute (name = "default", template = "{controller=Home}/{action=Index}/") |> ignore) 
        |> ignore

[<EntryPoint>]
let main argv =
    WebHostBuilder()
        .UseUrls("http://localhost:8083")
        .UseKestrel()
        .UseStartup<Startup>()
        .Build()
        .Run()


    0 // return an integer exit code
