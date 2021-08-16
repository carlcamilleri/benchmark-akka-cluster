#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
#
#FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
#WORKDIR /app
#EXPOSE 80
#
#FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
#WORKDIR /src
#COPY ["benchmark-harness/benchmark-harness.csproj", "benchmark-harness/"]
#RUN dotnet restore "benchmark-harness/benchmark-harness.csproj"
#COPY . .
#WORKDIR "/src/benchmark-harness"
#RUN dotnet build "benchmark-harness.csproj" -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish "benchmark-harness.csproj" -c Release -o /app/publish
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "benchmark-harness.dll"]
#
#
#
#
#

#ARG PROJPATH=thespis-api
#ARG DEPPATH=.

FROM alpine:latest AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
ARG PROJPATH
ARG DEPPATH
RUN echo $PROJPATH
RUN echo $DEPPATH
WORKDIR /src
COPY ["$PROJPATH/benchmark-akka-cluster.csproj", "$PROJPATH"]
#COPY ["$DEPPATH/proto", "$DEPPATH/proto/"]
RUN dotnet restore "$PROJPATH/benchmark-akka-cluster.csproj"
COPY $PROJPATH $PROJPATH
#COPY ["$DEPPATH/docker-image-resources/akka.hocon", "$PROJPATH/"]
#RUN dotnet publish "$PROJPATH/api/thespis-api.csproj" -c Release  --runtime linux-musl-x64 -p:PublishTrimmed=true   -p:PublishSingleFile=true -p:TrimMode=Link -o /app/publish
RUN dotnet publish "$PROJPATH/benchmark-akka-cluster.csproj" -c Release  --runtime linux-musl-x64  -o /app/publish



FROM base AS final

RUN apk add --no-cache \ 
    openssh libunwind \
    nghttp2-libs libidn krb5-libs libuuid lttng-ust zlib \
    libstdc++ libintl \
    icu

WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["./benchmark-akka-cluster"]

