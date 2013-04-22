@echo off

set DIR=TacSelfDestruct_v%1

mkdir Release\%DIR%

xcopy /s /f /y Parts Release\%DIR%\Parts\
xcopy /s /f /y Plugins Release\%DIR%\Plugins\

cd Release
7z a -tzip %DIR%.zip %DIR%
cd ..