FROM microsoft/dotnet:1.0.0-preview2.1-sdk

RUN mkdir -p /dotnetapp

COPY src /dotnetapp
WORKDIR /dotnetapp

RUN dotnet restore

EXPOSE 5000

VOLUME ["/dotnetapp/Api/wwwroot"]

WORKDIR /dotnetapp/Api
ENTRYPOINT ["dotnet", "run", "-p", "project.json"]