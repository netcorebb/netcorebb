#r "paket:
nuget Fake.DotNet.Cli
nuget Fake.IO.FileSystem
nuget Fake.Core.Target //"
#load ".fake/NetCoreBB.fsx/intellisense.fsx"

open Fake.Core
open Fake.Core.TargetOperators
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators

Target.initEnvironment ()

Target.create "Clean" (fun _ ->
    //!! "src/**/bin"
    //++ "src/**/obj"
    //|> Shell.cleanDirs
    Trace.trace "Clean")

Target.create "Build" (fun _ -> !!"NetCoreBB/*.csproj" ++ "NetCoreBB.*/*.csproj" |> Seq.iter (DotNet.build id))

Target.create "Frontend" (fun _ -> ())

Target.create "All" ignore ==> "Build" ==> "All"

Target.runOrDefault "Build"
