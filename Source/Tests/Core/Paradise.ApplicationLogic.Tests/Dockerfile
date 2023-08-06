FROM mcr.microsoft.com/dotnet/sdk:7.0

WORKDIR /src
COPY ["Source/Tests/Core/Paradise.ApplicationLogic.Tests/Paradise.ApplicationLogic.Tests.csproj", "Source/Tests/Core/Paradise.ApplicationLogic.Tests/"]

RUN dotnet restore "Source/Tests/Core/Paradise.ApplicationLogic.Tests/Paradise.ApplicationLogic.Tests.csproj"
COPY . .

WORKDIR "/src/Source/Tests/Core/Paradise.ApplicationLogic.Tests"
RUN dotnet build "Paradise.ApplicationLogic.Tests.csproj" -c Release -o /app/build

ENTRYPOINT ["dotnet", "test", "Paradise.ApplicationLogic.Tests.csproj", "--no-build", "-v:quiet"]