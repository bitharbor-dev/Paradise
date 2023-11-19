FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /src
COPY ["Source/Tests/Core/Paradise.Domain.Base.Tests/Paradise.Domain.Base.Tests.csproj", "Source/Tests/Core/Paradise.Domain.Base.Tests/"]

RUN dotnet restore "Source/Tests/Core/Paradise.Domain.Base.Tests/Paradise.Domain.Base.Tests.csproj"
COPY . .

WORKDIR "/src/Source/Tests/Core/Paradise.Domain.Base.Tests"
RUN dotnet build "Paradise.Domain.Base.Tests.csproj" -c Release -o /app/build

ENTRYPOINT ["dotnet", "test", "Paradise.Domain.Base.Tests.csproj", "--no-build", "-v:quiet"]