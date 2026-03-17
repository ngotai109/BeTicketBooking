# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY BookingTicket.sln .
COPY BookingTicket.API/BookingTicket.API.csproj BookingTicket.API/
COPY BookingTicket.Application/BookingTicket.Application.csproj BookingTicket.Application/
COPY BookingTicket.Domain/BookingTicket.Domain.csproj BookingTicket.Domain/
COPY BookingTicket.Infrastructure/BookingTicket.Infrastructure.csproj BookingTicket.Infrastructure/

# Restore dependencies
RUN dotnet restore BookingTicket.sln

# Copy all source files
COPY . .

# Build and publish
RUN dotnet publish BookingTicket.API/BookingTicket.API.csproj -c Release -o /app/publish --no-restore

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Railway sẽ cung cấp biến PORT
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}

EXPOSE 8080

ENTRYPOINT ["dotnet", "BookingTicket.API.dll"]
