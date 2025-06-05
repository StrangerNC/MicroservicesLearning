FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore 

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /appHKJHРKJJHKKH
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "CommandService.dll"]