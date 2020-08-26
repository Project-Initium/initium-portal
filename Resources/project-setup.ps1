param (
    [Parameter(Mandatory=$true)][string]$companyName,
    [Parameter(Mandatory=$true)][string]$projectName,
    [string]$copyText = ''
)

Remove-item $release.assets[0].name
if ($copyText -eq '') { $copyText = 'Copyright (c) ' + $companyName + '. All rights reserved.' }

$stylecop = Get-Content .\Shared\Initium.Portal.Shared\stylecop.json  | Out-String | ConvertFrom-Json
$stylecop.settings.documentationRules.companyName = $companyName
$oldCopyText = '// ' + $stylecop.settings.documentationRules.copyrightText.replace("`n","`n// ");
$stylecop.settings.documentationRules.copyrightText = $copyText
$stylecop | ConvertTo-Json -depth 100 | Set-Content .\Shared\Initium.Portal.Shared\stylecop.json


Get-ChildItem -Filter 'Initium.Portal.*' -Recurse -Directory | Rename-Item -NewName { $_.name -replace 'Initium.Portal', $projectName }
Get-ChildItem -Filter 'Initium.Portal.*' -Recurse | Rename-Item -NewName { $_.name -replace 'Initium.Portal', $projectName }

$finalCopyText = "// " + $copyText.Replace("`n","`n// ")

$files = Get-ChildItem -Filter '*.*' -Exclude generate.ps1 -Recurse -File;
foreach ($file in $files){
    $content = Get-Content $($file.FullName) -Raw
    $content.Replace($oldCopyText, $finalCopyText).Replace('Initium.Portal', $projectName).TrimEnd() | Out-File $($file.FullName) -NoNewline
    
}

Push-Location .\Build
.\build.ps1 -Target '__Clean'
.\build.ps1 -Target '__RestorePackages'
.\build.ps1 -Target '__Build'
.\build.ps1 -Target '__Test'
.\build.ps1 -Target '__Publish'
Pop-Location
