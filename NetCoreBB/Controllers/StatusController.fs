namespace NetCoreBB.Controllers

open Microsoft.AspNetCore.Mvc
open System


[<ApiController>]
[<Route("[controller]")>]
type StatusController() =
    inherit ControllerBase()

    member __.Get(): string = "Test"
