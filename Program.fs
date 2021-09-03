// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp
open Argu
open Resumaker
open Resumaker.Types
open Resumaker.Exceptions
open Resumaker.Options
open FsToolkit.ErrorHandling

[<EntryPoint>]
let main argv =
    let getCommand () : Result<ResumakerArgs, exn> =
        result {
            let parser = ArgumentParser.Create<ResumakerArgs>()

            let! parsed =
                try
                    parser.Parse argv |> Ok
                with
                | :? Argu.ArguParseException as ex -> CommandNotParsedException(ex.Message) |> Error


            let version =
                parsed.TryGetResult(Version)
                |> Option.map
                    (fun opt ->
                        if opt.IsNone then
                            true
                        else
                            opt |> Option.defaultValue false)
                |> Option.defaultValue false

            if version then
                printfn
                    $"{System
                           .Reflection
                           .Assembly
                           .GetEntryAssembly()
                           .GetName()
                           .Version.ToString()}"

                return! Error VersionRequestedException

            let help =
                parsed.TryGetResult(Help)
                |> Option.map
                    (fun opt ->
                        if opt.IsNone then
                            true
                        else
                            opt |> Option.defaultValue false)
                |> Option.defaultValue false

            if help then
                printfn "%s" (parser.PrintUsage())
                return! Error HelpRequestedException


            let cliArgs =
                parsed.GetAllResults()
                |> List.filter
                    (fun result ->
                        match result with
                        | Version _
                        | Help _ -> false
                        | _ -> true)

            match cliArgs with
            | [ Init subcmd ] -> return Init subcmd
            | [ Generate subcmd ] -> return Generate subcmd
            | args -> return! CommandNotParsedException $"%A{args}" |> Error
        }

    result {
        let! parsed = getCommand ()

        match parsed with
        | Init value -> return! Actions.init (InitArgs.GetOptions value)
        | Generate value -> return! Actions.generate (GenerateArgs.GetOptions value)
        | _ -> return 0
    }
    |> fun result ->
        match result with
        | Ok exitCode -> exitCode
        | Error ex ->
            match ex with
            | VersionRequestedException
            | HelpRequestedException -> 0
            | _ -> 1
