[1]: https://docs.microsoft.com/en-us/aspnet/core/security/docker-compose-https?view=aspnetcore-7.0

## **Setup before the first run**

### **Docker-compose HTTPS configuration**

To run `WebApi` using the HTTPS protocol in Docker,
you will need to export the certificate for Kestrel to a folder and mount it as a volume.

By default, `docker-compose.override.yml` has a volume mounted already:

    - ${APPDATA}/ASP.NET/Https:/root/.aspnet/https:ro

Therefore, all you need to do is to export the certificate by running the following command in PowerShell:

    dotnet dev-certs https --trust -ep "${env:appdata}\ASP.NET\Https\localhost.pfx" -p "Password123"

> NOTE: The commands above are valid for Windows using Linux containers. If you are using another configuration - you can find more information [here][1].

After that you will need to set the Kestrel environment variables in the `docker-compose.override.yml`:

```
- ASPNETCORE_Kestrel__Certificates__Default__Path=/root/.aspnet/https/localhost.pfx
- ASPNETCORE_Kestrel__Certificates__Default__Password=Password123
```

---

### **SMTP configuration**

To send emails, the application uses the SMTP client, which is configured using the values from the `options.json` files.  
You can find these files in the `Options` project.

Here is the list of these files:

- options.json
- options.Development.json
- options.Docker.json
- options.Docker.Development.json
- options.Staging.json

The values are picked from the file, which `options.[environment_name].json` matches the current environment name.  
It is the same mechanism as being used with the `appsettings.json` files by default.

**Each file has a section called `SmtpOptions`, for the application to be able to run - this section must contain valid values.**

It is up to you what mailing service to use.

Here is an example of how to configure this section for using Gmail as a mailing service:

```json
{
  "SmtpOptions": {
    "Credentials": {
      "UserName": "your.email.address@gmail.com",
      "Password": "Pa$$word123"
    },
    "EnableSecureSocketsLayer": true,
    "Host": "smtp.gmail.com",
    "Port": 587,
    "Timeout": 100000
  }
}
```

---

### **Database migrations**

Migrations are running on application startup. In multi-instance environments and/or distributed applications it is recommended
to perform migrations as a deployment step, rather than startup step, to avoid data-corruption, errors, and concurrency issues.

Migrations are running from here:
`Paradise.ApplicationLogic.Infrastructure.Seed.IDatabaseSeeder.EnsureStorageAvailableAsync`.

In multi-instance environments and/or distributed applications the method mentioned above
should only perform database availability check, but not create/amend database schema.

A single database is being used to persist the application data.

Two database contexts - `InfrastructureContext` and `DomainContext`, each of them tied to the database scheme: `infrastructure` and `domain` respectively.

To create a new migration open the terminal in `DataAccess` project directory and run migration creation command with the following parameters:

| Property name                    | Description                                                                       | Value                                                                                                                                |
| -------------------------------: | :-------------------------------------------------------------------------------: | :----------------------------------------------------------------------------------------------------------------------------------- |
| `-p`                             | Relative path to the project folder of the target project.                        | `"Paradise.DataAccess.csproj"`                                                                                                       |
| `-o`                             | The directory use to output the files.                                            | `"Database\Migrations\ApplicationLogic\Infrastructure\Domain"` or `"Database\Migrations\Domain"` (depending on the database context) |
| `-c`                             | The `DbContext` class to use.                                                     | `"InfrastructureContext"` or `"DomainContext"`                                                                                       |

**Complete example of the command:**

    dotnet ef migrations add InitialState -p "Paradise.DataAccess.csproj" -o "Database\Migrations\Domain" -c "DomainContext"