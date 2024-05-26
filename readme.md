# example-webapp

- [features](#features)
- [quickstart](#quickstart)
- [production](#production)
- [prerequisites](#prerequisites)
  - [create selfsigned cert](#create-selfsigned-cert)
  - [setup development nginx](#setup-development-nginx)
  - [adjust local dns](#adjust-local-dns)
  - [install root ca for local development](#install-root-ca-for-local-development)
- [how this project was built](#how-this-project-was-built)

<hr/>

## features

- asp net core backend
- react frontend + vite tooling
- https self signed cert development
- backend and frontend debugging in a solution
- publish release with frontend webpacked available through server static files available directly from within backend
- step by step how this project was built by git commit

## quickstart

- (optional) see [how this project was built](#how-this-project-was-built) for project setup from scratch
- see [prerequisites](#prerequisites) to setup self signed dev cert and nginx proxy

```sh
git clone https://github.com/devel0/example-webapp.git
cd example-webapp
code .
```

- choose `.NET Core Launch (web)` from run and debug then hit F5 ( this will start asp net web server on localhost:5000 )

- start vite

```sh
cd clientapp
npm run dev
```

- choose `Launch Chrome` from run and debug then click the play icon ( this will start browser )

- app running in local development within https

![](./doc/example-webapp.png)

- two debugger active ( asp net core + chrome )

![](./doc/example-webapp-debug.png)

## production

- prepare the build

```sh
dotnet publish -c Release
```

results

```sh
devel0@tuf:~/opensource/example-webapp$ ls /home/devel0/opensource/example-webapp/WebApiServer/bin/Release/net8.0/publish
appsettings.Development.json                     Microsoft.OpenApi.dll                  WebApiServer.deps.json
appsettings.json                                 Swashbuckle.AspNetCore.Swagger.dll     WebApiServer.dll
clientapp                                        Swashbuckle.AspNetCore.SwaggerGen.dll  WebApiServer.pdb
Microsoft.AspNetCore.OpenApi.dll                 Swashbuckle.AspNetCore.SwaggerUI.dll   WebApiServer.runtimeconfig.json
Microsoft.AspNetCore.SpaServices.Extensions.dll  WebApiServer                           web.config
```

- if want to publish with all required runtime to run w/out installed dotnet for specific platform

```sh
dotnet publish -c Release --runtime linux-x64 --sc
```

- start the production on server manually to test it

```sh
$ ./WebApiServer --urls http://10.10.1.47:5000
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://10.10.1.47:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Production
info: Microsoft.Hosting.Lifetime[0]
      Content root path: /root/publish
warn: Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionMiddleware[3]
      Failed to determine the https port for redirect.
```

- create `/etc/systemd/system/example-webapp.service` for automatic start at server boot

```conf
[Unit]
Description=Example webapp
After=network.target
StartLimitIntervalSec=0

[Service]
Type=simple
Restart=always
RestartSec=10
User=testuser
ExecStart=/srv/WebApiServer
Environment=ASPNETCORE_URLS=http://10.10.1.47:5000
Environment=ASPNETCORE_ENVIRONMENT=Production
#EnvironmentFile=/root/secrets/WebApiServer.Production.env

[Install]
WantedBy=multi-user.target
```

- enable service `systemctl enable example-webapp.service`

- start the service `service example-webapp start`

- watch service log `journalctl -u example-webapp -f`

- configure nginx server to point on the webapi server ( the lines managed by certbot are created using `certbot --nginx test.searchathing.com` )

```conf
server {
  root /var/www/html;

  #rewrite_log on;
  #access_log /var/log/nginx/test.access.log;
  #error_log /var/log/nginx/test.error.log notice;

  server_name test.searchathing.com;

  rewrite /$ /app;

  location /api {
    include /etc/nginx/mime.types;

    proxy_set_header Host $host;
    proxy_pass http://10.10.1.47:5000;
    proxy_set_header X-Real-IP $remote_addr;
    proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
  }

  location /app {
    include /etc/nginx/mime.types;

    proxy_set_header Host $host;
    proxy_pass http://10.10.1.47:5000/app;
    proxy_set_header X-Real-IP $remote_addr;
    proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;

    proxy_http_version 1.1;
    proxy_set_header Upgrade $http_upgrade;
    proxy_set_header Connection "upgrade";
  }

  listen 443 ssl; # managed by Certbot
  ssl_certificate /etc/letsencrypt/live/test.searchathing.com/fullchain.pem; # managed by Certbot
  ssl_certificate_key /etc/letsencrypt/live/test.searchathing.com/privkey.pem; # managed by Certbot
  include /etc/letsencrypt/options-ssl-nginx.conf; # managed by Certbot
  ssl_dhparam /etc/letsencrypt/ssl-dhparams.pem; # managed by Certbot
}

server {
  if ($host = test.searchathing.com) {
    return 301 https://$host$request_uri;
  } # managed by Certbot

  server_name test.searchathing.com;
  listen 80;
  return 404; # managed by Certbot
}
```

- try browse `https://test.searchathing.com` ( replace with your own )

## prerequisites

### create selfsigned cert

- clone [linux-scripts-utils](https://github.com/devel0/linux-scripts-utils)

```sh
mkdir -p ~/opensource
cd ~/opensource
git clone git@github.com:devel0/linux-scripts-utils.git
export PATH=$PATH:~/opensource/linux-scripts-utils
mkdir -p ~/sscerts
chmod 700 ~/sscerts
```

- create cert parameters file `~/sscerts/searchathing.com.params` ( replace `searchathing.com` with your own )

```
COUNTRY="IT"
STATE="Italy"
CITY="Trento"
ORGNAME="SearchAThing"
ORGUNIT="Development"
DOMAIN=localhost
DURATION_DAYS=36500 # 100 years
```

- create root-ca certificates

```sh
CERTPARAMS=~/sscerts/searchathing.com.params create-root-ca.sh
```

- generated root-ca files

| file                             | description                                                                             |
| -------------------------------- | --------------------------------------------------------------------------------------- |
| `~/sscerts/searchathing.com.crt` | root-ca certificate that you can register into the browser to trust linked certificates |
| `~/sscerts/searchathing.com.key` | key of the root-ca certificte ( this is NOT NEEDED anywhere, do not share )             |

- create a couple of test certificates ( note: the path uses the first certificate name, if you don't change the first name the path remain the same even you extend to add more )

```sh
CERTPARAMS=~/sscerts/searchathing.com.params create-cert.sh --add-empty webapp-test test1 test2
```

| file                                                                      | description                                  |
| ------------------------------------------------------------------------- | -------------------------------------------- |
| `~/sscerts/webapp-test.searchathing.com/webapp-test.searchathing.com.crt` | is the certificate crt for nginx https proxy |
| `~/sscerts/webapp-test.searchathing.com/webapp-test.searchathing.com.key` | is the certificate key for nginx https proxy |

### setup development nginx

- install nginx

```sh
apt install nginx
```

- create `/etc/nginx/conf.d/test-webapp.conf`

```conf
#
# DEVELOPMENT
#
server {
  root /var/www/html;

  rewrite_log on;
  access_log /var/log/nginx/webapp-test.access.log;
  error_log /var/log/nginx/webapp-test.error.log notice;

  server_name webapp-test.searchathing.com;

  rewrite /$ /app;

  location /swagger {
    include /etc/nginx/mime.types;

    proxy_set_header Host $host;
    proxy_pass http://127.0.0.1:5000/swagger;
    proxy_set_header X-Real-IP $remote_addr;
    proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
  }

  location /api {
    include /etc/nginx/mime.types;

    proxy_set_header Host $host;
    proxy_pass http://127.0.0.1:5000;
    proxy_set_header X-Real-IP $remote_addr;
    proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
  }

  location /app {
    include /etc/nginx/mime.types;

    proxy_set_header Host $host;
    proxy_pass http://127.0.0.1:5100/app;
    proxy_set_header X-Real-IP $remote_addr;
    proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;

    proxy_http_version 1.1;
    proxy_set_header Upgrade $http_upgrade;
    proxy_set_header Connection "upgrade";
  }

  listen 443 ssl;
  ssl_certificate /home/devel0/sscerts/webapp-test.searchathing.com/webapp-test.searchathing.com.crt;
  ssl_certificate_key /home/devel0/sscerts/webapp-test.searchathing.com/webapp-test.searchathing.com.key;
}

server {
  if ($host = webapp-test.searchathing.com) {
    return 301 https://$host$request_uri;
  }

  server_name webapp-test.searchathing.com;
  listen 80;
  return 404;
}
```

### adjust local dns

- edit `/etc/hosts`

```
127.0.0.1  localhost webapp-test.searchathing.com
```

### install root ca for local development

Installing root-ca certificate imply that certificates generated within that will be consequently trusted.

- **chrome**: settings/Privacy and security/Security/Manage certificates/Authorities/Import
  - select `~/sscerts/searchathing.com_CA.crt`
  - tick `Trust this certificate for identifying websites`

- **firefox**: settings/Privacy & Security/Certificates/View Certificates/Authorities/Import
  - select `~/sscerts/searchathing.com_CA.crt`
  - tick `Trust this CA to identify websites`
  
- **shell**
  - copy `~/sscerts/searchathing.com_CA.crt` to `/usr/local/share/ca-certificates`
  - issue `sudo update-ca-certificates`

## how this project was built

```sh
mkdir example-webapp
cd example-webapp
git init
dotnet new gitignore
dotnet new webapi -n WebApiServer
npm create vite@latest clientapp -- --template react-ts
cd clientapp
npm install --save-dev @vitejs/plugin-react
cd ..
dotnet new sln
dotnet sln add WebApiServer
dotnet build
```

- vscode (C-S-p: Generate assets)
- [bunch of configs](https://github.com/devel0/example-webapp/commit/ffda8d6054c097bc8418ceed5286a97d52420b43)
