# Use the official .NET SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory
WORKDIR /app

# Copy the .csproj file
COPY *.csproj .

# Restore NuGet packages
RUN dotnet restore

# Copy the remaining files
COPY . .

# Build the project
RUN dotnet publish -c Release -o out

# Use a smaller runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0

# Set the working directory
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app/out .

# Expose the necessary port
EXPOSE 5000

# Start the application
ENTRYPOINT ["dotnet", "MyApplication.dll"]
