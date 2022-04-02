# MSSQL Docker

### How to Run

`docker-compose up`
for build image and start up the container

### Initial Database

after start container at the first time you can initial database with excute create-db.sh file

for Mac : `./create-db.sh`
for Windows : `bash create-db.sh`

or Execute docker command
`docker exec -it sql-server /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P KBTtestdb123 -d master -i /app/create-database.sql`
