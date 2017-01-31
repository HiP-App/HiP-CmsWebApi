FROM microsoft/dotnet:1.1.0-sdk-projectjson

RUN apk add --update ca-certificates && \
    rm -rf /var/cache/apk/* /tmp/*

RUN update-ca-certificates

RUN mkdir -p /dotnetapp

COPY src /dotnetapp
WORKDIR /dotnetapp

RUN dotnet restore

EXPOSE 5000

VOLUME ["/dotnetapp/Api/wwwroot"]

WORKDIR /dotnetapp/Api
ENTRYPOINT ["dotnet", "run", "-p", "project.json"]