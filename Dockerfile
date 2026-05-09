FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Update to use MilkCollector.API.csproj
COPY ["MilkCollector.API.csproj", "./"]
RUN dotnet restore "MilkCollector.API.csproj"

COPY . .
# Build the project
RUN dotnet publish "MilkCollector.API.csproj" -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .

# Render configuration
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# The output DLL will also be named MilkCollector.API.dll
ENTRYPOINT ["dotnet", "MilkCollector.API.dll"]