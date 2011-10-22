@echo off
goto START

-------------------------------------------------------
 makeRelease.cmd


 Created Fri Dec 17 12:09:37 2010

-------------------------------------------------------

:START
SETLOCAL
set zipit=\dev\codeplex\DotNetZip\tools\Zipit\bin\Debug\Zipit.exe
@REM set zipit=c:\users\dino\bin\zipit.exe

if  _%1==_ goto USAGE

set relVersion=%1
set plat=x86
shift
if _%1==_ goto MAIN

set plat=%1
if not %plat% == x86 goto WRONG_PLATFORM


:MAIN

set releaseZip=CropperPlugins-v%relVersion%.%plat%.zip
if exist releases\%releaseZip% goto:ALREADY_EXISTS

call :BUILD

call :ZIPIT

goto ALL_DONE





-------------------------------------------------------
:BUILD
  call :ECHO_RUN powershell.exe .\SetVersion.ps1  %relVersion%

  call :ECHO_RUN msbuild Cropper.Plugins.sln /p:Platform=x86 /p:Configuration=Release /t:Clean
  call :ECHO_RUN msbuild Cropper.Plugins.sln /p:Platform=x86 /p:Configuration=Release

goto :EOF
-------------------------------------------------------

-------------------------------------------------------
:ZIPIT
  %zipit% %releaseZip% -d x86 -D output\x86\Release -E "(name^!=Cropper.Core.dll) and (name^!=Cropper.Extensibility.dll)"

  %zipit% %releaseZip% -d x86 -D "\Program Files (x86)\Microsoft WCF REST\WCF REST Starter Kit Preview 2\Assemblies" -E Microsoft.Http*.dll

  %zipit% %releaseZip% -d x86 -D Dependencies -E Flickrnet.dll

  unzip -l %releaseZip%
  move %releaseZip% releases

  echo.
  echo your release zip is    releases\%releaseZip%
goto :EOF
-------------------------------------------------------

-------------------------------------------------------
:ECHO_RUN
    echo %*
    %*
goto :EOF
-------------------------------------------------------

-------------------------------------------------------
:USAGE
  echo usage:   makeRelease ^<number-of-release^> [x86^|x64]
  echo   make a release of cropper-plugins
  goto ALL_DONE
-------------------------------------------------------

-------------------------------------------------------
:ALREADY_EXISTS
  echo.
  echo Error: that release zip (%releaseZip%) already exists.
  echo.
  goto USAGE
-------------------------------------------------------

-------------------------------------------------------
:WRONG_PLATFORM
  echo.
  echo Error: that platform isn't supported.  Use x86 only.
  echo.
  goto USAGE
-------------------------------------------------------


:ALL_DONE
ENDLOCAL
