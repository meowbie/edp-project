# GrowGreen Project

## Description
Coming soon...

## Set-up guide
Firstly, import the database script using SSMS.

This will create a new database called 'GrowGreen'.


Open Terminal (MacOS/Linux) or Command Prompt (Windows).

cd to GrowGreen/GrowGreenWeb (project directory).

Then type these commands (with Visual Studio closed):

```
dotnet tool install --global dotnet-ef --version 6.*
dotnet restore
dotnet user-secrets init
dotnet user-secrets ConnectionStrings:GrowGreenDB "Server=<server_ip_here>; 
Database=GrowGreen; User Id=<server_user_id_here>; Password=<server_user_password_here"
dotnet ef dbcontext scaffold Name=ConnectionStrings:GrowGreenDB Microsoft.EntityFrameworkCore.SqlServer --output-dir Models --force
```