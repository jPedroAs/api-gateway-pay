FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src
COPY ["banco/banco.csproj", "banco/"]
RUN dotnet restore "banco/banco.csproj"

COPY . . 
WORKDIR /src/banco
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build


FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src/banco
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
ENV ConnectionStrings__DefaultConnection="Host=dpg-cuj7ektumphs738bgfs0-a;Port=5432;Database=banco_gh05;Username=banco_gh05_user;Password=M99HADj2qIBjpePTPV9O3mNhYSFvYGSJ;SSL Mode=Require;Trust Server Certificate=True"
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "banco.dll"]
