FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["consumer/WorkerConsumer/WorkerCreateUserConsumer.csproj", "consumer/WorkerConsumer/"]
RUN dotnet restore "consumer/WorkerConsumer/WorkerCreateUserConsumer.csproj"

COPY . .
WORKDIR "/src/consumer/WorkerConsumer"
RUN dotnet build "WorkerCreateUserConsumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WorkerCreateUserConsumer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WorkerCreateUserConsumer.dll"]