#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM hca-docker-innersource.repos.medcity.net/containers/base/dotnet-8.0-runtime:latest AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 80
EXPOSE 443

FROM hca-docker-innersource.repos.medcity.net/containers/base/dotnet-8.0-build:latest AS build
WORKDIR /src
COPY ["demo/demo.csproj", "demo/"]
RUN dotnet restore "demo/demo.csproj"
COPY . .
WORKDIR "/src/demo"
RUN dotnet build "demo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "demo.csproj" -c Release -o /app/publish

FROM base AS final
ARG APP_USER=appuser
ARG APP_GROUP=appgroup
ARG COMPANY_NAME="HCA Healthcare"
ARG ORGANIZATION_NAME="Public Cloud Engineering"
ARG DEPARTMENT_NAME="Container Platform Administration"
LABEL company="${COMPANY_NAME}"
LABEL organization="${ORGANIZATION_NAME}"
LABEL department="${DEPARTMENT_NAME}"
LABEL version=$IMAGE_VERSION
WORKDIR /app
COPY --from=publish /app/publish .
RUN chown -Rh ${APP_USER}:${APP_GROUP} /app 
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "demo.dll"]
CMD ["--urls","http://0.0.0.0:8080"]