# Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
COPY pub/ /root/
WORKDIR /root/

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "ApiHost.Core.dll"]
