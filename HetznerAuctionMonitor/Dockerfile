FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["HetznerAuctionMonitor/HetznerAuctionMonitor.csproj", "HetznerAuctionMonitor/"]
RUN dotnet restore "HetznerAuctionMonitor/HetznerAuctionMonitor.csproj" -a $TARGETARCH
COPY . .
WORKDIR "/src/HetznerAuctionMonitor"
RUN dotnet build "HetznerAuctionMonitor.csproj" -c $BUILD_CONFIGURATION -o /app/build -a $TARGETARCH

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "HetznerAuctionMonitor.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false -a $TARGETARCH

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HetznerAuctionMonitor.dll"]
