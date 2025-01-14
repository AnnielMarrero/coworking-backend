# README #

This README would normally document whatever steps are necessary to get your application up and running.

### What is this repository for? ###

Aplicación coworking permite gestionar salas y reservas para esa salas, asi como la gestión de usuarios y roles


### How do I get set up? ###

* The solution consists of 4 projects, one for the API (AMBEV), another for the domain and data access (Domain and Data) respectively, and another for the unit tests (Test). The app uses SSO with Azure Active Directory accounts.
* Deployment instructions: In the folder .ambevtech is configured the pipeline for three environments: dev, stg and prd.
* The app use Azure Blob storage for store permanentely some users uploads, tipically .pdf, .jpeg nad .png files. The app is configured in dev mode to use "Azurite" fr testing. For startup Azurite go to C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\Extensions\Microsoft\Azure Storage Emulator, this program is installed automatically in VS 2022, opne the cmd in admimistrator mode and type: azurite.exe start --skipApiVersionCheck , in this way it can be uploaded, delete and downloaded files locally. The containerName = "stpgpeventos-conn-string" and the connection string is configured through an environment variable. The configuration is done in the Program.cs.

### Build the application
```
cd coworking
dotnet build
```

### Run the application
```
cd coworking
dotnet run
```

### Test the application
```
cd coworking.Test
dotnet test --settings:test.runsettings

```

### Run migrations 

(From package manager)
```
add-migration <name>
update-database
remove-migration
```

(From terminal)
```
cd coworking (in application folder)
dotnet ef migrations add <name>
dotnet ef database update
dotnet ef migrations remove
```

In the Program.cs file there is a segment of code that  will run the migration programmatically, once the application was run

### Code format 
First install the tool: dotnet tool install csharpier -g
After that move to the whole directory that contains the solution, execute the below command
```
dotnet csharpier .
```

### Explicación de las decisiones técnicas y arquitectónicas
Se usan controladoras que llaman a metodos de la capa de servicios que a su vez llaman a metodos de la capa de reopsitorio, para organizar el codigo y permitir reutilizarlo, se siguen los patrones SOLID
En el fichero coworking.http existen algunos ejemplos peticiones a la api

Ejemplo de curl

curl --location 'https://localhost:7083/Reservations/Create' \
--header 'Content-Type: application/json' \
--header 'Authorization: Bearer eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9zaWQiOiIxNCIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJzdHJpbmciLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJzdHJpbmciLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiIyIiwiZXhwIjoxNzM2ODg0Mzk2fQ.njtDHAs_JR1oZO7IgOl8Vj3AXsToQMHa6zzEISxg2lw' \
--data '{
  "roomId": 2,
  "date": "2025-01-14T15:44:09.549Z"
}'

Diagrama de entidad relación

![Diagrama de la BD](db_diagrama.png)