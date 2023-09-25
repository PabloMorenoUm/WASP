FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["WASP/WASP.csproj", "WASP/"]
RUN dotnet restore "WASP/WASP.csproj"
COPY . .
WORKDIR "/src/WASP"
RUN dotnet build "WASP.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WASP.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WASP.dll"]
