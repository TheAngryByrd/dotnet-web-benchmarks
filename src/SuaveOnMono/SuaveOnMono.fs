
open Suave                 // always open suave
open Suave.Successful      // for OK-result
open Suave.Web             // for config\
open Suave.Http

[<EntryPoint>]
let main argv =
    startWebServer defaultConfig (OK "Hello World from Suave on Mono!")
    0
