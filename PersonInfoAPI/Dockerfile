# Use official .NET SDK image to build and publish the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy everything and restore dependencies
COPY . ./
RUN dotnet restore

# Build the application
RUN dotnet publish -c Release -o out

# Use a lightweight runtime image for execution
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

# Copy the compiled app from the previous stage
COPY --from=build /app/out ./

# Expose the port for the API
EXPOSE 8080

# Set the entry point
ENTRYPOINT ["dotnet", "PersonInfoAPI.dll"]