FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["FGC.Users.Api/FGC.Users.Api.csproj", "FGC.Users.Api/"]
COPY ["FGC.Users.Application/FGC.Users.Application.csproj", "FGC.Users.Application/"]
COPY ["FGC.Users.Infrastructure/FGC.Users.Infrastructure.csproj", "FGC.Users.Infrastructure/"]
COPY ["FGC.Users.Domain/FGC.Users.Domain.csproj", "FGC.Users.Domain/"]
RUN dotnet restore "FGC.Users.Api/FGC.Users.Api.csproj"

COPY . .
WORKDIR "/src/FGC.Users.Api"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FGC.Users.Api.dll"]
