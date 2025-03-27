# Gunakan base image .NET SDK untuk build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj dan restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy seluruh project dan build
COPY . ./
RUN dotnet publish -c Release --no-restore -o /out

# Gunakan base image runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out ./

EXPOSE 8080
# Jalankan aplikasi di port 8080
ENTRYPOINT ["dotnet", "BackendTrasgo.dll"]