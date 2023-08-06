FROM mcr.microsoft.com/dotnet/sdk:7.0

WORKDIR /src
COPY ["Source/Tests/Core/Paradise.DataAccess.Tests/Paradise.DataAccess.Tests.csproj", "Source/Tests/Core/Paradise.DataAccess.Tests/"]

RUN dotnet restore "Source/Tests/Core/Paradise.DataAccess.Tests/Paradise.DataAccess.Tests.csproj"
COPY . .

WORKDIR "/src/Source/Tests/Core/Paradise.DataAccess.Tests"
RUN dotnet build "Paradise.DataAccess.Tests.csproj" -c Release -o /app/build

ENTRYPOINT ["dotnet", "test", "Paradise.DataAccess.Tests.csproj", "--no-build", "-v:quiet"]