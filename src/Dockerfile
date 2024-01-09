FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/CreateUser.API/CreateUser.API.csproj", "src/CreateUser.API/"]
COPY ["src/CreateUser.Core/CreateUser.Core.csproj", "src/CreateUser.Core/"]
COPY ["src/CreateUser.Infrastructure/CreateUser.Infrastructure.csproj", "src/CreateUser.Infrastructure/"]
RUN dotnet restore "src/CreateUser.API/CreateUser.API.csproj"

COPY . .
WORKDIR "/src/src/CreateUser.API"
RUN dotnet build "CreateUser.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CreateUser.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CreateUser.API.dll"]