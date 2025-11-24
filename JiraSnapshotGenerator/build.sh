#!/bin/bash

echo "========================================"
echo "  Jira Snapshot Generator - Build"
echo "========================================"
echo

cd JiraSnapshotGenerator

echo "[1/3] Restaurando dependências..."
dotnet restore
if [ $? -ne 0 ]; then
    echo "ERRO: Falha ao restaurar dependências"
    exit 1
fi

echo
echo "[2/3] Compilando projeto..."
dotnet build --configuration Release
if [ $? -ne 0 ]; then
    echo "ERRO: Falha ao compilar"
    exit 1
fi

echo
echo "[3/3] Testando execução..."
dotnet run --no-build --configuration Release -- --version 2>/dev/null

echo
echo "========================================"
echo "  Build concluído com sucesso!"
echo "========================================"
echo
echo "Para executar:"
echo "  ./run.sh"
echo
echo "Ou manualmente:"
echo "  cd JiraSnapshotGenerator"
echo "  dotnet run"
echo
