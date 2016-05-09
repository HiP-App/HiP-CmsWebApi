#!/bin/bash
#DATABASE_NAME=hipapp
#DATABASE_USER=hip
#DATABASE_PSWD=password

#install postgresql database
#apt-get update && apt-get install postgresql -y

#create user and database
#sudo -u "postgres" createdb $DATABASE_NAME
#sudo -u "postgres" psql $DATABASE_NAME --command="CREATE USER "$DATABASE_USER" WITH PASSWORD '"$DATABASE_PSWD"';" 
#sudo -u "postgres" psql $DATABASE_NAME --command="GRANT ALL PRIVILEGES ON DATABASE "$DATABASE_NAME" TO "$DATABASE_USER";"

#get docker host ip, we need it to listen on that port
DOCKER_HOST_IP=$( ip addr show docker0 | grep inet | grep -Eo '(([0-9]+\.){3}[0-9]+)' )
echo "Docker Host IP: " $DOCKER_HOST_IP 

#get the postgres config file
updatedb
POSTGRES_CONFIG_PATH=$(locate postgresql.conf -n 1)
echo "Postgres config file is located under: " $POSTGRES_CONFIG_PATH

PG_HBA_CONFIG_PATH=$(locate pg_hba.conf -n 1)
echo "Postgres hba  config file is located under: " $PG_HBA_CONFIG_PATH

#update the postgres config file to listen on our docker host ip
sed -i 's/#listen_addresses/listen_addresses/' $POSTGRES_CONFIG_PATH
sed -i "/listen_addresses/s/'.*'/'"$DOCKER_HOST_IP"'/" $POSTGRES_CONFIG_PATH

PG_HBA_LINE='host	'$DB_NAME'	'$DB_USER'	172.16.0.1/12	password'
echo $PG_HBA_LINE
echo $(tail -1 $PG_HBA_CONFIG_PATH)
if [ "$(tail -1 $PG_HBA_CONFIG_PATH)" != "echo $PG_HBA_LINE" ]
	then
	echo $PG_HBA_LINE >> $PG_HBA_CONFIG_PATH
fi

#restart postrgesql service to reload config file
/etc/init.d/postgresql restart
