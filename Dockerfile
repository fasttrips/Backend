# Gunakan official .NET SDK 8.0 sebagai build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set working directory
WORKDIR /app

# Salin .csproj dan restore dependensi
COPY *.csproj ./
RUN dotnet restore

# Salin semua kode sumber dan build aplikasi
COPY . ./
RUN dotnet publish -c Release -o /app/out --no-restore

# Gunakan image runtime .NET 8.0 yang lebih kecil
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set working directory
WORKDIR /app

# Salin hasil build dari stage sebelumnya
COPY --from=build /app/out .

# Tentukan variabel lingkungan agar .NET berjalan di port 8080
ENV ASPNETCORE_URLS=http://+:8080

# Ekspose port yang digunakan aplikasi
EXPOSE 8080

# Jalankan aplikasi
ENTRYPOINT ["dotnet", "BackendTrasgo.dll"]
