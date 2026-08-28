FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/Services/Content.Service/Seadora.Content.API/Seadora.Content.API.csproj", "src/Services/Content.Service/Seadora.Content.API/"]
COPY ["src/Services/Content.Service/Seadora.Content.Application/Seadora.Content.Application.csproj", "src/Services/Content.Service/Seadora.Content.Application/"]
COPY ["src/Services/Content.Service/Seadora.Content.Domain/Seadora.Content.Domain.csproj", "src/Services/Content.Service/Seadora.Content.Domain/"]
COPY ["src/Services/Content.Service/Seadora.Content.Infrastructure/Seadora.Content.Infrastructure.csproj", "src/Services/Content.Service/Seadora.Content.Infrastructure/"]
COPY ["src/Services/Common/Seadora.Common/Seadora.Common.csproj", "src/Services/Common/Seadora.Common/"]
COPY ["src/Services/Common/Seadora.Contracts/Seadora.Contracts.csproj", "src/Services/Common/Seadora.Contracts/"]
RUN dotnet restore "src/Services/Content.Service/Seadora.Content.API/Seadora.Content.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Content.Service/Seadora.Content.API"
RUN dotnet build "Seadora.Content.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Seadora.Content.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Seadora.Content.API.dll"]
