#!/bin/bash

# Sprint Analytics Dashboard - Start Script
echo "================================"
echo "Sprint Analytics Dashboard"
echo "================================"
echo ""

# Check if Docker is installed
if ! command -v sudo docker &> /dev/null; then
    echo "❌ Docker não está instalado!"
    echo ""
    echo "Por favor, instale o Docker:"
    echo "  - Windows/Mac: https://www.docker.com/products/docker-desktop"
    echo "  - Linux: sudo apt-get install docker.io docker-compose"
    exit 1
fi

# Check if docker-compose is available
if command -v sudo docker-compose &> /dev/null; then
    COMPOSE_CMD="sudo docker-compose"
elif sudo docker compose version &> /dev/null; then
    COMPOSE_CMD="sudo docker compose"
else
    echo "❌ docker-compose não está disponível!"
    exit 1
fi

echo "✅ Docker encontrado"
echo ""

# Stop any existing containers
echo "🛑 Parando containers existentes..."
$COMPOSE_CMD down 2>/dev/null

# Build and start
echo "🔨 Construindo imagem Docker..."
$COMPOSE_CMD build

echo ""
echo "🚀 Iniciando dashboard..."
$COMPOSE_CMD up -d

# Wait for service to be ready
echo ""
echo "⏳ Aguardando serviço iniciar..."
sleep 5

# Check if service is running
if $COMPOSE_CMD ps | grep -q "Up"; then
    echo ""
    echo "================================"
    echo "✅ Dashboard iniciado com sucesso!"
    echo "================================"
    echo ""
    echo "📊 Acesse o dashboard em:"
    echo "   http://localhost:3000"
    echo ""
    echo "📁 Para adicionar novos snapshots:"
    echo "   1. Coloque arquivos JSON na pasta ./data/"
    echo "   2. Atualize ./data/snapshots.json"
    echo ""
    echo "📋 Comandos úteis:"
    echo "   Ver logs:     $COMPOSE_CMD logs -f"
    echo "   Parar:        $COMPOSE_CMD down"
    echo "   Reiniciar:    $COMPOSE_CMD restart"
    echo ""
else
    echo ""
    echo "❌ Erro ao iniciar o dashboard!"
    echo "Verifique os logs com: $COMPOSE_CMD logs"
    exit 1
fi
