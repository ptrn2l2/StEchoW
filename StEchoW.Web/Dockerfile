ARG DNSDK_BASE_IMAGE
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
# strange on mutiarch build only arm64 and arnhf fails:
#    RUN [ -z "$DNSDK_BASE_IMAGE" ] && echo "DNSDK_BASE_IMAGE is required" && exit 1 || true

# -----------------------------------------------------------------------
# You cannot build 43 bit arm on a 64 bit system, so we must choose a
# "fixed" sdk image to build (https://github.com/dotnet/dotnet-docker/issues/1537#issuecomment-755351628)
# and set "/p:UseAppHost=false" in publish. 
FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim-amd64 AS build
# ---
# I would like to choose based on an ARG variable, so to permit building on an arm64 system
# but I could not make this work 
# FROM ${DNSDK_BASE_IMAGE} AS build
# ---
# -----------------------------------------------------------------------
WORKDIR /src/StEchoW.Web
COPY "./StEchoW.Web.csproj" "/src/StEchoW.Web/."
WORKDIR "/src/StEchoW.Web"
RUN dotnet restore StEchoW.Web.csproj
COPY . .
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
