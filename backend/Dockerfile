FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Kanban.sln ./
COPY src/Kanban.Domain/Kanban.Domain.csproj src/Kanban.Domain/
COPY src/Kanban.Application/Kanban.Application.csproj src/Kanban.Application/
COPY src/Kanban.Infrastructure/Kanban.Infrastructure.csproj src/Kanban.Infrastructure/
COPY src/Kanban.Api/Kanban.Api.csproj src/Kanban.Api/
COPY tests/Kanban.UnitTests/Kanban.UnitTests.csproj tests/Kanban.UnitTests/
RUN dotnet restore Kanban.sln

COPY . .
RUN dotnet publish src/Kanban.Api/Kanban.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Kanban.Api.dll"]
