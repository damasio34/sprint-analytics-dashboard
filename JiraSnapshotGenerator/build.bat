@echo off
echo ========================================
echo   Jira Snapshot Generator - Build
echo ========================================
echo.

cd JiraSnapshotGenerator

echo [1/3] Restaurando dependencias...
dotnet restore
if %ERRORLEVEL% NEQ 0 (
    echo ERRO: Falha ao restaurar dependencias
    pause
    exit /b 1
)

echo.
echo [2/3] Compilando projeto...
dotnet build --configuration Release
if %ERRORLEVEL% NEQ 0 (
    echo ERRO: Falha ao compilar
    pause
    exit /b 1
)

echo.
echo [3/3] Testando execucao...
dotnet run --no-build --configuration Release -- --version 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo AVISO: Nao foi possivel testar execucao
)

echo.
echo ========================================
echo   Build concluido com sucesso!
echo ========================================
echo.
echo Para executar:
echo   run.bat
echo.
echo Ou manualmente:
echo   cd JiraSnapshotGenerator
echo   dotnet run
echo.
pause
