namespace Resumaker

open Types
open Exceptions
open System.IO
open System.Text.Json
open System.Reflection
open FsToolkit.ErrorHandling

[<RequireQualifiedAccess>]
module Actions =

    let init (opts: InitOptions) : Result<int, exn> =

        let templatePath =
            let dirname =
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)

            Path.Combine(dirname, "resumaker.sample.json")

        printfn """Hi! thanks for using Resumaker. Creating "resumaker.json" file."""

        try
            let bytes = File.ReadAllBytes(templatePath)

            let path =
                match opts.Path with
                | Some path -> Path.GetFullPath(path)
                | None -> Path.Combine(Directory.GetCurrentDirectory(), "resumaker.json")

            File.WriteAllBytes(path, bytes)
            printfn """Wrote "resumaker.json" file at "%s".""" path
            Ok 0
        with
        | ex -> ex |> Error

    let generate (opts: GenerateOptions) : Result<int, exn> =
        let path =
            match opts.Path with
            | Some path -> Path.GetFullPath(path)
            | None -> Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "resumaker.json"))

        let file = File.ReadAllText path


        let data =
            let jsonopts = JsonSerializerOptions()
            jsonopts.AllowTrailingCommas <- true
            jsonopts.ReadCommentHandling <- JsonCommentHandling.Skip
            jsonopts.PropertyNamingPolicy <- JsonNamingPolicy.CamelCase
            jsonopts.PropertyNameCaseInsensitive <- true
            JsonSerializer.Deserialize<ResumakerData>(file, jsonopts)

        let cwd = Path.GetDirectoryName path

        result {
            try
                match opts.Output with
                | Some output -> Directory.CreateDirectory(output) |> ignore
                | None -> ()
            with
            | ex -> return! ex |> Error

            let results =
                match opts.Language |> Seq.length > 0 with
                | true ->
                    [| for language in opts.Language do
                           data.ResumeList
                           |> Seq.find
                               (fun resume -> resume.Language.Name.ToLowerInvariant() = language.ToLowerInvariant()) |]
                    |> Array.Parallel.map (fun resume -> Generator.html resume cwd opts.TemplatePath opts.Output)

                | false ->
                    data.ResumeList
                    |> Seq.toArray
                    |> Array.Parallel.map (fun resume -> Generator.html resume cwd opts.TemplatePath opts.Output)

            results
            |> Array.iter
                (fun r ->
                    match r with
                    | Error err -> eprintfn "%s" err.Message
                    | _ -> ())

            do!
                results
                |> Array.forall Result.isOk
                |> Result.requireTrue (TemplateException "Failed to produce one or more of the templates")

            return 0
        }
