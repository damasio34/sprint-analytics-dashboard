@echo off
echo ================================
echo Sprint Analytics Dashboard
echo ================================
echo.

REM Check if Docker is installed
docker --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Docker nao esta instalado!
    echo.
    echo Por favor, instale o Docker Desktop:
    echo https://www.docker.com/products/docker-desktop
    pause
    exit /b 1
)

echo ✅ Docker encontrado
echo.

REM Stop any existing containers
echo 🛑 Parando containers existentes...
docker-compose down 2>nul

REM Build and start
echo 🔨 Construindo imagem Docker...
docker-compose build

echo.
echo 🚀 Iniciando dashboard...
docker-compose up -d

REM Wait for service to be ready
echo.
echo ⏳ Aguardando servico iniciar...
timeout /t 5 >nul

echo.
echo ================================
echo ✅ Dashboard iniciado com sucesso!
echo ================================
echo.
echo 📊 Acesse o dashboard em:
echo    http://localhost:3000
echo.
echo 📁 Para adicionar novos snapshots:
echo    1. Coloque arquivos JSON na pasta .\data\
echo    2. Atualize .\data\snapshots.json
echo.
echo 📋 Comandos uteis:
echo    Ver logs:     docker-compose logs -f
echo    Parar:        docker-compose down
echo    Reiniciar:    docker-compose restart
echo.
pause
