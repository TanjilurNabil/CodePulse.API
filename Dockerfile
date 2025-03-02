# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 as build
WORKDIR /app
EXPOSE 80

COPY CodePulse.API.sln CodePulse.API.sln 
COPY CodePulse.API/CodePulse.API.csproj CodePulse.API/CodePulse.API.csproj
COPY . .
RUN dotnet restore CodePulse.API.sln

COPY CodePulse.API CodePulse.API

WORKDIR /app/CodePulse.API

RUN dotnet publish -c Release -o /app/out

# Serve Stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app/out .


ENTRYPOINT ["dotnet", "CodePulse.API.dll"]
