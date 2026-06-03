FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["certs/fortinet-ca.crt", "/usr/local/share/ca-certificates/fortinet-ca.crt"]
RUN update-ca-certificates

COPY ["src/EcommerceApi.Api/EcommerceApi.Api.csproj", "src/EcommerceApi.Api/"]
COPY ["src/EcommerceApi.Application/EcommerceApi.Application.csproj", "src/EcommerceApi.Application/"]
COPY ["src/EcommerceApi.Domain/EcommerceApi.Domain.csproj", "src/EcommerceApi.Domain/"]
COPY ["src/EcommerceApi.Infrastructure/EcommerceApi.Infrastructure.csproj", "src/EcommerceApi.Infrastructure/"]

RUN dotnet restore "src/EcommerceApi.Api/EcommerceApi.Api.csproj"

COPY . .

RUN dotnet publish "src/EcommerceApi.Api/EcommerceApi.Api.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "EcommerceApi.Api.dll"]
