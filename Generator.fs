namespace Resumaker

open Types
open System

[<RequireQualifiedAccess>]
module Templates =
    open System.IO
    open System.Reflection
    open Scriban

    let defaultTemplate (data: Resume) =
        let path =
            let dir =
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)

            Path.Combine(dir, "templates", "default.html")

        let template =
            let content = File.ReadAllText path
            Template.Parse(content, sourceFilePath = path)

        template.Render(data)

    let fromDisk (data: Resume) (filePath: string) =
        let template =
            let content = File.ReadAllText filePath
            Template.Parse(content, sourceFilePath = filePath)

        template.Render(data)

[<RequireQualifiedAccess>]
module Generator =
    open System.Text
    open System.IO

    let html (data: Resume) (cwd: string) (templateFilePath: Option<string>) =
        let html =
            match templateFilePath with
            | Some path -> Templates.fromDisk data path
            | None -> Templates.defaultTemplate data

        let path =
            Path.Combine(cwd, sprintf "Resume-%s.html" data.Language.Name)

        use stream = File.OpenWrite(path)
        let bytes = Encoding.UTF8.GetBytes html

        stream.Write(ReadOnlySpan bytes)
