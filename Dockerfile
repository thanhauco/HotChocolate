FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["src/ElectronicsComponents.GraphQL/ElectronicsComponents.GraphQL.csproj", "src/ElectronicsComponents.GraphQL/"]
RUN dotnet restore "src/ElectronicsComponents.GraphQL/ElectronicsComponents.GraphQL.csproj"

# Copy the rest of the code
COPY . .

# Build and publish
RUN dotnet publish "src/ElectronicsComponents.GraphQL/ElectronicsComponents.GraphQL.csproj" -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ElectronicsComponents.GraphQL.dll"] 