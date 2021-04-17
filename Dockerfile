FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build only on x64!!!
# see: https://github.com/dotnet/dotnet-docker/issues/1537
# and corresponding answer: https://github.com/dotnet/dotnet-docker/issues/1537#issuecomment-755351628
#                           (but I have used "/p:UseAppHost=false" in publish)
FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim-amd64 AS build
WORKDIR /src
COPY ["StEchoW.Web/StEchoW.Web.csproj", "StEchoW.Web/"]
RUN dotnet restore "StEchoW.Web/StEchoW.Web.csproj"
COPY . .
WORKDIR "/src/StEchoW.Web"
RUN dotnet build "StEchoW.Web.csproj" -c Release -o /app/build

FROM build AS publish
# Set UseAppHost=false to avoid creating a rid specific executable.
# Without this, the executable would be linux-amd64 specific.
# ref: https://github.com/dotnet/dotnet-docker/issues/1537#issuecomment-755351628
RUN dotnet publish "StEchoW.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StEchoW.Web.dll"]
