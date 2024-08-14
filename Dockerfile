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

FROM johnzaza/csb-retention:3.3.44 AS currentversion

FROM base AS final
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8042;8080;4200
COPY --from=publish /app/publish .
COPY --from=currentversion /usr/share/nginx/html /app/wwwroot

USER root
RUN chown -R app:app /app/wwwroot
USER app
ENTRYPOINT ["dotnet", "CSBDashboardServer.dll"]