@echo off
goto START

-------------------------------------------------------
 makeRelease.cmd



 Created Fri Dec 17 12:09:37 2010

-------------------------------------------------------

:START
SETLOCAL
if  _%1==_ goto USAGE

set relVersion=%1
set plat=x86
shift
if _%1==_ goto MAIN
set plat=%1
goto MAIN


-------------------------------------------------------
:MAIN

set zipit=c:\users\dino\bin\zipit.exe
if not %plat% == x86 goto PLATFORM_NO

powershell.exe .\SetVersion.ps1  %relVersion%

set releaseZip=CropperPlugins-v%relVersion%.%plat%.zip

if exist %releaseZip% goto ALREADY_EXISTS

:BUILD
msbuild /p:Platform=x86 /p:Configuration=Release /t:Clean
msbuild /p:Platform=x86 /p:Configuration=Release

:ZIPIT

%zipit% %releaseZip% -d x86 -D output\x86\Release "(name != Cropper.Core.dll) and (name != Cropper.Extensibility.dll)"
%zipit% %releaseZip% -d x86 -D "\Program Files (x86)\Microsoft WCF REST\WCF REST Starter Kit Preview 2\Assemblies" Microsoft.Http*.dll
%zipit% %releaseZip% -d x86 -D Dependencies Flickrnet.dll


unzip -l %releaseZip%

echo.
echo your release zip is    %releaseZip%

goto ALL_DONE

-------------------------------------------------------


--------------------------------------------
:USAGE
  echo usage:   makeRelease ^<number-of-release^> [x86^|x64]
  echo   make a release of cropper-plugins
  goto ALL_DONE
--------------------------------------------
--------------------------------------------
:ALREADY_EXISTS
  echo.
  echo Error! that release zip (%releaseZip%) already exists!
  echo.
  goto USAGE
--------------------------------------------
--------------------------------------------
:PLATFORM_NO
  echo.
  echo Error! that platform isn't supported.  Use x86 only.
  echo.
  goto USAGE

--------------------------------------------


:ALL_DONE
ENDLOCAL
