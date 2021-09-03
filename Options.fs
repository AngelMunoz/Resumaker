namespace Resumaker

open Argu
open Types

module Options =

    [<RequireQualifiedAccess>]
    type InitArgs =
        | [<AltCommandLine("-p")>] Path of string option

        interface IArgParserTemplate with
            member this.Usage: string =
                match this with
                | Path _ -> "Where should the \"resumaker.json\" file be created."

        static member GetOptions(results: ParseResults<InitArgs>) : InitOptions =
            { Path = results.TryGetResult(Path) |> Option.flatten }

    [<RequireQualifiedAccess>]
    type GenerateArgs =
        | [<AltCommandLine("-p")>] Path of string option
        | [<AltCommandLine("-o")>] Output of string option
        | [<AltCommandLine("-t")>] Template of string option
        | [<AltCommandLine("-l"); Last>] Language of string list

        interface IArgParserTemplate with
            member this.Usage: string =
                match this with
                | Path _ -> "Specify a path to the \"resumaker.json\" file."
                | Output _ -> "Where should we put the resulting file."
                | Template _ -> "The Full or relative file path to the custom template to use."
                | Language _ ->
                    "A list of languages you want to generate your resume in, a json file for each specified language is required."

        static member GetOptions(results: ParseResults<GenerateArgs>) : GenerateOptions =
            { Path = results.TryGetResult(Path) |> Option.flatten
              Output = results.TryGetResult(Output) |> Option.flatten
              TemplatePath = results.TryGetResult(Output) |> Option.flatten
              Language =
                  results.TryGetResult(Language)
                  |> Option.defaultValue List.empty }

    type ResumakerArgs =
        | [<CliPrefix(CliPrefix.None)>] Init of ParseResults<InitArgs>
        | [<CliPrefix(CliPrefix.None)>] Generate of ParseResults<GenerateArgs>

        interface IArgParserTemplate with
            member this.Usage: string =
                match this with
                | Init _ -> "Creates basic files and directories to start using resumaker."
                | Generate _ ->
                    "Generates an HTML file with the contents of your resumaker.json file or files if you have multiple languages."
