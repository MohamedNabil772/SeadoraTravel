FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/Services/Booking.Service/Seadora.Booking.API/Seadora.Booking.API.csproj", "src/Services/Booking.Service/Seadora.Booking.API/"]
COPY ["src/Services/Booking.Service/Seadora.Booking.Application/Seadora.Booking.Application.csproj", "src/Services/Booking.Service/Seadora.Booking.Application/"]
COPY ["src/Services/Booking.Service/Seadora.Booking.Domain/Seadora.Booking.Domain.csproj", "src/Services/Booking.Service/Seadora.Booking.Domain/"]
COPY ["src/Services/Booking.Service/Seadora.Booking.Infrastructure/Seadora.Booking.Infrastructure.csproj", "src/Services/Booking.Service/Seadora.Booking.Infrastructure/"]
COPY ["src/Services/Common/Seadora.Common/Seadora.Common.csproj", "src/Services/Common/Seadora.Common/"]
RUN dotnet restore "src/Services/Booking.Service/Seadora.Booking.API/Seadora.Booking.API.csproj"
COPY . .
WORKDIR "/src/src/Services/Booking.Service/Seadora.Booking.API"
RUN dotnet build "Seadora.Booking.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Seadora.Booking.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Seadora.Booking.API.dll"]
