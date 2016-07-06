FROM microsoft/dotnet:latest

RUN mkdir -p /dotnetapp
WORKDIR /dotnetapp

COPY src /dotnetapp
RUN dotnet restore

EXPOSE 5000

ENTRYPOINT ["dotnet", "run", "-p", "Api/project.json"]