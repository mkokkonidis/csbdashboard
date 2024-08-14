FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
RUN mkdir /app
EXPOSE 8042

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
USER app
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CSBDashboardServer.csproj", "."]
RUN dotnet restore "./CSBDashboardServer.csproj"
COPY . .
RUN dotnet build "./CSBDashboardServer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
USER app
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CSBDashboardServer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM johnzaza/csb-retention:3.3.41 AS currentversion

FROM base AS final
USER app
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8042;8080;4200
COPY --chown=app:app --from=publish /app/publish .
COPY --chown=app:app --from=currentversion /usr/share/nginx/html /app/wwwroot
USER app
ENTRYPOINT ["dotnet", "CSBDashboardServer.dll"]