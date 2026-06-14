# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Website forum/Website forum.csproj", "Website forum/"]
RUN dotnet restore "Website forum/Website forum.csproj"

COPY . .
WORKDIR "/src/Website forum"
RUN dotnet publish "Website forum.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Website forum.dll"]
