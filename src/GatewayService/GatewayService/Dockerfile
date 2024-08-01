# Use the official .NET SDK 7.0 image as the base image for the build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 as build

# Set the working directory inside the container to /app
WORKDIR /app

# Expose port 80 (this is a hint to anyone running the container)
EXPOSE 80

# Copy the solution file and project files to the container
COPY car-auction-app.sln car-auction-app.sln
COPY src/AuctionService/AuctionService.csproj src/AuctionService/AuctionService.csproj
COPY src/SearchService/SearchService.csproj src/SearchService/SearchService.csproj
COPY src/IdentityService/IdentityService.csproj src/IdentityService/IdentityService.csproj
COPY src/GatewayService/GatewayService/GatewayService.csproj src/GatewayService/GatewayService/GatewayService.csproj
COPY src/BiddingService/BiddingService.csproj src/BiddingService/BiddingService.csproj
COPY src/Contracts/Contracts.csproj src/Contracts/Contracts.csproj

# Restore package dependencies using the .NET CLI
RUN dotnet restore car-auction-app.sln

# Copy the application folders from the host to the container
COPY src/GatewayService/GatewayService src/GatewayService/GatewayService

# Set the working directory to /app/src/AuctionService
WORKDIR /app/src/GatewayService/GatewayService

# Build the application in Release mode and output to /app/src/out
RUN dotnet publish -c Release -o /app/src/out

# Switch to a different base image for the runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0

# Set the working directory to /app
WORKDIR /app

# Copy the published output from the build stage to the runtime image
COPY --from=build /app/src/out .

# Specify the entry point for the container (the main application)
ENTRYPOINT ["dotnet", "GatewayService.dll"]


#Use the command to create the docker image from the instructions above 
# docker build -f path/to/Dockerfile -t image_tag .

# The build context is the set of files and directories that Docker uses to build the image.
# When you include a period (.) at the end of the docker build command, it tells Docker to use the current directory as the build context.
# Docker will search for the specified Dockerfile within this context (i.e., the current directory).
# If the Dockerfile is not in the current directory, you’ll encounter an error.
# See the graphical representation for the build context of the auction service directory:

#car-auction-app/
#├── src/
#│   └── AuctionService/
#│       └── Dockerfile
#└── other_files/