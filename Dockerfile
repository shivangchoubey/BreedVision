# Base Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080
ENV DatabaseProvider=Sqlite

# Build & Publish Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/BreedVision.Web/BreedVision.Web.csproj", "src/BreedVision.Web/"]
RUN dotnet restore "src/BreedVision.Web/BreedVision.Web.csproj"
COPY . .
WORKDIR "/src/src/BreedVision.Web"
RUN dotnet publish "BreedVision.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final Production Container
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Ensure Data & Uploads directories exist
RUN mkdir -p /app/Data /app/wwwroot/uploads

ENTRYPOINT ["dotnet", "BreedVision.Web.dll"]

