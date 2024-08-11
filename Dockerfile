#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8042

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CSBDashboardServer.csproj", "."]
RUN dotnet restore "./CSBDashboardServer.csproj"
COPY . .
RUN dotnet build "./CSBDashboardServer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CSBDashboardServer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM johnzaza/csb-retention:3.3.40 AS currentversion

FROM base AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://localhost:8042/
COPY --from=publish /app/publish .
COPY --from=currentversion /usr/share/nginx/html /wwwroot
ENTRYPOINT ["dotnet", "CSBDashboardServer.dll"]