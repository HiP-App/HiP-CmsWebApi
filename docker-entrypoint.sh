#!/bin/bash
set -e
APPDIR = src/HiP-CmsWebApi/
mv $(APPDIR)appsettings.json $(APPDIR)appsettings.json.bak
sed 's_\w*"ConnectionString":.*$_      "ConnectionString": "Host='$HIP_POSTGRES_PORT_5432_TCP_ADDR';Username='$HIP_POSTGRES_ENV_POSTGRES_USER';Password='$HIP_POSTGRES_ENV_POSTGRES_PASSWORD';Database='$HIP_POSTGRES_ENV_POSTGRES_DB'"_' $(APPDIR)appsettings.json.bak > $(APPDIR)appsettings.json

exec $(APPDIR)dotnet run