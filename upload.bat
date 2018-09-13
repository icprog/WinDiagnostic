@echo off

ping -n 1 192.168.100.94 | find "ms" > nul	rem check server is running

if %errorlevel% == 0 (goto pingok) else (goto pingfail)

:pingok
if not exist x:\ (
net use x: \\192.168.100.94\testdb /user:ubuntu password	rem map network drive x:
)

if %errorlevel% == 0 (goto success) else (goto fail)

:pingfail
goto fail

:fail
echo Ping 192.168.100.94 failed
goto done

:success
rmdir /s /q testdb
mkdir testdb

rem copy all files to its testdb/Lib/%%i path
for /f "delims=" %%i in ('"dir/a:d/b"') do (
mkdir testdb\Lib\%%i
copy %%i\%%i\bin\Release\*.exe  testdb\Lib\%%i
copy %%i\%%i\bin\Release\*.dll  testdb\Lib\%%i
copy %%i\%%i\bin\Release\*.wav  testdb\Lib\%%i
echo.start %%i.exe>testdb\Lib\%%i\run.bat	rem create run.bat
)

rmdir /s /q testdb\Lib\.git
rmdir /s /q testdb\Lib\sample
rmdir /s /q testdb\Lib\Main
rmdir /s /q testdb\Lib\testdb
rmdir /s /q testdb\Lib\SerialNumber
rmdir /s /q testdb\Lib\packages
rmdir /s /q testdb\Lib\test_sample

rem compress all files to zip
for /f "delims=" %%i in ('"dir/a:d/b testdb\Lib"') do (
	7za a -tzip %cd%\testdb\Lib\%%i\%%itest.zip %cd%\testdb\Lib\%%i\*
)

rem delete not necessary files
for /r %cd%\testdb\Lib %%i in (*) do (
	if not "%%~xi" == ".zip" (DEL /F /A /Q %%i)
)

rem copy .json files
for /f "delims=" %%i in ('"dir/a:d/b sample"') do (
mkdir testdb\Lib\%%i
copy sample\%%i\*.json  testdb\Lib\%%i
)

rem merge wifi and lan to network folder
mkdir testdb\Lib\network
xcopy testdb\Lib\wifi testdb\Lib\network\wifi /Y /I
xcopy testdb\Lib\lan testdb\Lib\network\lan /Y /I
rmdir /s /q testdb\Lib\wifi
rmdir /s /q testdb\Lib\lan

rem copy all files to server
for /f "delims=" %%i in ('"dir/a:d/b testdb\Lib"') do (
	xcopy testdb\Lib\%%i x:\Lib\%%i /Y /I /E
)
goto done

:done
