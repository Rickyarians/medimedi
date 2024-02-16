#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CollectionServices/CollectionServices.csproj", "CollectionServices/"]
COPY ["CollectionServices/appsettings.json", "CollectionServices/"]
COPY ["CollectionServices/appsettings.Development.json", "CollectionServices/"]
COPY ["CollectionServices/nuget.config", "CollectionServices/"]
RUN dotnet restore "CollectionServices/CollectionServices.csproj"
COPY . .
WORKDIR "/src/CollectionServices"
RUN dotnet build "CollectionServices.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CollectionServices.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CollectionServices.dll"]