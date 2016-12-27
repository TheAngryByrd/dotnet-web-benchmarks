namespace App 

module App =
    open Nancy
    type HelloWorldModule() as this =
        inherit NancyModule()
        do
            this.Get("/", fun _ -> "Hello From Nancy on Kestrel!" :> obj )

module Main =

    open Microsoft.AspNetCore.Builder
    open Microsoft.AspNetCore.Hosting
    open Microsoft.Extensions.Configuration
    open Nancy.Owin

    type Startup() =
        member this.Configure (app : IApplicationBuilder) : unit=
            app.UseOwin(fun o -> o.UseNancy() |> ignore) |> ignore

    [<EntryPoint>]
    let main argv =
        WebHostBuilder()
            .UseUrls("http://localhost:8083")
            .UseKestrel()
            .UseStartup<Startup>()
            .Build()
            .Run()


        0    // return an integer exit code
