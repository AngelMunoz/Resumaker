namespace Resumaker

open Types
open Exceptions
open System.IO
open System.Reflection
open Scriban
open FsToolkit.ErrorHandling

[<RequireQualifiedAccess>]
module Templates =

    let defaultTemplate (data: Resume) : Result<string, exn> =
        let path =
            let dir =
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)

            Path.Combine(dir, "templates", "default.html")

        result {
            try
                let template =
                    let content = File.ReadAllText path
                    Template.Parse(content, sourceFilePath = path)

                return template.Render(data)
            with
            | ex -> return! ex.Message |> TemplateException |> Error
        }


    let fromDisk (data: Resume) (filePath: string) : Result<string, exn> =
        try
            let template =
                let content = File.ReadAllText filePath
                Template.Parse(content, sourceFilePath = filePath)

            template.Render(data) |> Ok
        with
        | ex -> ex.Message |> TemplateException |> Error

[<RequireQualifiedAccess>]
module Generator =

    let html
        (data: Resume)
        (cwd: string)
        (templateFilePath: string option)
        (output: string option)
        : Result<unit, exn> =
        result {
            let! html =
                match templateFilePath with
                | Some path -> Templates.fromDisk data path
                | None -> Templates.defaultTemplate data

            let path =
                match output with
                | Some output -> Path.GetFullPath(output)
                | None -> (Path.Combine(cwd, sprintf "Resume-%s.html" data.Language.Name))

            File.WriteAllText(path, html)
        }
