set scriptpath=%~dp0
cd %~dp0

docker build -t simsdb databasePostgres\. 
docker build -t simsapi --label "latest" SIMSAPI\.
docker build -t sims --label "latest" SIMS\.
docker compose up -d
