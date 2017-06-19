module Main

open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http

open Giraffe.HttpHandlers
open Giraffe.Middleware
open Giraffe.AsyncTask
open Test.HttpRouter
// let handleAsync (ctx) = task {
//     return! text "Hello World!" ctx
// }
// let webApp = 
//     choose [
//         GET  >=> route "/" >=> handleAsync
//     ]
let trieApi : HttpHandler =
    routeTrie [
        routeT "/" ==> text "Hello world, from Giraffe!"
        routeT "/test" ==> text "Giraffe test working"
        routeT "/about" ==> text "Giraffe about page!"
        routeT "/wheretofindus" ==> text "our location page"
        routeT "/ourstory" ==> text "our story page"
        routeT "/products" ==> text "product page"
        routeT "/delivery" ==> text "delivery page"
        routeTf "/data/%s/weather" (fun v -> sprintf "json (weatherForecasts (%s))" v |> text)
        routeTf "/value/%s" text 
        subRouteT "/auth" ==> routeTrie [
            routeT "/dashboard" ==> text "Auth Dashboard"
            routeT "/inbox" ==> text "Auth Inbox"
            routeT "/helpdesk" ==> text "Auth Helpdesk"
            routeTf "/parse%slong%istrings%sand%sIntegers" (fun (a,b,c,d) -> sprintf "%s | %i | %s | %s" a b c d |> text)
            routeTf "token/%s" (fun v -> (text ("following token recieved:" + v)))                                    
            subRouteT "/manager" ==> routeTrie [
                routeT "/payroll" ==> text "Manager Payroll"
                routeT "/timesheets" ==> text "Manager Timesheets"
                routeT "/teamview" ==> text "Manager Teamview"
                routeTf "/team%ssales%f" (fun (t,s) -> sprintf "team %s had sales of %f" t s |> text)
                routeTf "/accesscode/%i" (fun i -> sprintf "manager access close is %i" i |> text)
                subRouteT "/executive" ==> routeTrie [
                    routeT "/finance" ==> text "executive finance"
                    routeT "/operations" ==> text "executive operations"
                    routeT "/mis" ==> text "executive mis"
                    routeTf "/area/%s" (sprintf "executive area %s" >> text)
                    routeTf "/area/%s/district/%s/costcode%i" (fun (a,d,c) -> sprintf "executive area %s district %s costcode %s"  a d c |> text)
                 ]
            ]
        ]
    ]

[<EntryPoint>]
let main argv =
    WebHostBuilder()
        .UseUrls("http://0.0.0.0:8083")
        .UseKestrel()
        .Configure(fun a -> a.UseGiraffe(trieApi))
        .Build()
        .Run()


    0 // return an integer exit code
