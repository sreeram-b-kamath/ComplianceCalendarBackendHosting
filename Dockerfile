# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory inside the container
WORKDIR /src

# Copy the .csproj and restore any dependencies (via NuGet)
COPY ["ComplianceCalendar/ComplianceCalendar.csproj", "ComplianceCalendar/"]


RUN dotnet restore "ComplianceCalendar/ComplianceCalendar.csproj"
# Copy the rest of the application code
COPY . .

# Build the application
RUN dotnet publish "ComplianceCalendar/ComplianceCalendar.csproj" -c Release -o /app/publish

# Use a runtime image for deployment
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set the working directory for the runtime
WORKDIR /app

# Copy the built application from the previous stage
COPY --from=build /app/publish .

# Expose the port that your application runs on
EXPOSE 80

# Set the entry point to run your app
ENTRYPOINT ["dotnet", "ComplianceCalendar.dll"]
