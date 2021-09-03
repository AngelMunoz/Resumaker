#!/usr/bin/env -S dotnet fsi
#r "nuget: Suave"
#r "nuget: CliWrap"
#r "nuget: FSharp.Control.AsyncSeq"


open System
open System.IO
open System.Runtime.InteropServices
open FSharp.Control

open CliWrap

open Suave
open Suave.Redirection
open Suave.Filters
open Suave.Operators


let resumakerCmd () =
    let isWindows =
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows)

    Cli
        .Wrap(if isWindows then "dotnet.exe" else "")
        .WithArguments("run -- generate -o ./public -c ./resumaker.sample.json")
        .WithStandardErrorPipe(PipeTarget.ToStream(Console.OpenStandardError()))
        .WithStandardOutputPipe(PipeTarget.ToStream(Console.OpenStandardOutput()))
        .WithValidation(CommandResultValidation.None)

let generate () =
    async {
        let task = resumakerCmd().ExecuteAsync()
        return! task.Task |> Async.AwaitTask
    }

let (|Generate|Clear|Exit|Unknown|) =
    function
    | "g"
    | "generate" -> Generate
    | "clear" -> Clear
    | "exit"
    | "stop" -> Exit
    | value -> Unknown value


let onStdinAsync (value: string) =
    async {
        match value with
        | Generate ->
            async {
                printfn "Generating Resume"

                let! result = generate ()
                printfn "Finished in: %s - %i" (result.RunTime.ToString()) result.ExitCode
            }
            |> Async.Start
        | Clear -> Console.Clear()
        | Exit ->
            printfn "Finishing the session"
            exit 0
        | Unknown value -> printfn "Unknown option [%s]" value
    }

let stdinAsyncSeq () =
    let readFromStdin () =
        Console.In.ReadLineAsync() |> Async.AwaitTask

    asyncSeq {
        "generate"

        while true do
            let! value = readFromStdin ()
            value
    }
    |> AsyncSeq.distinctUntilChanged
    |> AsyncSeq.iterAsync onStdinAsync

let app =
    choose [ path "/"
             >=> GET
             >=> Files.browseFileHome "Resume-es-MX.html"
             GET >=> Files.browseHome
             RequestErrors.NOT_FOUND "Not Found"
             >=> redirect "/" ]

let config (publicPath: string option) =
    let path = Path.GetFullPath("./public")

    printfn $"Serving content from {path}"

    { defaultConfig with
          bindings = [ HttpBinding.createSimple HTTP "0.0.0.0" 3000 ]
          homeFolder = Some path
          compressedFilesFolder = Some(Path.GetFullPath "./.compressed") }

stdinAsyncSeq () |> Async.Start
// dotnet fsi suave.fsx built to show how bundled files work
startWebServer (config (fsi.CommandLineArgs |> Array.tryLast)) app
