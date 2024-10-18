# # Use the official .NET SDK image to build the app
# FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# # Set the working directory inside the container
# WORKDIR /app

# # Copy the .csproj and restore any dependencies (via NuGet)
# COPY *.csproj ./
# RUN dotnet restore

# # Copy the rest of the application code
# COPY . ./

# # Build the application
# RUN dotnet publish -c Release -o /out

# # Use a runtime image for deployment
# FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# # Set the working directory for the runtime
# WORKDIR /app

# # Copy the built application from the previous stage
# COPY --from=build /out .

# # Expose the port that your application runs on
# EXPOSE 7013

# # Set the entry point to run your app
# ENTRYPOINT ["dotnet", "ComplianceCalendar.dll"]



# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory inside the container
WORKDIR /app

# Copy the .csproj and restore any dependencies (via NuGet)
COPY *.csproj ./
RUN dotnet restore --verbosity detailed

# Copy the rest of the application code
COPY . ./

# Build the application
RUN dotnet publish -c Release -o /out

# Use a runtime image for deployment
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

# Set the working directory for the runtime
WORKDIR /app

# Copy the built application from the previous stage
COPY --from=build /out .

# Expose the port that your application runs on
EXPOSE 7013

# Set the entry point to run your app
ENTRYPOINT ["dotnet", "ComplianceCalendar.dll"]
