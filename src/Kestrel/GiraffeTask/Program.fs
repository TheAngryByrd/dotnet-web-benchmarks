module Main

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http

open Giraffe.HttpHandlers
open Giraffe.Middleware
open Giraffe.AsyncTask

// let handleAsync (ctx) = task {
//     return! text "Hello World!" ctx
// }
// let webApp = 
//     choose [
//         GET  >=> route "/" >=> handleAsync
//     ]
let chooseApi : HttpHandler =
    choose [
        route "/" >=> text "Hello world, from Giraffe!"
        route "/test" >=> text "Giraffe test working"
        route "/about" >=> text "Giraffe about page!"
        route "/wheretofindus" >=> text "our location page"
        route "/ourstory" >=> text "our story page"
        route "/products" >=> text "product page"
        route "/delivery" >=> text "delivery page"
        routef "/data/%s/weather" (fun v -> sprintf "json (weatherForecasts (%s))" v |> text)
        routef "/value/%s" text 
        subRoute "/auth" >=> choose [
            route "/dashboard" >=> text "Auth Dashboard"
            route "/inbox" >=> text "Auth Inbox"
            route "/helpdesk" >=> text "Auth Helpdesk"
            routef "/parse%slong%istrings%sand%sIntegers" (fun (a,b,c,d) -> sprintf "%s | %i | %s | %s" a b c d |> text)
            routef "token/%s" (fun v -> text ("following token recieved:" + v))                                    
            subRoute "/manager" >=> choose [
                route "/payroll" >=> text "Manager Payroll"
                route "/timesheets" >=> text "Manager Timesheets"
                route "/teamview" >=> text "Manager Teamview"
                routef "/team%ssales%f" (fun (t,s) -> sprintf "team %s had sales of %f" t s |> text)
                routef "/accesscode/%i" (fun i -> sprintf "manager access close is %i" i |> text)
                subRoute "/executive" >=> choose [
                    route "/finance" >=> text "executive finance"
                    route "/operations" >=> text "executive operations"
                    route "/mis" >=> text "executive mis"
                    routef "/area/%s" (sprintf "executive area %s" >> text)
                    routef "/area/%s/district/%s/costcode%i" (fun (a,d,c) -> sprintf "executive area %s district %s costcode %s"  a d c |> text)
                 ]
            ]
        ]
    ]

[<EntryPoint>]
let main argv =
    WebHostBuilder()
        .UseUrls("http://0.0.0.0:8083")
        .UseKestrel()
        .Configure(fun a -> a.UseGiraffe(chooseApi))
        .Build()
        .Run()


    0 // return an integer exit code
