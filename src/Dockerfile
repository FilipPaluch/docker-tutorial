# Prepare build image based on net core sdk
FROM microsoft/dotnet:2.1-sdk AS build-env

# Set work dir to /app. 
# The WORKDIR instruction sets the working directory for any RUN, CMD, ENTRYPOINT, COPY and ADD instructions that follow it in the Dockerfile.
# The WORKDIR instruction can be used multiple times in a Dockerfile. If a relative path is provided, it will be relative to the path of the previous WORKDIR instruction. For example:
# WORKDIR /a
# WORKDIR b
# WORKDIR c
# RUN pwd
# The output of the final pwd command in this Dockerfile would be /a/b/c.

WORKDIR /app

# The COPY instruction copies new files or directories from <src> and adds them to the filesystem of the container at the path <dest>.
# Source path is relative to build directory. Dest path is relative to workdir
COPY ["DockerTutorial/DockerTutorial.csproj", "DockerTutorial/"]

# The dotnet restore command uses NuGet to restore dependencies as well as project-specific tools that are specified in the project file. 
# A common way to take advantage of the build cache when building your ASP.NET Core app, is to copy across only the .csproj, .sln and nuget.config files for your app before doing dotnet restore, 
# instead of copying the entire source code. 
# The NuGet package restore can be one of the slowest parts of the build, and it only depends on these files. 
# By copying them first, Docker can cache the result of the restore, so it doesn't need to run again if all you do is change a .cs file for example.
# For performance purposes, it's important to take advantage of Docker's caching mechanisms when building your ASP.NET Core applications. 
# Some of the biggest gains can be had by caching the restore phase of the build process.
RUN dotnet restore "DockerTutorial/DockerTutorial.csproj"

# Copy everything else
COPY . .

# dotnet publish [<PROJECT>]. Dotnet publish compiles the application, reads through its dependencies specified in the project file, and publishes the resulting set of files to a directory.
RUN dotnet publish "DockerTutorial/DockerTutorial.csproj" -c Release -o /app/out


# Build runtime image
FROM microsoft/dotnet:2.1-aspnetcore-runtime

WORKDIR /app

# Optionally COPY accepts a flag --from=<name|index> that can be used to set the source location to a previous build stage (created with FROM .. AS <name>) 
COPY --from=build-env /app/out .

# The EXPOSE instruction informs Docker that the container listens on the specified network ports at runtime. 
# The EXPOSE instruction does not actually publish the port. 
# It functions as a type of documentation between the person who builds the image and the person who runs the container, about which ports are intended to be published. 
# To actually publish the port when running the container, use the -p flag on docker run to publish and map one or more ports
EXPOSE 5000

ENTRYPOINT ["dotnet", "DockerTutorial.dll"]