
open Suave                 // always open suave
open Suave.Successful      // for OK-result
open Suave.Web             // for config\
open Suave.Http

[<EntryPoint>]
let main argv =
    let config = {defaultConfig with 
                    bindings = [ HttpBinding.createSimple HTTP "127.0.0.1" 8083 ]}
    startWebServer config (OK "Hello World from Suave on Mono!")
    0
