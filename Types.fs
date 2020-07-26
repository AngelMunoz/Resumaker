namespace Resumaker

open System.Collections.Generic

module Types =
    [<CLIMutable>]
    type Link = { Name: string; Url: string }

    [<CLIMutable>]
    type Project =
        { Name: string
          ToolOrStack: seq<string>
          Url: string }

    [<CLIMutable>]
    type Job =
        { Employer: string
          Title: string
          Summary: string }

    [<CLIMutable>]
    type Skill = { Name: string; Experience: string }

    [<CLIMutable>]
    type Profile =
        { Name: string
          LastName: string
          PersonalSummary: string
          Objectives: string
          Picture: string
          Email: string }

    [<CLIMutable>]
    type Language =
        { Name: string
          Keywords: IDictionary<string, string> }

    [<CLIMutable>]
    type Resume =
        { Language: Language
          Profile: Profile
          Skills: seq<Skill>
          PreviousJobs: seq<Job>
          Projects: seq<Project>
          DevLinks: seq<Link>
          SocialMedia: seq<Link> }

    [<CLIMutable>]
    type ResumakerData = { ResumeList: seq<Resume> }
