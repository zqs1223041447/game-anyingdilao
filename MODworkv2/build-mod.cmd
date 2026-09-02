@echo off
setlocal
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0build-mod.ps1" %*
exit /b %errorlevel%
