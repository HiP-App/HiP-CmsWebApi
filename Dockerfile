FROM microsoft/dotnet:1.1-sdk

RUN mkdir -p /dotnetapp

COPY src /dotnetapp
WORKDIR /dotnetapp

RUN dotnet restore

EXPOSE 5000

VOLUME ["/dotnetapp/Api/wwwroot"]

WORKDIR /dotnetapp/Api
RUN dotnet publish -c Release -o out
ENTRYPOINT ["dotnet", "out/Api.dll"]