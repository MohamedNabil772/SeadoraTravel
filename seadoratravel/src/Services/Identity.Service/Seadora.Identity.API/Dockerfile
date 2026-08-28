FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/Services/Identity.Service/Seadora.Identity.API/Seadora.Identity.API.csproj", "src/Services/Identity.Service/Seadora.Identity.API/"]
COPY ["src/Services/Identity.Service/Seadora.Identity.Application/Seadora.Identity.Application.csproj", "src/Services/Identity.Service/Seadora.Identity.Application/"]
COPY ["src/Services/Identity.Service/Seadora.Identity.Domain/Seadora.Identity.Domain.csproj", "src/Services/Identity.Service/Seadora.Identity.Domain/"]
COPY ["src/Services/Identity.Service/Seadora.Identity.Infrastructure/Seadora.Identity.Infrastructure.csproj", "src/Services/Identity.Service/Seadora.Identity.Infrastructure/"]
COPY ["src/Services/Common/Seadora.Common/Seadora.Common.csproj", "src/Services/Common/Seadora.Common/"]
COPY ["src/Services/Common/Seadora.Contracts/Seadora.Contracts.csproj", "src/Services/Common/Seadora.Contracts/"]
RUN dotnet restore "src/Services/Identity.Service/Seadora.Identity.API/Seadora.Identity.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Identity.Service/Seadora.Identity.API"
RUN dotnet build "Seadora.Identity.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Seadora.Identity.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Seadora.Identity.API.dll"]
