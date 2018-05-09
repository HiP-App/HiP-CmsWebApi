FROM microsoft/dotnet:2.0.0-sdk-jessie

RUN mkdir -p /dotnetapp

COPY src /dotnetapp
WORKDIR /dotnetapp

EXPOSE 5000

VOLUME ["/dotnetapp/Api/wwwroot"]

WORKDIR /dotnetapp/Api
RUN dotnet build
ENTRYPOINT ["dotnet", "run"]

#RUN dotnet publish -c Release -o out
#ENTRYPOINT ["dotnet", "out/Api.dll"]
