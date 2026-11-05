# Zobacz https://aka.ms/customizecontainer, aby dowiedzieć się, jak dostosować kontener debugowania i jak program Visual Studio używa tego pliku Dockerfile do kompilowania obrazów w celu szybszego debugowania.

# Ten etap jest używany podczas uruchamiania z programu VS w trybie szybkim (domyślnie dla konfiguracji debugowania)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Ten etap służy do kompilowania projektu usługi
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ProductMessengerRabbitMQ.API/ProductMessengerRabbitMQ.API.csproj", "ProductMessengerRabbitMQ.API/"]
COPY ["ProductMessengerRabbitMQ.Contracts/ProductMessengerRabbitMQ.Contracts.csproj", "ProductMessengerRabbitMQ.Contracts/"]
RUN dotnet restore "./ProductMessengerRabbitMQ.API/ProductMessengerRabbitMQ.API.csproj"
COPY . .
WORKDIR "/src/ProductMessengerRabbitMQ.API"
RUN dotnet build "./ProductMessengerRabbitMQ.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Ten etap służy do publikowania projektu usługi do skopiowania do etapu końcowego
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ProductMessengerRabbitMQ.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Ten etap jest używany w środowisku produkcyjnym lub w przypadku uruchamiania z programu VS w trybie regularnym (domyślnie, gdy nie jest używana konfiguracja debugowania)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductMessengerRabbitMQ.API.dll"]