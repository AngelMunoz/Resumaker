// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp
open Argu
open Resumaker
open Resumaker.Exceptions
open Resumaker.Options
open FsToolkit.ErrorHandling

[<EntryPoint>]
let main argv =
    let getCommand () : Result<ResumakerArgs, exn> =
        result {
            let! parser =
                try
                    ArgumentParser.Create<ResumakerArgs>() |> Ok
                with
                | ex -> CommandNotParsedException(ex.Message) |> Error

            let! parsed =
                try
                    parser.Parse argv |> Ok
                with
                | :? Argu.ArguParseException as ex ->
                    printfn "%s" (parser.PrintUsage())
                    CommandNotParsedException(ex.Message) |> Error


            let cliArgs = parsed.GetAllResults()

            match cliArgs with
            | [ Init subcmd ] -> return Init subcmd
            | [ Generate subcmd ] -> return Generate subcmd
            | args -> return! CommandNotParsedException $"%A{args}" |> Error
        }

    result {
        let! parsed = getCommand ()

        return!
            match parsed with
            | Init value -> Actions.init (InitArgs.GetOptions value)
            | Generate value -> Actions.generate (GenerateArgs.GetOptions value)
    }
    |> fun result ->
        match result with
        | Ok exitCode -> exitCode
        | Error ex ->
            match ex with
            | VersionRequestedException
            | HelpRequestedException -> 0
            | ex ->
                printfn "%O" ex
                1
