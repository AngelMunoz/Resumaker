namespace Resumaker

open System.Collections.Generic

module Types =
    type Link = { Name: string; Url: string }

    type Project =
        { Name: string
          Description: string
          ToolOrStack: string seq
          Url: string }

    type Job =
        { Employer: string
          Title: string
          Summary: string }

    type Skill = { Name: string; Experience: string }

    type Profile =
        { Name: string
          LastName: string
          PersonalSummary: string
          Objectives: string
          Picture: string
          Email: string }

    type Language =
        { Name: string
          Keywords: IDictionary<string, string> }

    type Resume =
        { Language: Language
          Profile: Profile
          Skills: Skill seq
          PreviousJobs: Job seq
          Projects: Project seq
          DevLinks: Link seq
          SocialMedia: Link seq }

    type ResumakerData = { ResumeList: Resume seq }

    type InitOptions = { Path: string option }

    type GenerateOptions =
        { Path: string option
          Output: string option
          TemplatePath: string option
          Language: string seq }

module Exceptions =
    exception CommandNotParsedException of string
    exception TemplateException of string
    exception VersionRequestedException
    exception HelpRequestedException
