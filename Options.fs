namespace Resumaker

module Options =
    open CommandLine

    [<Verb("init", HelpText = "Creates basic files and directories to start using resumaker.")>]
    type InitOptions =
        { [<Option('p',
                   "path",
                   Required = false,
                   HelpText =
                       "Where should the resumaker.json should be created defaults to wherever the binary has been executed")>]
          Path: string }

    [<Verb("generate", HelpText = "Generates a resume for each lang you have inside the resumaker.json file")>]
    type GenerateOptions =
        { [<Option('p', "path", Required = false, HelpText = "Specify a path to the resumaker.json file")>]
          Path: string
          [<Option('o',
                   "output",
                   Required = false,
                   HelpText = "Type of the produced output (Html for the moment)",
                   Default = "html")>]
          OutputType: string
          [<Option('t',
                   "template",
                   Required = false,
                   HelpText = "the file path to the custom template to use for generation")>]
          TemplatePath: string
          [<Option('l', "language", Required = false, HelpText = "Create only the specified language resume")>]
          Language: seq<string> }
