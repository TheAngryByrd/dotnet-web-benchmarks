module Main

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http

open AspNetCore.Lambda.HttpHandlers
open AspNetCore.Lambda.Middleware


let webApp = 
    choose [
        GET  >=> route "/" >=> text "Hello from AspNetCore Lambda on Kestrel!"

    ]
[<EntryPoint>]
let main argv =
    WebHostBuilder()
        .UseUrls("http://0.0.0.0:8083")
        .UseKestrel()
        .Configure(fun a -> a.UseLambda(webApp))
        .Build()
        .Run()


    0 // return an integer exit code
