# -----------------------------------------
# Stage 1: Base image (Runtime) - very lightweight, only for running the app
# -----------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# -----------------------------------------
# Stage 2: Build image (SDK) - heavy, contains all developer tools
# -----------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# First, copy ONLY the project files (.csproj)
COPY ["AggregationService/AggregationService.csproj", "AggregationService/"]
COPY ["AggregationService.Domain/AggregationService.Domain.csproj", "AggregationService.Domain/"]
COPY ["AggregationService.Application/AggregationService.Application.csproj", "AggregationService.Application/"]
COPY ["AggregationService.Infrastructure/AggregationService.Infrastructure.csproj", "AggregationService.Infrastructure/"]
COPY ["AggregationService.Sql/AggregationService.Sql.csproj", "AggregationService.Sql/"]

# Restore dependencies (download NuGet packages)
RUN dotnet restore "AggregationService/AggregationService.csproj"

# Now copy all the rest of the code
COPY . .
WORKDIR "/src/AggregationService"

# Build the project
RUN dotnet build "AggregationService.csproj" -c Release -o /app/build

# -----------------------------------------
# Stage 3: Publish
# -----------------------------------------
FROM build AS publish
RUN dotnet publish "AggregationService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# -----------------------------------------
# Stage 4: Final image (Production)
# -----------------------------------------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Specify what to run when the container starts
ENTRYPOINT ["dotnet", "AggregationService.dll"]