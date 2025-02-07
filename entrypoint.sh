#!/bin/sh
set -e
echo "Executando migrações..."
dotnet ef database update
echo "Iniciando aplicação..."
exec dotnet banco.dll
