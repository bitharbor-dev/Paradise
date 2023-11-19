FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /src
COPY ["Source/Tests/Core/Paradise.DataAccess.Seed.Tests/Paradise.DataAccess.Seed.Tests.csproj", "Source/Tests/Core/Paradise.DataAccess.Seed.Tests/"]

RUN dotnet restore "Source/Tests/Core/Paradise.DataAccess.Seed.Tests/Paradise.DataAccess.Seed.Tests.csproj"
COPY . .

WORKDIR "/src/Source/Tests/Core/Paradise.DataAccess.Seed.Tests"
RUN dotnet build "Paradise.DataAccess.Seed.Tests.csproj" -c Release -o /app/build

ENTRYPOINT ["dotnet", "test", "Paradise.DataAccess.Seed.Tests.csproj", "--no-build", "-v:quiet"]