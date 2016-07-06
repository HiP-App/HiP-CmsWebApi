FROM microsoft/dotnet:latest

RUN mkdir -p /dotnetapp

COPY src /dotnetapp
WORKDIR /dotnetapp

RUN dotnet restore

EXPOSE 5000

WORKDIR /dotnetapp/Api
ENTRYPOINT ["dotnet", "run", "-p", "project.json"]