FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CRUDServices/CRUDServices.csproj", "CRUDServices/"]
COPY ["CRUDServices/nuget.config", "CRUDServices/"]
RUN dotnet restore "CRUDServices/CRUDServices.csproj"
COPY . .
WORKDIR "/src/CRUDServices"
RUN dotnet build "CRUDServices.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CRUDServices.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CRUDServices.dll"]
