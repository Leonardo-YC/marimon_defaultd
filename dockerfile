# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

# Copiar csproj y restaurar dependencias
COPY *.csproj ./
RUN dotnet restore

# Copiar el resto del código y compilar
COPY . ./
RUN dotnet publish -c Release -o out

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copiar los archivos publicados desde la etapa de build
COPY --from=build-env /app/out .

# Exponer puertos
EXPOSE 80
EXPOSE 443

# Comando para arrancar la app
ENTRYPOINT ["dotnet", "marimon_defaultd.dll"]
