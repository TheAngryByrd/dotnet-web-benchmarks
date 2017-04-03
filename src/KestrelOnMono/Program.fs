module Main

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http


[<EntryPoint>]
let main argv =
    WebHostBuilder()
        .UseUrls("http://0.0.0.0:8083")
        .UseKestrel()
        .Configure(fun a -> a.Run(fun c -> c.Response.WriteAsync("Hello from Plain Kestrel on mono!")))
        .Build()
        .Run()


    0 // return an integer exit code
