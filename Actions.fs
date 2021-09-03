namespace Resumaker

[<RequireQualifiedAccess>]
module Actions =
    open Types
    open Options
    open System
    open System.IO
    open System.Text.Json
    open System.Reflection

    let init (opts: InitOptions) : int =
        let templatePath =
            let dirname =
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)

            Path.Combine(dirname, "resumaker.sample.json")

        printfn """Hi! thanks for using Resumaker. Creating "resumaker.json" file."""
        let bytes = File.ReadAllBytes(templatePath)

        match String.IsNullOrEmpty opts.Path with
        | false ->
            let path =
                Path.Combine(opts.Path, "resumaker.json")

            File.WriteAllBytes(path, bytes)
            printfn """Wrote "resumaker.json" file at "%s".""" path
        | true ->
            let path =
                Path.Combine(Directory.GetCurrentDirectory(), "resumaker.json")

            File.WriteAllBytes(path, bytes)
            printfn """Wrote "resumaker.json" file at "%s".""" path

        0


    let generateHtml (resume: Resume) (cwd: string) = Generator.html resume cwd

    let generate (opts: GenerateOptions) : int =
        let path =
            match String.IsNullOrEmpty opts.Path with
            | true ->
                let path =
                    Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "resumaker.json"))

                if File.Exists path then
                    path
                else
                    raise (ArgumentException(sprintf """The "resumaker.json" file does not exist at "%s".""" path))
            | false ->
                let path = Path.GetFullPath(opts.Path)

                match path.EndsWith ".json", File.Exists(path) with
                | true, true -> path
                | false, true ->
                    Directory.GetFiles(path, "resumaker.json")
                    |> Seq.head
                | true, false
                | false, false ->
                    raise (ArgumentException(sprintf """The "resumaker.json" file does not exist at "%s".""" path))

        let file = File.ReadAllText path

        let data =
            let jsonopts = JsonSerializerOptions()
            jsonopts.AllowTrailingCommas <- true
            jsonopts.ReadCommentHandling <- JsonCommentHandling.Skip
            jsonopts.PropertyNamingPolicy <- JsonNamingPolicy.CamelCase
            jsonopts.PropertyNameCaseInsensitive <- true
            JsonSerializer.Deserialize<ResumakerData>(file, jsonopts)

        let cwd = Path.GetDirectoryName path

        let customPath =
            if
                not (String.IsNullOrEmpty opts.TemplatePath)
                && File.Exists(Path.GetFullPath(opts.TemplatePath))
            then
                Some opts.TemplatePath
            else
                None

        match opts.Language |> Seq.length > 0 with
        | true ->
            [| for language in opts.Language do
                   data.ResumeList
                   |> Seq.find (fun resume -> resume.Language.Name.ToLowerInvariant() = language.ToLowerInvariant()) |]
            |> Array.Parallel.map
                (fun resume ->
                    match opts.Output.ToLowerInvariant().Trim() with
                    | "html" -> generateHtml resume cwd customPath
                    | _ ->
                        printfn "Warning: HTML is currently the only output"
                        generateHtml resume cwd customPath)

        | false ->
            data.ResumeList
            |> Seq.toArray
            |> Array.Parallel.map
                (fun resume ->
                    match opts.Output.ToLowerInvariant().Trim() with
                    | "html" -> generateHtml resume cwd customPath
                    | _ ->
                        printfn "Warning: HTML is currently the only output"
                        generateHtml resume cwd customPath)
        |> ignore

        0
