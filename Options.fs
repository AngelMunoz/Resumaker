namespace Resumaker

open Argu

module Options =

    [<RequireQualifiedAccess>]
    type InitArgs =
        | [<AltCommandLine("-p")>] Path of string option

        interface IArgParserTemplate with
            member this.Usage: string =
                match this with
                | Path _ -> "Where should the \"resumaker.json\" file be created."

    [<RequireQualifiedAccess>]
    type GenerateArgs =
        | [<AltCommandLine("-p")>] Path of string option
        | [<AltCommandLine("-o")>] Output of string option
        | [<AltCommandLine("-t")>] Template of string option
        | [<AltCommandLine("-l")>] Language of string seq

        interface IArgParserTemplate with
            member this.Usage: string =
                match this with
                | Path _ -> "Specify a path to the \"resumaker.json\" file."
                | Output _ -> "Where should we put the resulting file."
                | Template _ -> "The Full or relative file path to the custom template to use."
                | Language _ ->
                    "A list of languages you want to generate your resume in, a json file for each specified language is required."

    type ResumakerArgs =
        | [<CliPrefix(CliPrefix.None)>] Init of ParseResults<InitArgs>
        | [<CliPrefix(CliPrefix.None)>] Generate of ParseResults<GenerateArgs>
        | [<First; AltCommandLine("-h")>] Help of bool option
        | [<First; AltCommandLine("-v")>] Version of bool option

        interface IArgParserTemplate with
            member this.Usage: string =
                match this with
                | Init _ -> "Creates basic files and directories to start using resumaker."
                | Generate _ ->
                    "Generates an HTML file with the contents of your resumaker.json file or files if you have multiple languages."
                | Help _ -> "Prints the usage dialog."
                | Version _ -> "Prints the tool version."
