#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/synosscamera.api/synosscamera.api.csproj", "src/synosscamera.api/"]
COPY ["src/synosscamera.core/synosscamera.core.csproj", "src/synosscamera.core/"]
COPY ["src/synosscamera.station/synosscamera.station.csproj", "src/synosscamera.station/"]
COPY ["src/synosscamera.redis/synosscamera.redis.csproj", "src/synosscamera.redis/"]
COPY ["src/synosscamera.prometheus/synosscamera.prometheus.csproj", "src/synosscamera.prometheus/"]
RUN dotnet restore "src/synosscamera.api/synosscamera.api.csproj"
COPY . .
WORKDIR "/src/src/synosscamera.api"
RUN dotnet build "synosscamera.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "synosscamera.api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "synosscamera.api.dll"]
