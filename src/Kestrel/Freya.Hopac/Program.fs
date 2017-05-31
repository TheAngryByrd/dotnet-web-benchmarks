module Main

open Freya.Core
open Freya.Machines.Http
open Freya.Routers.Uri.Template
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open KestrelInterop


let hello =
    freya {
        return Represent.text ("Hello World!") }

let machine =
    freyaMachine {
        handleOk hello }

let router =
    freyaRouter {
        resource "/" machine }

let root = UriTemplateRouter.Freya router
[<EntryPoint>]
let main argv =
  let configureApp =
    ApplicationBuilder.useFreya root

  WebHost.create ()
  |> WebHost.bindTo [|"http://*:8083"|]
  |> WebHost.configure configureApp
  |> WebHost.buildAndRun

  0 // return an integer exit code
