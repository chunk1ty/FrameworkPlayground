# [Install MS SQL](https://hub.docker.com/_/microsoft-mssql-server)
```
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=yourStrong(!)Password" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
```

## Credentials

 &nbsp;        | &nbsp;                    
|--------------|---------------------------|
| **Host**     | **localhost**             |
| **Port**     | **1433**                  |
| **User**     | **sa**                    |
| **Password** | **yourStrong(!)Password** |

# Run migrations
Nagivate to: 
```
cd src\EntityFrameworkCore\SamuraiApp\SamuraiApp.Data
```

and execute the following command:
```
dotnet ef database update
```