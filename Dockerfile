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

ENV ConnectionStrings__DefaultConnection="Host=dpg-cuj7ektumphs738bgfs0-a;Database=banco_gh05;Username=banco_gh05_user;Password=M99HADj2qIBjpePTPV9O3mNhYSFvYGSJ;"

RUN dotnet tool install --global dotnet-ef --version 8.0.0
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet ef database update  # Executa a migração na fase de build

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

# Script de inicialização
COPY entrypoint.sh /app/entrypoint.sh
RUN chmod +x /app/entrypoint.sh

ENTRYPOINT ["/app/entrypoint.sh"]
