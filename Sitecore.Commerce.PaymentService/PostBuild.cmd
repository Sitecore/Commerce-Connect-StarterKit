@Echo OFF
SETLOCAL
set CPrefix=%~d0%~p0

REM if NOT '%OfficialBuildMachine%' == '1' ()
pushd %CPrefix%

if '%OfficialBuildMachine%' == '1' (
  call powershell.exe -ExecutionPolicy Unrestricted -NOPROFILE -FILE ..\..\buildscript\SignSingle.ps1 -FileToSign %1
)

call XCOPY /S /Y "%2%3%1" "%2..\Website\bin\"
popd
ENDLOCAL
EXIT /B %ERRORLEVEL%