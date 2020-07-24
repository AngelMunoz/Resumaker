// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open System.IO
open Scryber.Components


[<EntryPoint>]
let main argv =
    let cwd = Environment.CurrentDirectory
    let tmp = Path.GetTempPath()
    let path = Path.Combine(cwd, "templates", "default.pdfx")
    let output = Path.Combine(cwd, "default.pdf")

    let doc = PDFDocument.ParseDocument(path)
    doc.ProcessDocument(output, FileMode.OpenOrCreate)
    0 // return an integer exit code