# Usa dotnet como la imagen padre
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Se cambia al directorio /app dentro de la imagen
WORKDIR /App

# Copia todos los archivos a la imagen
COPY . ./

# Exponer el puerto 5000
EXPOSE 5000

# Construye la imagen
ENTRYPOINT ["dotnet", "ClosirisApiGateway.dll"]
