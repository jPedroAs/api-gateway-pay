#!/bin/sh
set -e

# Executa as migrações antes de iniciar a aplicação
/app/tools/dotnet-ef database update

# Inicia a aplicação
exec dotnet banco.dll
