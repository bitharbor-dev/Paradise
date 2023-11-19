FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /src
COPY ["Source/Tests/Core/Paradise.ApplicationLogic.Domain.Tests/Paradise.ApplicationLogic.Domain.Tests.csproj", "Source/Tests/Core/Paradise.ApplicationLogic.Domain.Tests/"]

RUN dotnet restore "Source/Tests/Core/Paradise.ApplicationLogic.Domain.Tests/Paradise.ApplicationLogic.Domain.Tests.csproj"
COPY . .

WORKDIR "/src/Source/Tests/Core/Paradise.ApplicationLogic.Domain.Tests"
RUN dotnet build "Paradise.ApplicationLogic.Domain.Tests.csproj" -c Release -o /app/build

ENTRYPOINT ["dotnet", "test", "Paradise.ApplicationLogic.Domain.Tests.csproj", "--no-build", "-v:quiet"]