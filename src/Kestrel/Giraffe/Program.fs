module Main

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http

open Giraffe.HttpHandlers
open Giraffe.Middleware


let webApp = 
    choose [
        GET  >=> route "/" >=> text "Hello World!"

    ]
[<EntryPoint>]
let main argv =
    WebHostBuilder()
        .UseUrls("http://0.0.0.0:8083")
        .UseKestrel()
        .Configure(fun a -> a.UseGiraffe(webApp))
        .Build()
        .Run()


    0 // return an integer exit code
