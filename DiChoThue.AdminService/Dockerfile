#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["DiChoThue.AdminService/DiChoThue.AdminService.csproj", "DiChoThue.AdminService/"]
RUN dotnet restore "DiChoThue.AdminService/DiChoThue.AdminService.csproj"
COPY . .
WORKDIR "/src/DiChoThue.AdminService"
RUN dotnet build "DiChoThue.AdminService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DiChoThue.AdminService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DiChoThue.AdminService.dll"]