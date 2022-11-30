FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Tests_Generator/Tests_Generator.csproj", "Tests_Generator/"]
RUN dotnet restore "Tests_Generator/Tests_Generator.csproj"
COPY . .
WORKDIR "/src/Tests_Generator"
RUN dotnet build "Tests_Generator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tests_Generator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tests_Generator.dll"]
