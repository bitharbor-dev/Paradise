[1]: https://docs.microsoft.com/en-us/aspnet/core/security/docker-compose-https?view=aspnetcore-7.0

## **Table of contents**

1. Setup before the first run
    - [Docker-compose HTTPS configuration](#**docker-compose-https-configuration**)
    - [SMTP configuration](#**smtp-configuration**)
    
2. Pre-implemented functionalities
    - [User management module](#**user-management-module**)
    - [Role management module](#**role-management-module**)
    - [Email templates and notifications module](#**email-templates-and-notifications-module**)

---

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

    - ASPNETCORE_Kestrel__Certificates__Default__Path=/root/.aspnet/https/localhost.pfx
    - ASPNETCORE_Kestrel__Certificates__Default__Password=Password123

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

    "SmtpOptions": {
        "Credentials": {
            "UserName": "your.email.address@gmail.com",
            "Password": "Password123"
        },
        "EnableSsl": true,
        "Host": "smtp.gmail.com",
        "Port": 587,
        "Timeout": 100000
    }

---

## **Pre-implemented functionalities**

Please note, these are the lists of functionalities that are exposed on the API level.  
But there are a lot of internal features described in the next parts of the documentation.

### **User management module**

- Getting the list of users
- Getting user by id
- Registration
- Email confirmation
- Logging in
- Two-factor authentication via email
- Renewing access tokens
- Logging out
- Terminating all sessions
- Resetting email address
- Resetting password
- Updating users
- Deleting users

### **Role management module**

- Getting the list of roles
- Getting role by id
- Getting the list of user roles
- Creating roles
- Updating roles
- Deleting roles
- Assigning roles
- Unassigning roles

### **Email templates and notifications module**

- Getting the list of templates
- Getting template by id
- Creating templates
- Updating templates
- Deleting templates