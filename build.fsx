// --------------------------------------------------------------------------------------
// FAKE build script 
// --------------------------------------------------------------------------------------
#I "packages/FAKE/tools"
#r "FakeLib.dll"

open Fake 
open Fake.Git
open Fake.AssemblyInfoFile
open Fake.ReleaseNotesHelper
open Fake.Testing
open Fake.DocuHelper

open System
open System.IO
open Fake.AppVeyor

// Information about each project is used
//  - for version and project name in generated AssemblyInfo file
//  - by the generated NuGet package 

type ProjectInfo =
  { /// The name of the project 
    /// (used by attributes in AssemblyInfo, name of a NuGet package and directory in 'src')
    Name : string
    /// Short summary of the project
    /// (used as description in AssemblyInfo and as a short summary for NuGet package)
    Summary : string
  }

let releaseNotes = "System.Domain Release Notes.md"
let tags = "csharp ddd Domain-Driven-Design Models Starters-Kit"
let gitOwner = "stijnmoreels"
let gitHome = sprintf "ssh://github.com/%s" gitOwner
let projectName = "System.Domain"
let summary = "System.Domain is a minimum extension library to write more secure Domain Model code in C#."
let solution = projectName + ".sln"

// Read additional information from the release notes document
Environment.CurrentDirectory <- __SOURCE_DIRECTORY__
let release = LoadReleaseNotes releaseNotes

let output = "./bin/"
let docs = "./docs/"

let isAppVeyorBuild = buildServer = BuildServer.AppVeyor
let buildDate = DateTime.UtcNow
let buildVersion = 
    let isVersionTag tag = Version.TryParse tag |> fst
    let hasRepoVersionTag = isAppVeyorBuild && AppVeyorEnvironment.RepoTag && isVersionTag AppVeyorEnvironment.RepoTagName
    let assemblyVersion = if hasRepoVersionTag then AppVeyorEnvironment.RepoTagName else release.NugetVersion
    if isAppVeyorBuild then sprintf "%s-b%s" assemblyVersion AppVeyorEnvironment.BuildNumber
    else assemblyVersion

let packages =
  [
    { Name = projectName
      Summary = summary
    }
  ]

Target "Clean" <| fun _ ->
    CleanDirs [output]

Target "AssemblyInfo" <| fun _ ->
    packages |> Seq.iter (fun package ->
    let fileName = "src/" + package.Name + "/AssemblyInfo.cs"
    CreateCSharpAssemblyInfo fileName
        ([Attribute.Title package.Name
          Attribute.Product package.Name
          Attribute.Description package.Summary
          Attribute.Version release.AssemblyVersion
          Attribute.FileVersion release.AssemblyVersion
        ])
    )

let assertExitCodeZero x = if x = 0 then () else failwithf "Command failed with exit code %i" x
let shellExec cmd args dir =
    printfn "%s %s" cmd args
    Shell.Exec(cmd, args, dir) |> assertExitCodeZero

Target "Restore" <| fun _ ->
    shellExec "dotnet" (sprintf "restore /p:Version=%s %s" buildVersion solution) "."

Target "Build" <| fun _ ->
    shellExec "dotnet" (sprintf "build --no-restore --configuration Release") "."
    !! (sprintf "./src/%s/bin/Release/**/*.*" projectName)
    |> Copy output

Target "RunTests" <| fun _ ->
    shellExec "dotnet" "xunit" <| sprintf "tests/%s.Tests" projectName

// Target "Release" <| fun _ ->
//     let user =
//         match getBuildParam "github-user" with
//         | s when not (String.IsNullOrWhiteSpace s) -> s
//         | _ -> getUserInput "Username: "
//     let pw =
//         match getBuildParam "github-pw" with
//         | s when not (String.IsNullOrWhiteSpace s) -> s
//         | _ -> getUserPassword "Password: "
//     let remote =
//         Git.CommandHelper.getGitResult "" "remote -v"
//         |> Seq.filter (fun (s: string) -> s.EndsWith("(push)"))
//         |> Seq.tryFind (fun (s: string) -> s.Contains(gitOwner + "/" + gitName))
//         |> function None -> gitHome + "/" + gitName | Some (s: string) -> s.Split().[0]

//     StageAll ""
//     Git.Commit.Commit "" (sprintf "Bump version to %s" release.NugetVersion)
//     Branches.pushBranch "" remote (Information.getBranchName "")

//     Branches.tag "" release.NugetVersion
//     Branches.pushTag "" remote release.NugetVersion

//     // release on github
//     createClient user pw
//     |> createDraft gitOwner gitName release.NugetVersion (release.SemVer.PreRelease <> None) release.Notes
//     // to upload a file: |> uploadFile "PATH_TO_FILE"
//     |> releaseDraft
//     |> Async.RunSynchronously

Target "Docs" <| fun _ ->
    let input = sprintf "./%s/%s.xml" output projectName
    shellExec 
        "./packages/Vsxmd/tools/Vsxmd.exe" 
        (sprintf "%s %s%s" input docs "api-reference.md")
        "."

"Clean"
==> "Restore"
==> "Build"
==> "RunTests"
==> "Docs"

RunTargetOrDefault "RunTests"