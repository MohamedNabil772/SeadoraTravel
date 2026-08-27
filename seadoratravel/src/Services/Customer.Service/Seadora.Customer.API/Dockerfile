FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/Services/Customer.Service/Seadora.Customer.API/Seadora.Customer.API.csproj", "src/Services/Customer.Service/Seadora.Customer.API/"]
COPY ["src/Services/Customer.Service/Seadora.Customer.Application/Seadora.Customer.Application.csproj", "src/Services/Customer.Service/Seadora.Customer.Application/"]
COPY ["src/Services/Customer.Service/Seadora.Customer.Domain/Seadora.Customer.Domain.csproj", "src/Services/Customer.Service/Seadora.Customer.Domain/"]
COPY ["src/Services/Customer.Service/Seadora.Customer.Infrastructure/Seadora.Customer.Infrastructure.csproj", "src/Services/Customer.Service/Seadora.Customer.Infrastructure/"]
COPY ["src/Services/Common/Seadora.Common/Seadora.Common.csproj", "src/Services/Common/Seadora.Common/"]
COPY ["src/Services/Common/Seadora.Contracts/Seadora.Contracts.csproj", "src/Services/Common/Seadora.Contracts/"]
RUN dotnet restore "src/Services/Customer.Service/Seadora.Customer.API/Seadora.Customer.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Customer.Service/Seadora.Customer.API"
RUN dotnet build "Seadora.Customer.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Seadora.Customer.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Seadora.Customer.API.dll"]
