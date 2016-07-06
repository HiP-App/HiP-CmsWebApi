FROM microsoft/dotnet:latest

RUN mkdir -p /dotnetapp
RUN dotnet restore

COPY src /dotnetapp

WORKDIR /dotnetapp/Api
EXPOSE 5000
ENTRYPOINT ["dotnet", "run", "-p", "project.json"]