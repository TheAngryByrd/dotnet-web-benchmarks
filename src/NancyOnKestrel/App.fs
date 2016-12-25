module App
open Nancy
type HelloWorldModule() as this =
    inherit NancyModule()
    do
        this.Get("/", fun _ -> "Hello From Nancy on Kestrel!" :> obj )