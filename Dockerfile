# Stage 1: Restore dependencies
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR ReviewAPI/
COPY ReviewAPI/*.csproj ./
# RUN dotnet restore
 
# Stage 2: Build the application
COPY ReviewAPI ./
RUN dotnet publish -c Release -o out
 
# Stage 3: Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
RUN ls
WORKDIR .
COPY --from=build /ReviewAPI/out .
ENTRYPOINT ["dotnet", "ReviewAPI.dll"]
 
EXPOSE 8080