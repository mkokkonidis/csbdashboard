FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
RUN pwd; ls -laR
WORKDIR /app/build
RUN pwd; ls -laR
WORKDIR /app/publish
RUN pwd; ls -laR
WORKDIR /app
RUN pwd; ls -laR
EXPOSE 8042

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
USER app
WORKDIR /src
COPY  --chown=app:app ["CSBDashboardServer.csproj", "."]
RUN dotnet restore "./CSBDashboardServer.csproj"
COPY  --chown=app:app . .
RUN ls -lR 
RUN dotnet build "./CSBDashboardServer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CSBDashboardServer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM johnzaza/csb-retention:3.3.44 AS currentversion

FROM base AS final
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8042;8080;4200
COPY   --chown=app:app --from=publish /app/publish .
COPY   --chown=app:app --from=currentversion /usr/share/nginx/html /app/wwwroot
ENTRYPOINT ["dotnet", "CSBDashboardServer.dll"]