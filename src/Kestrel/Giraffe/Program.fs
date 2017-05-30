module Main

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http

open Giraffe.HttpHandlers
open Giraffe.Middleware


let handleAsync (ctx) = async {
    return! text "Hello World!" ctx
}

let webApp = 
    choose [
        GET  >=> route "/" >=> handleAsync
    ]


// let webApp = 
//     choose [
//         GET >=>
//             choose [
//                 route "/" >=> text "Hello world, from Giraffe!"
//                 route "/test" >=> text "Giraffe test working"
//                 subRoute "/auth" ( 
//                     choose [
//                         route "/dashboard" >=> text "Auth Dashboard"
//                         route "/inbox" >=> text "Auth Inbox"
//                         subRoute "/manager" (
//                             route "/payroll" >=> text "Manager Payroll"
//                             route "/timesheets" >=> text "Manager Timesheets"
//                         )
//                     ]
//                 )
//                 route "/data" >=> text "json (weatherForecasts ())"
//                 routef "/value/%s" text  
//             ]
//     ]
[<EntryPoint>]
let main argv =
    WebHostBuilder()
        .UseUrls("http://0.0.0.0:8083")
        .UseKestrel()
        .Configure(fun a -> a.UseGiraffe(webApp))
        .Build()
        .Run()


    0 // return an integer exit code
