mkdir Windiagnostic

for /f "delims=" %%i in ('"dir/a:d/b"') do (
mkdir Windiagnostic\%%i
copy %%i\%%i\bin\Release\*.exe  Windiagnostic\%%i
copy %%i\%%i\bin\Release\*.dll  Windiagnostic\%%i
)

copy Main\TestTool\bin\Release\*.exe Windiagnostic\
copy Main\TestTool\bin\Release\*.dll Windiagnostic\
copy Windiagnostic\SerialNumber\*.exe Windiagnostic\
copy Windiagnostic\SerialNumber\*.dll Windiagnostic\

rmdir Windiagnostic\.git
rmdir Windiagnostic\sample
rmdir Windiagnostic\Main
rmdir Windiagnostic\Windiagnostic
rmdir Windiagnostic\SerialNumber

copy sample\*.json Windiagnostic\
#for /f "delims=" %%i in ('"dir/a:d/b sample"') do (
#mkdir Windiagnostic\%%i
#copy sample\%%i\*.json  Windiagnostic\%%i
#)
