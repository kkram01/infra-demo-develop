## demo application

### Files
- Dockerfile1
- [Project Name].csproj - Handles configuration and Dependencies of the application
- Program.cs - Handles initialization of the application
- Controllers - Details the endpoints the application will serve
- Logic - Defines the logic to be used in the controllers.
- appsettings / launchSettings.json - A place to include environment variables for the application

### Testing
build and test locally:
- IMAGE_NAME=demo
- docker build -t $IMAGE_NAME .
- docker run -dp 127.0.0.1:8080:8080 $IMAGE_NAME
- curl 127.0.0.1:8080

cleanup after test:
- docker container rm -f $(docker container ls | grep $IMAGE_NAME | awk '{print $1}')
- docker image rm $IMAGE_NAME 

## Connect to Cloud SQL from on Premesis

From on-prem you will connect to the public endpoint, you can find your public endpoint , so a [firewall rule](https://hcaservicecentral.service-now.com/hca?id=hca_cat_item&sys_id=bc9146dedb79970006c1ef92ca96196e) is required to connect. If you are using the [cloud SQL auth proxy](https://cloud.google.com/sql/docs/postgres/connect-auth-proxy), the outbound port is `3307` instead of `5432` 

### Connecting to Cloud SQL postgresql

The service account provided, only has access to connect to public schema, you will need to [Grant](https://www.postgresql.org/docs/current/sql-grant.html) it access to the databases and tables it needs access to by obtaining the defaul user credentials. You can reset the password by following [this document](https://cloud.google.com/sql/docs/postgres/create-manage-users#change-pwd).

## Manage Database connections best practices:

If you do not handle connections properly, you might lock yourself out of the database, please read the [following doc to learn more](https://cloud.google.com/sql/docs/postgres/manage-connections#c_1)

## Cloud CI/CD pipelines

The pipeline documentation can be found in this [directory](/docs) in this repo
