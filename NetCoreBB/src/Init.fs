(*
 * This file is part of the NetCoreBB forum software package.
 * License: GNU General Public License, version 3 (GNU GPLv3)
 * Copyright © 2019–2020 Roman Volkov
 *)

namespace NetCoreBB

open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging
open Serilog
open System.Runtime.InteropServices


module Init =
    let exitCodeOk = 0
    let exitCodeErr = 1


    [<EntryPoint>]
    let main args =
        try
            try
                let builder = WebHost.CreateDefaultBuilder(args)

                // [1] Application configuration
                let builder =
                    builder.ConfigureAppConfiguration(fun hostCtx config ->
                        hostCtx.Configuration.GetSection("AllowedHosts").Bind("*")

                        //let hostEnv = hostCtx.HostingEnvironment.EnvironmentName
                        //cfg.AddIniFile("Config/database.toml", true, true) |> ignore
                        0 |> ignore)


                // [2] Application logging
                let builder =
                    builder.UseSerilog().ConfigureLogging(fun hostCtx logBuilder ->
                           logBuilder.ClearProviders() |> ignore

                           //hostCtx.Configuration.GetSection("Serilog:MinimumLevel:Override:Microsoft.AspNetCore").Bind("Warning")

                           let hostEnv = hostCtx.HostingEnvironment.EnvironmentName

                           let logConfig = LoggerConfiguration().MinimumLevel.Information().Enrich.FromLogContext()
                           //logConfig = logConfig.ReadFrom.Configuration(hostingContext.Configuration);

                           (* let logConfig = logConfig.WriteTo.File((sprintf "Logs/netcorebb-%s-.log" hostEnv Char.ToLower),
                        (rollingInterval: RollingInterval.Day),
                        (rollOnFileSizeLimit: true),
                        (fileSizeLimitBytes: 1024 * 1024 * 100), // 100 MiB
                        (retainedFileCountLimit: 365), // 1 year
                        (buffered: true),
                        (flushToDiskInterval: span),
                        (outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                        )*)

                           let logConfig = logConfig.WriteTo.ColoredConsole()

                           Log.Logger <- logConfig.CreateLogger()

                           let msg =
                               [| "+++++ Starting NetCoreBB +++++"
                                  sprintf "> Environment: %s" hostEnv
                                  sprintf "> Content root: %s" hostCtx.HostingEnvironment.ContentRootPath
                                  sprintf "> %s @ %s" RuntimeInformation.FrameworkDescription RuntimeInformation.OSDescription |]
                           for m in msg do
                               Log.Information m |> ignore)


                // [3] Application DI-based services
                let builder = builder.ConfigureServices(fun hostCtx (services: IServiceCollection) -> services.AddControllers() |> ignore)


                // [4] Application setup (the rest)
                let builder =
                    builder.Configure(fun webHostBuilder appBuilder ->
                        let env = webHostBuilder.HostingEnvironment
                        let app = appBuilder

                        (if (env.IsDevelopment()) then app.UseDeveloperExceptionPage()
                         else app.UseHsts())
                        |> ignore

                        app.UseSerilogRequestLogging() |> ignore

                        app.UseRouting() |> ignore

                        app.UseAuthorization() |> ignore

                        app.UseEndpoints(fun endpoints -> endpoints.MapControllers() |> ignore) |> ignore)


                builder.Build().Run()
                exitCodeOk

            with e ->
                eprintfn "Fatal error: %A" e
                exitCodeErr

        finally
            Log.Information "+++++ Stopping NetCoreBB +++++\n\n"
            Log.CloseAndFlush()
