FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["Exhibition/Exhibition.csproj", "Exhibition/"]
RUN dotnet restore "Exhibition/Exhibition.csproj"
COPY . .
WORKDIR "/src/Exhibition"
RUN dotnet build "Exhibition.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Exhibition.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Exhibition.dll"]