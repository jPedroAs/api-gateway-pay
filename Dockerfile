# Etapa Base (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Etapa de Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src
COPY ["banco/banco.csproj", "banco/"]
RUN dotnet restore "banco/banco.csproj"

COPY . .
WORKDIR /src/banco
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

# Etapa de Publicação
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src/banco
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Etapa Final (Runtime)
FROM base AS final
WORKDIR /app

# Copia a aplicação publicada
COPY --from=publish /app/publish .

# Instala dotnet-ef na pasta local (não global)
RUN dotnet tool install --tool-path /app/tools dotnet-ef --version 8.0

# Script de inicialização para rodar as migrações antes de iniciar o app
COPY entrypoint.sh /app/entrypoint.sh
RUN chmod +x /app/entrypoint.sh

# Define o ponto de entrada
ENTRYPOINT ["/app/entrypoint.sh"]
