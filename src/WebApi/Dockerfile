FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore WebApi/BusTicketReservationSystem.WebApi.csproj

RUN dotnet publish WebApi/BusTicketReservationSystem.WebApi.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:10000
ENV DOTNET_USE_POLLING_FILE_WATCHER=1
EXPOSE 10000

ENTRYPOINT ["dotnet", "BusTicketReservationSystem.WebApi.dll"]