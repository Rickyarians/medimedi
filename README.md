* * *

Test
============

Description
-----------

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


Installation
------------

1.  Clone or download the repository.
2.  Open the project in visual studio
3.  Build the solution.

Usage
-----

1.  Run the application.
2.  Follow the on-screen instructions.

Contributing
------------

1.  Fork the repository.
2.  Create a new branch (`git checkout -b feature/your-feature`).
3.  Make your changes.
4.  Commit your changes (`git commit -am 'Add new feature'`).
5.  Push to the branch (`git push origin feature/your-feature`).
6.  Create a new Pull Request.

License
-------

This project is licensed under the \[License Name\] - see the [LICENSE.md](LICENSE.md) file for details.

Contact
-------

*   Author: Ricky Ariansyah
*   Email: rickyarians@outlook.com

* * *
