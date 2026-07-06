FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/ApiGateway/Seadora.ApiGateway.csproj", "src/ApiGateway/"]
RUN dotnet restore "src/ApiGateway/Seadora.ApiGateway.csproj"
COPY . .
WORKDIR "/src/src/ApiGateway"
RUN dotnet build "Seadora.ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Seadora.ApiGateway.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Seadora.ApiGateway.dll"]
