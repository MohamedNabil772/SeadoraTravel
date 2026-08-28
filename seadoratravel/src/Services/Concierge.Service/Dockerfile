FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Services/Concierge.Service/Seadora.Concierge.API/Seadora.Concierge.API.csproj", "src/Services/Concierge.Service/Seadora.Concierge.API/"]
COPY ["src/Services/Concierge.Service/Seadora.Concierge.Application/Seadora.Concierge.Application.csproj", "src/Services/Concierge.Service/Seadora.Concierge.Application/"]
COPY ["src/Services/Concierge.Service/Seadora.Concierge.Domain/Seadora.Concierge.Domain.csproj", "src/Services/Concierge.Service/Seadora.Concierge.Domain/"]
COPY ["src/Services/Concierge.Service/Seadora.Concierge.Infrastructure/Seadora.Concierge.Infrastructure.csproj", "src/Services/Concierge.Service/Seadora.Concierge.Infrastructure/"]
RUN dotnet restore "src/Services/Concierge.Service/Seadora.Concierge.API/Seadora.Concierge.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Concierge.Service/Seadora.Concierge.API"
RUN dotnet build "Seadora.Concierge.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Seadora.Concierge.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Seadora.Concierge.API.dll"]
