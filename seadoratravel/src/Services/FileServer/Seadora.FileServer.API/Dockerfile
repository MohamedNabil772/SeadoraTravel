FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/Services/FileServer/Seadora.FileServer.API/Seadora.FileServer.API.csproj", "src/Services/FileServer/Seadora.FileServer.API/"]
COPY ["src/Services/Common/Seadora.Common/Seadora.Common.csproj", "src/Services/Common/Seadora.Common/"]
RUN dotnet restore "src/Services/FileServer/Seadora.FileServer.API/Seadora.FileServer.API.csproj"
COPY . .
WORKDIR "/src/src/Services/FileServer/Seadora.FileServer.API"
RUN dotnet build "Seadora.FileServer.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Seadora.FileServer.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Seadora.FileServer.API.dll"]
