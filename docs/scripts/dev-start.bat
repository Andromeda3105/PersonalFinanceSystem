@echo off
chcp 65001 > nul
echo Запуск Personal Finance System (Режим розробника)...

cd ..\..\src\PersonalFinanceSystem.API

echo Відновлення залежностей...
dotnet restore

echo Запуск сервера з Hot Reload...
dotnet watch run

pause