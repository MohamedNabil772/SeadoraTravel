FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/Services/Support.Service/Seadora.Support.API/Seadora.Support.API.csproj", "src/Services/Support.Service/Seadora.Support.API/"]
COPY ["src/Services/Support.Service/Seadora.Support.Application/Seadora.Support.Application.csproj", "src/Services/Support.Service/Seadora.Support.Application/"]
COPY ["src/Services/Support.Service/Seadora.Support.Domain/Seadora.Support.Domain.csproj", "src/Services/Support.Service/Seadora.Support.Domain/"]
COPY ["src/Services/Support.Service/Seadora.Support.Infrastructure/Seadora.Support.Infrastructure.csproj", "src/Services/Support.Service/Seadora.Support.Infrastructure/"]
COPY ["src/Services/Common/Seadora.Common/Seadora.Common.csproj", "src/Services/Common/Seadora.Common/"]
COPY ["src/Services/Common/Seadora.Contracts/Seadora.Contracts.csproj", "src/Services/Common/Seadora.Contracts/"]
RUN dotnet restore "src/Services/Support.Service/Seadora.Support.API/Seadora.Support.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Support.Service/Seadora.Support.API"
RUN dotnet build "Seadora.Support.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Seadora.Support.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Seadora.Support.API.dll"]
