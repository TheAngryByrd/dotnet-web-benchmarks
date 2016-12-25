namespace HelloMvc.Controllers

open Microsoft.AspNetCore.Mvc

type HomeController() =
    inherit Controller()
    member x.Index () =
        x.Content "Hello from MVC on Kestrel!"