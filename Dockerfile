FROM microsoft/aspnet:1.0.0-rc1-update1

RUN printf "deb http://ftp.us.debian.org/debian jessie main\n" >> /etc/apt/sources.list
RUN apt-get -qq update && apt-get install -qqy net-tools && rm -rf /var/lib/apt/lists/*

COPY . /app
WORKDIR /app
RUN DOCKER_HOST_IP=$(route -n | awk '/UG[ \t]/{print $2}') &&\
	echo $DOCKER_HOST_IP &&\
	mv app/appsettings.json app/appsettings.json.bak &&\
	sed 's/^      "ConnectionString":.*$/      "ConnectionString": "Host='$DOCKER_HOST_IP';Username='$DB_USER';Password='$DB_PSWD';Database='$DB_NAME'"/' app/appsettings.json.bak > app/appsettings.json.bak1 &&\
	sed 's/^      "Username":.*$/      "Username": "'$ADMIN_EMAIL'"/' app/appsettings.json.bak1 > app/appsettings.json.bak2 &&\
	sed 's/^      "Password":.*$/      "Password": "'$ADMIN_PSWD'"/' app/appsettings.json.bak2 > app/appsettings.json &&\
	rm -f app/appsettings.json.bak app/appsettings.json.bak1 app/appsettings.json.bak2 

RUN ["dnu", "restore"]

EXPOSE 5000/tcp
ENTRYPOINT ["dnx", "-p", "app/project.json", "web"]
