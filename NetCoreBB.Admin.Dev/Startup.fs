module NetCoreBB.Admin.Dev.Startup

open Microsoft.AspNetCore
//open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.DependencyInjection
//open Bolero.Remoting.Server
open Bolero.Server.RazorHost
open Bolero.Templating.Server


type Startup() =
    member _.ConfigureServices(services: IServiceCollection) =
        services.AddMvc().AddRazorRuntimeCompilation() |> ignore
        services.AddServerSideBlazor() |> ignore
        services
            //.AddAuthorization()
            //.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //    .AddCookie()
            //   .Services
            //.AddRemoting<BookService>()
            .AddBoleroHost()
#if DEBUG
            .AddHotReload(templateDir = __SOURCE_DIRECTORY__ + "/../NetCoreBB.Admin")
#endif
        |> ignore


    member _.Configure(app: IApplicationBuilder, env: IWebHostEnvironment) =
        app
            //.UseAuthentication()
            //.UseRemoting()
            .UseStaticFiles()
            .UseRouting()
            .UseBlazorFrameworkFiles()
            .UseEndpoints(fun endpoints ->
#if DEBUG
                endpoints.UseHotReload()
#endif
                endpoints.MapBlazorHub() |> ignore
                endpoints.MapFallbackToPage("/Index") |> ignore)
        |> ignore


[<EntryPoint>]
let main args =
    WebHost.CreateDefaultBuilder(args)
        .UseStaticWebAssets()
        .UseStartup<Startup>()
        .Build()
        .Run()
    let exitCodeOk = 0
    exitCodeOk
