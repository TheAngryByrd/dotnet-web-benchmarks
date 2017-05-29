module Main

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http

open Giraffe.HttpHandlers
open Giraffe.Middleware
open Giraffe.AsyncTask

let handleAsync (ctx) = task {
    return! text "Hello World!" ctx
}
let webApp = 
    choose [
        GET  >=> route "/" >=> handleAsync
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
