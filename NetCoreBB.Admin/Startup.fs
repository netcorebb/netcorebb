module NetCoreBB.Admin.Startup

//open Bolero.Remoting.Client
open Microsoft.AspNetCore.Components.WebAssembly.Hosting


[<EntryPoint>]
let main args =
    let wasmBuilder = WebAssemblyHostBuilder.CreateDefault(args)
    wasmBuilder.RootComponents.Add<Root.Component>("#app")
    //builder.Services.AddRemoting(builder.HostEnvironment) |> ignore
    wasmBuilder.Build().RunAsync() |> ignore

    let exitCodeOk = 0
    exitCodeOk
