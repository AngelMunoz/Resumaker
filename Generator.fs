namespace Resumaker

open Types
open Feliz.ViewEngine
open System

[<RequireQualifiedAccess>]
module Templates =

    let defaultTemplate (data: Resume) =
        Html.html
            [ Html.head []
              Html.body [ Html.h1 data.Language ] ]

[<RequireQualifiedAccess>]
module Generator =
    open System.Text
    open System.IO

    let html (data: Resume) (cwd: string) =
        let content = Templates.defaultTemplate data
        let html = Render.htmlDocument content

        let path =
            Path.Combine(cwd, sprintf "Resume-%s.html" data.Language)

        use stream = File.OpenWrite(path)
        let bytes = Encoding.UTF8.GetBytes html

        stream.Write(ReadOnlySpan bytes)
