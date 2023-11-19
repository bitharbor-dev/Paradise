FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ["Source/Code/Web/Paradise.Maintenance/Paradise.Maintenance.csproj", "Source/Code/Web/Paradise.Maintenance/"]
RUN dotnet restore "Source/Code/Web/Paradise.Maintenance/Paradise.Maintenance.csproj"
COPY . .

WORKDIR "/src/Source/Code/Web/Paradise.Maintenance"
RUN dotnet build "Paradise.Maintenance.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Paradise.Maintenance.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Paradise.Maintenance.dll"]