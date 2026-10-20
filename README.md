# GestionStk-BE

REST API backend for a stock-management application. The API is built with ASP.NET Core and Entity Framework Core and uses PostgreSQL for persistence.

## Current features

- Client CRUD endpoints: create, list, get by ID, update, and delete.
- PostgreSQL connectivity check.
- Entity Framework Core data model for clients, articles, suppliers, orders, invoices, order lines, and supplier-article stock lots.

Only client routes and the connectivity check are currently exposed as HTTP endpoints. The other entities are database models; they do not yet have API controllers.

## Technology

- ASP.NET Core Web API, target framework `net10.0`
- .NET Data Access Layer (`DAL`) and repository project, target framework `net8.0`
- Entity Framework Core and the Npgsql PostgreSQL provider
- PostgreSQL
- IIS deployment support through ASP.NET Core Module (ANCM)

## Repository layout

```text
GestionStk-BE/
├── API/
│   ├── Controllers/ClientController.cs  # Client HTTP endpoints
│   ├── Properties/launchSettings.json   # Local development URLs
│   ├── Program.cs                       # Dependency injection and middleware
│   └── API.csproj
├── DAL/
│   ├── Entities/                        # EF Core entities and STKDbContext
│   ├── Repositories/                    # Client repository
│   └── DAL.csproj
├── Repositories/                        # Shared repository abstractions
├── publish/                             # Existing publish output; regenerate when deploying
└── GestionStk-BE.sln
```

## Prerequisites

For local development:

1. Windows, macOS, or Linux.
2. The .NET 10 SDK. The API targets `net10.0`.
3. PostgreSQL, with a database and user that the API can access.
4. Git, if cloning this repository.

For IIS hosting, see [Deploy to IIS](#deploy-to-iis) for the additional Windows prerequisites.

## Configuration

The API reads its PostgreSQL connection string from the `ConnectionStrings:DefaultConnection` configuration key. The local configuration file is `API/appsettings.json`.

Set the connection string to match your PostgreSQL host, port, database, and credentials. For example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=Gestion_stk;Username=postgres;Password=YOUR_LOCAL_PASSWORD"
  }
}
```

Do not commit real passwords or production connection strings. For local development, prefer .NET User Secrets or an environment variable. ASP.NET Core maps double underscores in environment variable names to configuration sections:

```powershell
$env:ConnectionStrings__DefaultConnection = "Host=localhost;Port=5432;Database=Gestion_stk;Username=postgres;Password=YOUR_LOCAL_PASSWORD"
```

The value above applies only to the current PowerShell session. Configure an appropriate persistent secret or environment variable for your own machine/server when needed.

The current configuration must match the database server exactly. A PostgreSQL error such as `28P01: password authentication failed` means the username/password are not accepted; a connection-refused error usually means the host, port, or PostgreSQL service is unavailable. The database must already exist and contain the tables described below; this repository does not currently include a database creation or migration script.

## Run locally

From the repository root:

```powershell
dotnet restore .\GestionStk-BE.sln
dotnet build .\GestionStk-BE.sln
dotnet run --project .\API\API.csproj
```

The configured launch profiles use:

- HTTP: `http://localhost:5284`
- HTTPS: `https://localhost:7100`

The API's root endpoint is `GET /`. HTTPS redirection may send HTTP requests to HTTPS depending on the active launch profile. For local HTTPS, trust the development certificate if prompted:

```powershell
dotnet dev-certs https --trust
```

The OpenAPI document is mapped in the Development environment at:

```text
https://localhost:7100/openapi/v1.json
```

This project maps an OpenAPI document, not a Swagger UI page.

## Database model

The EF Core context is `DAL.Entities.STKDbContext`. It maps these tables:

| Table | Description |
|---|---|
| `client` | Customer details |
| `article` | Stock items |
| `fournisseur` | Suppliers |
| `commande` | Customer orders |
| `facture` | Invoices associated with orders |
| `ligne_commande` | Articles and quantities in an order |
| `lst_article` | Supplier/article stock lots |

Relationships include clients to orders, orders to invoices and order lines, articles to order lines and supplier lots, and suppliers to supplier lots. The `client.email` and `fournisseur.email` columns are configured with unique indexes.

## API endpoints

Base URL for the local HTTP launch profile: `http://localhost:5284`.

| Method | Route | Description | Successful response |
|---|---|---|---|
| `GET` | `/` | API status | `200 OK` |
| `GET` | `/api/test-db` | Checks whether PostgreSQL can be reached | `200 OK` |
| `GET` | `/api/Client` | Returns all clients | `200 OK` |
| `GET` | `/api/Client/{id}` | Returns a client by `Codeclt` | `200 OK`, or `404 Not Found` |
| `POST` | `/api/Client` | Creates a client | `201 Created` |
| `PUT` | `/api/Client/{id}` | Updates a client | `204 No Content`, or `404 Not Found` |
| `DELETE` | `/api/Client/{id}` | Deletes a client | `204 No Content`, or `404 Not Found` |

### Client request body

Use `Content-Type: application/json`.

```json
{
  "nom": "Jane Doe",
  "adresse": "10 Main Street",
  "email": "jane.doe@example.com",
  "tel": "+1 555 0100"
}
```

For `PUT /api/Client/{id}`, include `codeclt` in the JSON body and ensure it matches the ID in the URL:

```json
{
  "codeclt": 1,
  "nom": "Jane Doe",
  "adresse": "12 Main Street",
  "email": "jane.doe@example.com",
  "tel": "+1 555 0100"
}
```

Create validation currently requires `nom` and `adresse`, each no longer than 50 characters. `email` is optional, limited to 50 characters, and must be unique when supplied. `tel` is optional and limited to 20 characters. A validation error returns `400 Bad Request` with a validation-problem JSON response. The API does not currently enforce an email format.

### Example responses

`GET /`:

```json
{
  "message": "Gestion Stock API",
  "status": "API is running"
}
```

`GET /api/test-db`:

```json
{
  "database": "Gestion_stk",
  "connected": true
}
```

The `database` field is a fixed label in the endpoint; `connected` reports the result of the database connection check.

## Test with Postman

1. Start the API using the local run instructions.
2. In Postman, create a collection, for example `GestionStk-BE`.
3. Add a collection variable named `baseUrl` with value `http://localhost:5284`. Change it to `https://localhost:7100` if using the HTTPS profile.
4. Add requests below. For JSON requests, set `Content-Type` to `application/json`.

| Test | Request |
|---|---|
| API is running | `GET {{baseUrl}}/` |
| Database connection | `GET {{baseUrl}}/api/test-db` |
| List clients | `GET {{baseUrl}}/api/Client` |
| Get one client | `GET {{baseUrl}}/api/Client/1` |
| Create client | `POST {{baseUrl}}/api/Client` with the client JSON above |
| Update client | `PUT {{baseUrl}}/api/Client/1` with a body containing `"codeclt": 1` |
| Delete client | `DELETE {{baseUrl}}/api/Client/1` |

For a basic CRUD test, create a client, copy the returned `codeclt`, use it for the get/update/delete requests, and list clients to verify the changes. Use an email address not already present in the database when testing creation.

If Postman reports a TLS/certificate error while using HTTPS locally, either trust the .NET development certificate or use the HTTP base URL for local testing. If a request returns a database error, first check `GET /api/test-db`, then verify PostgreSQL is running and the configured credentials, port, database, and schema are correct.

## Deploy to IIS

### 1. Prepare the Windows server

1. Install and enable IIS.
2. Install the **ASP.NET Core Hosting Bundle for .NET 10** on the server. It installs the .NET runtime and the IIS ASP.NET Core Module needed to host the API.
3. Restart IIS after installing the Hosting Bundle.
4. Ensure the IIS server can reach the PostgreSQL host and port.

### 2. Publish the API

From the repository root on a machine with the .NET 10 SDK:

```powershell
dotnet publish .\API\API.csproj --configuration Release --output .\publish
```

Copy the published output to a deployment directory on the IIS server, for example `C:\inetpub\GestionStkApi`. Deploy the *contents* of the publish output, including `API.dll` and `web.config`; do not point IIS at the source-code folder.

### 3. Configure the IIS website

In IIS Manager:

1. Create an application pool for the API and set **.NET CLR Version** to **No Managed Code**.
2. Create a website (or IIS application), select the publish directory as its physical path, and assign the application pool.
3. Configure an HTTP or HTTPS binding and an available port/host name.
4. Grant the application-pool identity read and execute access to the publish directory. Grant write access only to directories the application actually needs to write.
5. Set `ASPNETCORE_ENVIRONMENT` to `Production` for the IIS application. The OpenAPI endpoint is only mapped in Development, so it will not be available in Production by default.
6. Configure `ConnectionStrings__DefaultConnection` for the IIS application or server using a secure deployment-specific method. Do not put production credentials in a committed file.
7. Recycle the application pool after changing configuration.

The published `web.config` is used by IIS and ASP.NET Core Module to start the app. Do not remove it from the deployment directory.

### 4. Verify the IIS deployment

Use the IIS binding as the Postman base URL, for example `http://localhost:8080`:

```text
GET http://localhost:8080/
GET http://localhost:8080/api/test-db
GET http://localhost:8080/api/Client
```

The actual URL depends on the IIS binding you configure. The production server should use HTTPS for non-local access.

## Troubleshooting

| Symptom | What to check |
|---|---|
| `dotnet` cannot target `net10.0` | Install the .NET 10 SDK for development, or the .NET 10 Hosting Bundle/runtime for IIS hosting. |
| PostgreSQL connection refused | Verify PostgreSQL is running and the host/port are reachable. |
| PostgreSQL error `28P01` | Verify the PostgreSQL username and password. |
| Database or relation does not exist | Create/use the expected database and provision its tables. No schema setup script is included here. |
| API starts but client operations fail | Check the database connectivity endpoint and confirm the `client` table and its columns exist. |
| IIS returns `500.30` or fails to start | Check the Hosting Bundle/runtime installation, deployment files, application-pool identity, connection configuration, and Windows Event Viewer. Temporarily enable stdout logs only for diagnosis, protect the log directory, and disable them again afterward. |
| HTTPS request fails in local Postman | Trust the .NET development certificate or test over the local HTTP URL. |

## Current limitations

- Client is the only entity with CRUD API routes at this time.
- No database schema script or EF Core migration setup is included in the repository.
- No authentication or authorization is configured; do not expose the API publicly until access control and production security settings are added.
- OpenAPI document generation is enabled only when the application runs in Development.
