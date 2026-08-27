FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/Services/Finance.Service/Seadora.Finance.API/Seadora.Finance.API.csproj", "src/Services/Finance.Service/Seadora.Finance.API/"]
COPY ["src/Services/Finance.Service/Seadora.Finance.Application/Seadora.Finance.Application.csproj", "src/Services/Finance.Service/Seadora.Finance.Application/"]
COPY ["src/Services/Finance.Service/Seadora.Finance.Domain/Seadora.Finance.Domain.csproj", "src/Services/Finance.Service/Seadora.Finance.Domain/"]
COPY ["src/Services/Finance.Service/Seadora.Finance.Infrastructure/Seadora.Finance.Infrastructure.csproj", "src/Services/Finance.Service/Seadora.Finance.Infrastructure/"]
COPY ["src/Services/Common/Seadora.Common/Seadora.Common.csproj", "src/Services/Common/Seadora.Common/"]
COPY ["src/Services/Common/Seadora.Contracts/Seadora.Contracts.csproj", "src/Services/Common/Seadora.Contracts/"]
RUN dotnet restore "src/Services/Finance.Service/Seadora.Finance.API/Seadora.Finance.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Finance.Service/Seadora.Finance.API"
RUN dotnet build "Seadora.Finance.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Seadora.Finance.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Seadora.Finance.API.dll"]
