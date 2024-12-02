FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
RUN pwd; ls -laR; chmod -R 777 .
WORKDIR /app/build
RUN pwd; ls -laR; chmod -R 777 .
WORKDIR /app/publish; chmod 777 .
RUN pwd; ls -laR; chmod -R 777 .
WORKDIR /app
RUN pwd; ls -laR; chmod -R 777 .
EXPOSE 8042

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
USER app
WORKDIR /app
RUN pwd; ls -laR; chmod -R 777 .
WORKDIR /app/build
RUN pwd; ls -laR; chmod -R 777 .
WORKDIR /app/publish; chmod 777 .
RUN pwd; ls -laR; chmod -R 777 .
WORKDIR /app
RUN pwd; ls -laR; chmod -R 777 .
WORKDIR /src
COPY  --chown=app:app ["CSBDashboardServer.csproj", "."]
RUN dotnet restore "./CSBDashboardServer.csproj"
COPY  --chown=app:app . .
RUN chmod -R 777 /src /app && ls -lR /src /app || echo somthing failed
RUN dotnet build "./CSBDashboardServer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CSBDashboardServer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM johnzaza/csb-retention:4.0.16 AS currentversion
WORKDIR /usr/share/nginx/html
RUN sed -i 's|"https://test-retention.biomed.ntua.gr/|"https://" + location.hostname + "/|g' assets/env.js
RUN pwd; ls -laR || echo Failed to ls; chmod -R 777 . || echo Failed to chmod ; chown -R app:app . || echo Did not manage to chown


FROM base AS final
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8042;8080;4200
COPY   --chown=app:app --from=publish /app/publish .
COPY   --chown=app:app --from=currentversion /usr/share/nginx/html /app/wwwroot
RUN sed -i 's|"https://test-retention.biomed.ntua.gr/|"https://" + location.hostname + "/|g' wwwroot/assets/env.js
ENTRYPOINT ["dotnet", "CSBDashboardServer.dll"]
