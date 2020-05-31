#addin "nuget:?package=Cake.Npm&version=0.17.0"
#addin "nuget:?package=Cake.Coverlet&version=2.4.2"
#tool "nuget:?package=GitVersion.CommandLine&version=5.1.3"
#addin "nuget:?package=SharpZipLib&version=1.2.0"
#addin "nuget:?package=Cake.Compression&version=0.2.4"
#addin "Cake.FileHelpers&version=3.2.1"

var target = Argument<string>("target", "Default");
var buildPath = Directory("./build-artifacts");
var publishPath = buildPath + Directory("publish");
var releasePath = buildPath + Directory("release");
var coverPath = buildPath + Directory("cover");
var coverClientPath = coverPath + Directory("client");
var coverServerPath = coverPath + Directory("server");


Task("__Clean")
    .Does(() => {
        if (BuildSystem.IsLocalBuild) {
            CleanDirectories(new DirectoryPath[] {
                buildPath
            });

            CleanDirectories("../**/bin");
            CleanDirectories("../**/obj");
            if (DirectoryExists("../Source/Initium.Portal.Web/node_modules")) {
                DeleteDirectory("../Source/Initium.Portal.Web/node_modules", new DeleteDirectorySettings {
                    Recursive = true,
                    Force = true
                });
            }
        }  

        CreateDirectory(releasePath);
        CreateDirectory(publishPath);
        CreateDirectory(coverPath);   
    });

Task("__Versioning")
    .Does(() => {
        var version = GitVersion(new GitVersionSettings {
            UpdateAssemblyInfo = true
        });
    
        TFBuild.Commands.UpdateBuildNumber(version.SemVer);
    });

Task("__RestorePackages")
    .Does(() => {
        var npmInstallSettings = new NpmInstallSettings {
            WorkingDirectory = "../Source/Initium.Portal.Web",
            LogLevel = NpmLogLevel.Silent
        };
        NpmInstall(npmInstallSettings);
    });

Task("__Build")
    .Does(() => {
        var npmRunScriptSettings = new NpmRunScriptSettings {
           ScriptName = "release:build",
           WorkingDirectory = "../Source/Initium.Portal.Web",
           LogLevel = NpmLogLevel.Silent
        };		
        NpmRunScript(npmRunScriptSettings);  

        var settings = new DotNetCoreBuildSettings {
            Configuration = "Release"
        };
        DotNetCoreBuild("../Initium.Portal.sln", settings);

        var msBuildSettings = new MSBuildSettings {
            Verbosity = Verbosity.Minimal,
            Configuration = "Database",
            PlatformTarget = PlatformTarget.MSIL
        };
        MSBuild("../Initium.Portal.sln", msBuildSettings);
    });
Task("__Test")
    .Does(() => {
        var testResults = MakeAbsolute(coverServerPath + File("xunit-report.xml")).FullPath;
        var testSettings = new DotNetCoreTestSettings {
            Configuration = "Release",
            NoBuild = true,
            Logger = $"trx;LogFileName={testResults};verbosity=normal"
        };

        var coverletSettings = new CoverletSettings {
            CollectCoverage = true,
            CoverletOutputFormat = CoverletOutputFormat.opencover|CoverletOutputFormat.lcov|(CoverletOutputFormat)12,
            CoverletOutputDirectory = coverServerPath,
            CoverletOutputName = "coverage"        
        };

        DotNetCoreTest("../Initium.Portal.sln", testSettings, coverletSettings);
        
        var npmRunScriptSettings = new NpmRunScriptSettings {
            ScriptName = "test:coverage",
            WorkingDirectory = "../Source/Initium.Portal.Web",
            //LogLevel = NpmLogLevel.Silent
        };		
        NpmRunScript(npmRunScriptSettings);  
        CopyDirectory("../Source/Initium.Portal.Web/coverage", coverClientPath);
        System.IO.StreamReader file =  new System.IO.StreamReader(MakeAbsolute(File("./build-artifacts/cover/server/coverage.info")).ToString()); 
        string line;
        while((line = file.ReadLine()) != null ){  
            if (line.IndexOf("Source\\Initium.Portal.Web\\Program.cs") == -1) {
                continue;
            }
            
            ReplaceTextInFiles("./build-artifacts/cover/client/lcov.info", "Resources\\Scripts\\", line.Replace("SF:",string.Empty).Replace("Program.cs", string.Empty) + "Resources\\Scripts\\");
            break;
             
        }  
        file.Close();  
        
    });
Task("__Publish")
    .Does(() => {
        var pubSettings = new DotNetCorePublishSettings {
            Configuration = "Release",
            OutputDirectory = publishPath
        };
        
        DotNetCorePublish("../source/Initium.Portal.Web/Initium.Portal.Web.csproj", pubSettings);

            });
Task("__Package")
    .Does(() => {
        ZipCompress(publishPath, releasePath + File("Initium.Portal.zip"));        
        ZipCompress("../Data/Initium.Portal.Data/bin/Debug/Initium.Portal.Data.dacpac", releasePath + File("Initium.Portal.Data.zip"));        
    });

Task("Build")
    .IsDependentOn("__Clean")
    .IsDependentOn("__Versioning")    
    .IsDependentOn("__RestorePackages")
    .IsDependentOn("__Build")
    .IsDependentOn("__Test")
    .IsDependentOn("__Publish")
    .IsDependentOn("__Package")
    ;

Task("Default")
    .IsDependentOn("Build");

RunTarget(target);