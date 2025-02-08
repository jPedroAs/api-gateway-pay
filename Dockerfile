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



RUN dotnet tool install --global dotnet-ef --version 8.0.0
ENV PATH="${PATH}:/root/.dotnet/tools"

# Etapa de Publicação
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src/banco
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


FROM build AS final-sdk
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT "dotnet ef database update

# Etapa Final (Runtime)
FROM base AS final
WORKDIR /app

# Copia a aplicação publicada
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "banco.dll"]
