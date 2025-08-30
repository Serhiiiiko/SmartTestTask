# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["SmartTestTask.sln", "./"]
COPY ["src/SmartTestTask.API/SmartTestTask.API.csproj", "src/SmartTestTask.API/"]
COPY ["src/SmartTestTask.Application/SmartTestTask.Application.csproj", "src/SmartTestTask.Application/"]
COPY ["src/SmartTestTask.Domain/SmartTestTask.Domain.csproj", "src/SmartTestTask.Domain/"]
COPY ["src/SmartTestTask.Infrastructure/SmartTestTask.Infrastructure.csproj", "src/SmartTestTask.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/SmartTestTask.API/SmartTestTask.API.csproj"

# Copy everything else
COPY . .

# Build the application
WORKDIR "/src/src/SmartTestTask.API"
RUN dotnet build "SmartTestTask.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "SmartTestTask.API.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Copy published files
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "SmartTestTask.API.dll"]