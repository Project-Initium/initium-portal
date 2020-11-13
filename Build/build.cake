#addin "nuget:?package=Cake.Npm&version=0.17.0"
#addin "nuget:?package=Cake.Coverlet&version=2.4.2"
#tool "nuget:?package=GitVersion.CommandLine&version=5.0.0"
#addin "nuget:?package=SharpZipLib&version=1.2.0"
#addin "nuget:?package=Cake.Compression&version=0.2.4"
#addin "Cake.FileHelpers&version=3.2.1"

var target = Argument<string>("target", "Default");
var buildPath = Directory("./build-artifacts");

var publishPath = buildPath + Directory("publish");
var tenantPublishPath = publishPath + Directory("tenant");
var managementPublishPath = publishPath + Directory("management");
var databasePublishPath = publishPath + Directory("database");

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
            if (DirectoryExists("../node_modules")) {
               DeleteDirectory("../node_modules", new DeleteDirectorySettings {
                   Recursive = true,
                   Force = true
               });
            }
        }  

        CreateDirectory(releasePath);
        CreateDirectory(publishPath);
        CreateDirectory(tenantPublishPath);
        CreateDirectory(managementPublishPath);
        CreateDirectory(databasePublishPath);
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
            WorkingDirectory = "../",
            LogLevel = NpmLogLevel.Silent
        };
        NpmInstall(npmInstallSettings);
    });

Task("__Build")
    .Does(() => {
        var npmRunScriptSettings = new NpmRunScriptSettings {
           ScriptName = "management:release:build",
           WorkingDirectory = "../",
           LogLevel = NpmLogLevel.Silent
        };		
        NpmRunScript(npmRunScriptSettings);  
        
        npmRunScriptSettings.ScriptName = "tenant:release:build";

        var settings = new DotNetCoreBuildSettings {
            Configuration = "Release"
        };
        DotNetCoreBuild("../Initium.Portal.sln", settings);

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
    });
Task("__Publish")
    .Does(() => {
        var pubSettings = new DotNetCorePublishSettings {
            Configuration = "Release",
            OutputDirectory = managementPublishPath
        };
        DotNetCorePublish("../source/Initium.Portal.Web.Management/Initium.Portal.Web.Management.csproj", pubSettings);
        
        pubSettings.OutputDirectory = tenantPublishPath;
        DotNetCorePublish("../source/Initium.Portal.Web.Tenant/Initium.Portal.Web.Tenant.csproj", pubSettings);

        pubSettings.OutputDirectory = databasePublishPath;
        DotNetCorePublish("../data/Initium.Portal.Data.Build/Initium.Portal.Data.Build.csproj", pubSettings);
    });
Task("__Package")
    .Does(() => {
        ZipCompress(managementPublishPath, releasePath + File("Initium.Portal.Management.zip"));        
        ZipCompress(tenantPublishPath, releasePath + File("Initium.Portal.Tenant.zip"));        
        ZipCompress(databasePublishPath, releasePath + File("Initium.Portal.Data.zip"));
    });

Task("Build")
    .IsDependentOn("__Clean")
    .IsDependentOn("__Versioning")    
    .IsDependentOn("__RestorePackages")
    .IsDependentOn("__Build")
    .IsDependentOn("__Test")
    .IsDependentOn("__Publish")
    .IsDependentOn("__Package");

Task("Default")
    .IsDependentOn("Build");

RunTarget(target);