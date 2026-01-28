FROM mcr.microsoft.com/dotnet/sdk:8.0-bookworm-slim AS build
WORKDIR /src
COPY . .
RUN dotnet restore ./FrenzyNet.Api.csproj
RUN dotnet publish ./FrenzyNet.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0-bookworm-slim
RUN apt-get update \
    && apt-get install -y --no-install-recommends wireguard-tools jq curl \
    && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "FrenzyNet.Api.dll"]
