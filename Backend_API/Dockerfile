# Use the official ASP.NET Core runtime image as the base image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

# Use the official .NET Core SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Backend_API.csproj", "./"]
RUN dotnet restore "./Backend_API.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "Backend_API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Backend_API.csproj" -c Release -o /app/publish

# Copy the published application to the base image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Backend_API.dll"]
