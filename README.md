# DoctorAppointmentApi

ASP.NET Core Web API for the WebDoctor MVC application. It uses a Firebird database.

## Configure the database

The database file and connection string are local settings and are not included in this repository. Set `DBSettings:DbConnectionString` in an ignored `DoctorAppointmentApi/appsettings.Development.json`, or set the environment variable `DBSettings__DbConnectionString` before running the API.

Example PowerShell session variable:

```powershell
$env:DBSettings__DbConnectionString = "<your Firebird connection string>"
```

## Run locally

Open `DoctorAppointmentApi.slnx` in Visual Studio, configure the database connection, and run the `DoctorAppointmentApi` project. The included launch profile uses `https://localhost:7110` and `http://localhost:5091`.
