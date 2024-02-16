* * *

Test
============

Description
-----------
- Logic test and crud test


Installation
------------

1.  Clone or download the repository.
2.  Open the project in visual studio

How To Run
------------
- Logic Test

How to run :

1. open solution in folder logic
2. running

--------
- CRUD Test

How to run :

1. run mssql server with docker, ```docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Example@Password123" -p 1433:1433 --name sql_server_container -d mcr.microsoft.com/mssql/server:2019-latest``` (if not have database running on your machine )
2. open crud folder 
3. run  this ```dotnet ef database update --project .\CRUDServices\CRUDServices.csproj```
4. run scafffold ```Scaffold-DbContext "Data Source=localhost; Initial Catalog=USER; User ID=sa;Password=Example@Password123; MultipleActiveResultSets=true;TrustServerCertificate=true" Microsoft.EntityFrameworkCore.SqlServer -NoOnConfiguring -OutputDir "DataAccess\Models\CRUD" -UseDatabaseNames -ContextDir "DataAccess\Context" -Context CRUDContext -NoPluralize -f```
5. run solution
--------


Contact
-------

*   Author: Ricky Ariansyah
*   Email: rickyarians@outlook.com

* * *
