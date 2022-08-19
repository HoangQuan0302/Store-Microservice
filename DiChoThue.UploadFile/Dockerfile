#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["DiChoThue.UploadFile/DiChoThue.UploadFile.csproj", "DiChoThue.UploadFile/"]
RUN dotnet restore "DiChoThue.UploadFile/DiChoThue.UploadFile.csproj"
COPY . .
WORKDIR "/src/DiChoThue.UploadFile"
RUN dotnet build "DiChoThue.UploadFile.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DiChoThue.UploadFile.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DiChoThue.UploadFile.dll"]