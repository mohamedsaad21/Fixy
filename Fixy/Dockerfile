FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Fixy.Api/Fixy.Api.csproj", "Fixy.Api/"]
COPY ["Fixy.Application/Fixy.Application.csproj", "Fixy.Application/"]
COPY ["Fixy.Domain/Fixy.Domain.csproj", "Fixy.Domain/"]
COPY ["Fixy.Infrastructure/Fixy.Infrastructure.csproj", "Fixy.Infrastructure/"]
RUN dotnet restore "Fixy.Api/Fixy.Api.csproj"
COPY . .
WORKDIR "/src/Fixy.Api"
RUN dotnet build "Fixy.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Fixy.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet", "Fixy.Api.dll" ]