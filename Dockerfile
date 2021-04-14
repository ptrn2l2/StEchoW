FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["StEchoW.Web/StEchoW.Web.csproj", "StEchoW.Web/"]
RUN dotnet restore "StEchoW.Web/StEchoW.Web.csproj"
COPY . .
WORKDIR "/src/StEchoW.Web"
RUN dotnet build "StEchoW.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StEchoW.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StEchoW.Web.dll"]
