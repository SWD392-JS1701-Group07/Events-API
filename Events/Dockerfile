#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Events/Events.API.csproj", "Events/"]
COPY ["Events.Business/Events.Business.csproj", "Events.Business/"]
COPY ["Events.Data/Events.Data.csproj", "Events.Data/"]
COPY ["Events.Models/Events.Models.csproj", "Events.Utils/"]
RUN dotnet restore "Events/Events.API.csproj"
COPY . .
WORKDIR "/src/Events"
RUN dotnet build "Events.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Events.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Events.API.dll"]