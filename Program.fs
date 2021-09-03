// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open Resumaker
open Resumaker.Options
open System

[<EntryPoint>]
let main argv =
    let result =
        Parser.Default.ParseArguments<InitOptions, GenerateOptions>(argv)

    match result with
    | :? (Parsed<obj>) as parsed ->
        match parsed.Value with
        | :? InitOptions as opts ->
            try
                Actions.init opts
            with
            | :? UnauthorizedAccessException as un ->
                eprintfn "Failed to read/write to disk due to user permissions"
                eprintfn "Ensure you have read/write access to the selected directory"
                Debug.WriteLine(sprintf "%O" un)
                3
            | :? FileNotFoundException as fn ->
                eprintfn
                    "We couldn't find the file that was requested please ensure 'resumaker.json' file exists at the selected path"

                Debug.WriteLine(sprintf "%O" fn)
                3
            | :? PathTooLongException as pt ->
                eprintfn "Selected path is out of the system allowed range"
                Debug.WriteLine(sprintf "%O" pt)
                3
        | :? GenerateOptions as opts ->
            try
                Actions.generate opts
            with
            | :? ArgumentException as ex ->
                eprintfn "%s" ex.Message
                3
        | somethingelse ->
            Debug.WriteLine(sprintf "%O" somethingelse)
            3
    | :? (NotParsed<obj>) as notParsed ->
        Debug.WriteLine("Not Parsed Errors:")

        for err in notParsed.Errors do
            Debug.WriteLine(sprintf "\t%A %b" err.Tag err.StopsProcessing)

        2
    | somethingelse ->
        Debug.WriteLine(sprintf "%O" somethingelse)
        3
