# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY backend/ResumeMatcher.Api/*.csproj backend/ResumeMatcher.Api/
RUN dotnet restore backend/ResumeMatcher.Api/*.csproj

COPY . .
RUN dotnet publish backend/ResumeMatcher.Api/*.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ResumeMatcher.Api.dll"]