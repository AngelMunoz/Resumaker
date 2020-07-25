// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open CommandLine
open Resumaker
open Resumaker.Options


[<EntryPoint>]
let main argv =
    let result = Parser.Default.ParseArguments<InitOptions, GenerateOptions>(argv)

    match result with 
    | :? Parsed<obj> as parsed ->
        match parsed.Value with 
        | :? InitOptions as opts -> Actions.init opts
        | :? GenerateOptions as opts -> Actions.generate opts
        | somethingelse ->
#if DEBUG        
            eprintfn "%O" somethingelse
#endif
            3
    | :? NotParsed<obj> as notParsed ->
#if DEBUG
        eprintfn "Not Parsed Errors:"
        for err in notParsed.Errors do
            eprintfn "\t%A %b" err.Tag err.StopsProcessing
#endif
        2
    | somethingelse ->
#if DEBUG        
        eprintfn "%O" somethingelse
#endif
        3


