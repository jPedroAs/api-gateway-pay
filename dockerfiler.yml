# Usa a imagem base do .NET
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 8080

# Copia os arquivos do projeto e publica
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["banco.csproj", "./"]
RUN dotnet restore "banco.csproj"
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Configura o container final
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "banco.dll"]
