#addin "nuget:?package=Cake.Npm&version=0.17.0"
#addin "nuget:?package=Cake.Coverlet&version=2.4.2"

var target = Argument<string>("target", "Default");
var buildPath = Directory("./build-artifacts");
var publishPath = buildPath + Directory("publish");
var releasePath = buildPath + Directory("release");
var coverPath = buildPath + Directory("cover");


Task("__Clean")
    .Does(() => {
        if (BuildSystem.IsLocalBuild) {
            CleanDirectories(new DirectoryPath[] {
                buildPath
            });

            CleanDirectories("../**/bin");
            CleanDirectories("../**/obj");
            if (DirectoryExists("../Source/Stance.Web/node_modules")) {
                DeleteDirectory("../Source/Stance.Web/node_modules", new DeleteDirectorySettings {
                    Recursive = true,
                    Force = true
                });
            }
        }  

        CreateDirectory(releasePath);
        CreateDirectory(publishPath);
        CreateDirectory(coverPath);   
    });

Task("__RestorePackages")
    .Does(() => {
        var npmInstallSettings = new NpmInstallSettings {
            WorkingDirectory = "../Source/Stance.Web"
        };
        NpmInstall(npmInstallSettings);
    });

Task("__Build")
    .Does(() => {
        var npmRunScriptSettings = new NpmRunScriptSettings {
           ScriptName = "release:build",
           WorkingDirectory = "../Source/Stance.Web"
        };		
        NpmRunScript(npmRunScriptSettings);  

        var settings = new DotNetCoreBuildSettings {
            Configuration = "Release"
        };
        DotNetCoreBuild("../Stance.sln", settings);

        var msBuildSettings = new MSBuildSettings {
            Verbosity = Verbosity.Minimal,
            Configuration = "Database",
            PlatformTarget = PlatformTarget.MSIL
        };
        MSBuild("../Stance.sln", msBuildSettings);
    });
Task("__Test")
    .Does(() => {
        var testResults = MakeAbsolute(coverPath + File("xunit-report.xml")).FullPath;
        var testSettings = new DotNetCoreTestSettings {
            Configuration = "Release",
            NoBuild = true,
            Logger = $"trx;LogFileName={testResults};verbosity=normal"
    };

    var coverletSettings = new CoverletSettings {
        CollectCoverage = true,
        CoverletOutputFormat = CoverletOutputFormat.opencover|(CoverletOutputFormat)12,
        CoverletOutputDirectory = coverPath,
        CoverletOutputName = "coverage"        
    };

    DotNetCoreTest("../Stance.sln", testSettings, coverletSettings);
    });
Task("__Publish")
    .Does(() => {
        var pubSettings = new DotNetCorePublishSettings {
            Configuration = "Release",
            OutputDirectory = publishPath
        };
        
        DotNetCorePublish("../source/Stance.Web/Stance.Web.csproj", pubSettings);

            });
Task("__Package")
    .Does(() => {
        Zip(publishPath, releasePath + File("DeviousCreation.CqrsStarterTemplate.Web.zip"));        
        Zip("../Data/Stance.Data/bin/Debug/Stance.Data.dacpac", releasePath + File("Stance.Data.zip"));        
    });

Task("Build")
    .IsDependentOn("__Clean")
    .IsDependentOn("__RestorePackages")
    .IsDependentOn("__Build")
    .IsDependentOn("__Test")
    .IsDependentOn("__Publish")
    .IsDependentOn("__Package")
    ;

Task("Default")
    .IsDependentOn("Build");

RunTarget(target);