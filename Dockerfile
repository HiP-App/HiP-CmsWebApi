FROM microsoft/dotnet:onbuild

RUN printf "deb http://ftp.us.debian.org/debian jessie main\n" >> /etc/apt/sources.list

EXPOSE 5000

COPY docker-entrypoint.sh /
RUN chmod +x docker-entrypoint.sh

ENTRYPOINT ["/docker-entrypoint.sh"]

COPY . /src
WORKDIR /src/HiP-CmsWebApi

RUN ["dotnet", "restore"]


