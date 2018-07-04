mkdir Windiagnostic
for /f "delims=" %%i in ('"dir/a:d/b"') do (
mkdir Windiagnostic\%%i
copy %%i\%%i\bin\Release\%%i.exe  Windiagnostic\%%i
copy %%i\%%i\bin\Release\*.dll  Windiagnostic\%%i
copy %%i\%%i\bin\Release\*.wav  Windiagnostic\%%i
)
rmdir Windiagnostic\.git
rmdir Windiagnostic\sample
rmdir Windiagnostic\Main
rmdir Windiagnostic\Windiagnostic
copy Main\TestTool\bin\Release\Windiagnostic.exe Windiagnostic\
copy Main\TestTool\bin\Release\*.dll Windiagnostic\
copy sample\*.json Windiagnostic\
for /f "delims=" %%i in ('"dir/a:d/b sample"') do (
mkdir Windiagnostic\%%i
copy sample\%%i\*.json  Windiagnostic\%%i
)
