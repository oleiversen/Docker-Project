FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY Web-Api-PoC.sln ./
COPY Web-Api/Web-Api.csproj Web-Api/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/Web-Api
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
HEALTHCHECK --interval=5s --timeout=3s --retries=3 CMD curl --fail http://localhost:80/backdoor/ || exit 1
ENTRYPOINT ["dotnet", "Web-Api.dll"]
