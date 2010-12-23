@echo off
goto :START
-------------------------------------------------------

  gac.cmd

  Install Microsoft.Http.dll and Microsoft.Http.Extensions.dll
  into the gac.


-------------------------------------------------------
:START
@setlocal

set pluginDir="C:\Program Files (x86)\Fusion8Design\Cropper\plugins"

call  :GAC   Microsoft.Http             %pluginDir%
call  :GAC   Microsoft.Http.Extensions  %pluginDir%

goto ALL_DONE


-------------------------------------------------------
:GAC
    echo.
    echo \winsdk\bin\gacutil -i  "%~2\%~1.dll"
    \winsdk\bin\gacutil -i  "%~2\%~1.dll"
    goto :EOF


-------------------------------------------------------
:ALL_DONE

@endlocal
