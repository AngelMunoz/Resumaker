namespace Resumaker

module Types =

    type Link = { Name: string; Url: string }

    type Project =
        { Name: string
          ToolOrStack: seq<string>
          Url: string }

    type Job = { Employer: string; Summary: string }

    type Skill = { Name: string; Years: int }

    type Profile =
        { Name: string
          LastName: string
          PersonalSummary: string
          Objectives: string
          Picture: string
          Email: string }

    type Resume =
        { Language: string
          Profile: Profile
          Skills: seq<Skill>
          PreviousJobs: seq<Job>
          Projects: seq<Project>
          DevLinks: seq<Link>
          SocialMedia: seq<Link> }


    type ResumakerData =
        { ResumeList: seq<Resume>
          CustomTemplatePath: string }
