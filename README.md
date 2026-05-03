# Grupa3-Tim3

## FitManager baza podataka

Aplikacija koristi SmarterASP MSSQL bazu.

### Pristup bazi kroz web panel

1. Otvoriti: https://mssql-eu.site4now.net/
2. Unijeti podatke za bazu:
   - Server: `SQL6034.site4now.net`
   - Database: `db_ac8e2a_fitmanager`
   - Username: `db_ac8e2a_fitmanager_admin`
   - Password: lozinka se ne cuva u repozitoriju; preuzeti je od clana tima/administratora.

### Lokalno pokretanje aplikacije sa cloud bazom

Connection string se postavlja kroz .NET user secrets, a ne u `appsettings.json`:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Data Source=SQL6034.site4now.net;Initial Catalog=db_ac8e2a_fitmanager;User Id=db_ac8e2a_fitmanager_admin;Password=<PASSWORD>;Encrypt=True;TrustServerCertificate=True;" --project FitManager.csproj
```

Nakon toga se migracije mogu provjeriti ili primijeniti iz foldera `FitManager`:

```powershell
.\obj\dotnet-tools\dotnet-ef.exe migrations list
.\obj\dotnet-tools\dotnet-ef.exe database update
```
