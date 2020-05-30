#addin "nuget:?package=SharpZipLib&version=1.2.0"
#addin "nuget:?package=Cake.Compression&version=0.2.4"

var target = Argument<string>("target", "Default");
var buildPath = Directory("./build-artifacts");
var templatePath = Directory("../__template");
var releasePath = buildPath + Directory("release");

Task("__TemplateCopy")
    .Does(() => {
        CreateDirectory(templatePath);
        CreateDirectory(templatePath + Directory("Build"));
        CopyFile("../Build/build.cake", templatePath + File("Build/build.cake"));
        CopyFile("../Build/build.ps1", templatePath + File("Build/build.ps1"));
        CopyFile("../Build/watch-tests.cmd", templatePath + File("Build/watch-tests.cmd"));
        CopyDirectory("../.vscode", templatePath + Directory(".vscode"));
        CopyDirectory("../Data", templatePath + Directory("Data"));
        CopyDirectory("../Shared", templatePath + Directory("Shared"));
        CopyDirectory("../Source", templatePath + Directory("Source"));
        CopyDirectory("../Tests", templatePath + Directory("Tests"));
        CopyFileToDirectory("../.editorconfig", templatePath);
        CopyFileToDirectory("../.gitignore", templatePath);
        CopyFileToDirectory("../CODE_OF_CONDUCT.md", templatePath);
        CopyFileToDirectory("../CONTRIBUTING.md", templatePath);
        CopyFileToDirectory("../docker-compose.yml", templatePath);
        CopyFileToDirectory("../GitVersion.yml", templatePath);
        CopyFileToDirectory("../Initium.Portal.sln", templatePath);

        ZipCompress(templatePath, File("template.zip"));

        DeleteDirectory(templatePath, true);
    });
Task("__Build")
    .Does(() => {
        CakeExecuteScript("./build.cake");
    });
// RUN build
// COMPRESS COPIED AND USE AS RELEASE
Task("__TemplatePackage")
    .Does(() => {
        CopyFileToDirectory("template.zip", releasePath);
              
    });

Task("Tempate-Build")
    .IsDependentOn("__TemplateCopy")
    .IsDependentOn("__Build")
    .IsDependentOn("__TemplatePackage")

    ;

Task("Default")
    .IsDependentOn("Tempate-Build");

RunTarget(target);