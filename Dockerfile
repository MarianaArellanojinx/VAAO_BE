# =========================
# Build stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar solución y restaurar dependencias
COPY VAAO_BE.sln .
COPY VAAO_BE/VAAO_BE.csproj VAAO_BE/
RUN dotnet restore

# Copiar todo y compilar
COPY . .
WORKDIR /src/VAAO_BE
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# =========================
# Runtime stage
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# DigitalOcean usa el puerto 8080
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "VAAO_BE.dll"]
