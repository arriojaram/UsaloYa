Scaffold-DbContext "Data Source=DESKTOP-PL9432I\MSSQLSERVER01;Initial Catalog=Avatar;Integrated Security=True;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models


// Commands to do the code first in VS and start working with the DB in PostgreSql
dotnet ef migrations add InitialMigration
dotnet ef database update

// To remove an existing migration - run this only during development.
dotnet ef migrations remove


// For the release, a SQL script must be created with the follow command and executed in the prod environment
dotnet ef migrations script --idempotent --output "avatarDbMigrations.sql"

dotnet ef migrations script 20240913161525_AddBuyPriceToSales 20240926234312_AddCustomerTable --idempotent --output "avatarDbMigrations.sql"

// Regresar a una version especifica de DB
dotnet ef database update 20240913161525_AddBuyPriceToSales


// Command to map the DB from Powershell in VS - Database first
dotnet ef dbcontext scaf

// Run as offline mode in VS
npm install -g http-server
http-server -p 8082
 serve -s .\dist\pwa\browser\ 