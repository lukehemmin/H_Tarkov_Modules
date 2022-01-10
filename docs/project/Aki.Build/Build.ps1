param ([switch]$VSBuilt)

#copy and verify file function. Destination path should be a directory.
function CopyAndVerifyFile 
{
    param
    (
        [System.IO.FileInfo]$File,
        [string]$DestinationPath,
        $OverrideFileName = $null
    )

    $friendlyName = "$($file.Directory.Parent.Parent.Name) - $($File.Name)"

    Write-Host "Copying $($friendlyName) " -NoNewLine
    
    #check paths
    if(-not(Test-Path $File.FullName)) 
    { 
        Write-Host "Can't find file path: `n$($File.FullName)" -ForegroundColor Red
        return
    }
    if(-not(Test-Path $DestinationPath)) 
    {
        Write-Host "Can't find destination path: `n$($DestinationPath)" -ForegroundColor Red
        return
    }

    if($OverrideFileName -ne $null) 
    {
        $DestinationPath = "$($DestinationPath)\$($OverrideFileName)"
        Write-Host ": New Name -> $($OverrideFileName)" -NoNewLine -ForegroundColor Magenta
    }

    Write-Host " ... " -NoNewLine
    
    Copy-Item -Path $File.FullName -Destination $DestinationPath -Force -ErrorAction SilentlyContinue

    #make sure the file was copied - I'm going to work on cleaning this script up, so this is just to get things going. It needs some TLC.
    if($OverrideFileName -ne $null) 
    {
      if(Test-Path $DestinationPath)
      {
        Write-Host "OK" -ForegroundColor Green
        return
      }
    }
    else 
    {
      if(Test-Path "$($DestinationPath)\$($File.Name)") 
      {
          Write-Host "OK" -ForegroundColor Green
          return
      }
    }

    Write-Host "Something went wrong :( `nError: $($Error[0])" -ForegroundColor Red
}

if(-not $VSBuilt)
{
    # locate msbuild
    Write-Host "Scanning for build tools..."

    $vsWhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"

    if (($vsWhere -eq("")) -or(-not(Test-Path $vsWhere)))
    {
        Write-Warning "  Could not find VSWhere.exe, please install BuildTools 2017 or newer"
        return
    }

    $msbuild = & $vsWhere -latest -products * -requires Microsoft.Component.MSBuild -find MSBuild\**\Bin\MSBuild.exe

    if (($msbuild -eq("")) -or(-Not(Test-Path $msbuild)))
    {
        # make sure msbuild ins't empty and that the path exists, otherwise warn and exit.
        Write-Warning "  Could not find Microsoft Buildtools"
        return
    }

    Write-Host "  Found MSBuild.exe" -ForegroundColor Green
    Write-Host ""

    # restore nuget packages and build project
    Write-Host "Building Modules..." -ForegroundColor Cyan
    $buildProcess = Start-Process -FilePath $msbuild -NoNewWindow -ArgumentList "-nologo /verbosity:minimal -consoleloggerparameters:Summary -t:Restore;Rebuild -p:Configuration=Release Modules.sln" -PassThru
    Wait-Process -InputObject $buildProcess
    Write-Host "Done" -ForegroundColor Cyan
    Write-Host ""
}
else
{
    Write-Host "VSBuilt: Skipping build" -ForegroundColor Cyan
}


if($VSBuilt) 
{
    #move up a directory if the script is run from VS (it runs from the PostBuildProject dir)
    cd ../../..
}

#get root of project folder
$rootPath = Resolve-Path -path "."

#path to build directory
$buildDir = "$($rootPath)\Build"

#path to managed data directory
$managedFolder = "$($buildDir)\EscapeFromTarkov_Data\Managed"
$akiModulesFolder = "$($buildDir)\Aki_Data\Modules"

#remove build directory if it exists.
if(Test-Path $buildDir) 
{
    Remove-Item $buildDir -Recurse -Force
}

$postBuildBin = "$($rootPath)\PostBuildProject\bin"
$postBuildObj = "$($rootPath)\PostBuildProject\obj"

if(Test-Path $postBuildBin)
{
    Remove-Item $postBuildBin -Recurse -Force
    Write-Host "Removed PostBuildProject bin" -ForegroundColor Cyan
}

if(Test-Path $postBuildObj)
{
    Remove-Item $postBuildObj -Recurse -Force
    Write-Host "Removed PostBuildProject obj" -ForegroundColor Cyan
}

#get all release dlls and exe files
$dllAndExeFiles = Resolve-Path -Path "*\bin\release\" | % {Get-ChildItem -Path $_} | where {$_.Name -like "*.dll" -or $_.Name -like "*.exe"}

#create the build directory structure
[System.IO.Directory]::CreateDirectory($managedFolder) | Out-Null

#copy and verify build files
Write-Host ""
foreach($file in $dllAndExeFiles) 
{
    if($file.Name.StartsWith("aki-")) 
    {
        $akiNoExtension = $file.Name.Replace(".dll","")
        $akiModuleFilePath = "$($akiModulesFolder)\$($akiNoExtension)"
        [System.IO.Directory]::CreateDirectory($akiModuleFilePath) | Out-Null
        CopyAndVerifyFile $file $akiModuleFilePath "module.dll"
    }
    else 
    {
        CopyAndVerifyFile $file $managedFolder
    }
}

Write-Host ""

# delete build waste
if(-not $VSBuilt)
{
    Write-Host "Cleaning garbage produced by build..." -ForegroundColor Cyan

    $delPaths = Get-ChildItem -Recurse -Path $rootPath | where {$_.FullName -like "*\bin"} | select -ExpandProperty FullName
    $delPaths += Get-ChildItem -Recurse -Path $rootPath | where {$_.FullName -like "*\obj"} | select -ExpandProperty FullName

    foreach ($path in $delPaths)
    {
        Write-Host "  Delete: $($path)"
        Remove-Item $path -Force -Recurse
    }
}

Write-Host ""
Write-Host "Done building" -ForegroundColor Cyan