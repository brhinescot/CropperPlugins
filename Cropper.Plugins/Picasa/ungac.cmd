@echo off
goto :START
-------------------------------------------------------

  ungac.cmd

  Remove Microsoft.Http.dll and Microsoft.Http.Extensions.dll
  from the gac.


-------------------------------------------------------
:START
@setlocal

set assemblyInfo=", Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"

call :UNGAC Microsoft.Http %assemblyInfo%
call :UNGAC Microsoft.Http.Extensions %assemblyInfo%

goto ALL_DONE


-------------------------------------------------------
:UNGAC
    echo.
    echo \winsdk\bin\gacutil -u "%~1%~2"
    \winsdk\bin\gacutil -u "%~1%~2"
    goto :EOF


-------------------------------------------------------
:ALL_DONE

@endlocal

