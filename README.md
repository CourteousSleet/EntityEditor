# Entity Editor

A small ASP.NET Core Razor Pages CRUD demo using .NET 10 LTS, Entity Framework
Core 10 and SQL Server. Create, list, view, edit and delete clients. Organization
types are company (EN) and individual entrepreneur (IE). The existing Founder
model is retained; sample founders belong to a company.

## Run locally

Install a current [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
and SQL Server Express LocalDB (available through the Visual Studio installer).
The command line is sufficient; Visual Studio is optional.

```powershell
dotnet restore EntityEditor.sln --locked-mode
dotnet run --project EntityEditor.csproj
```

Open the localhost URL printed by the application and select **Clients**.
For the HTTPS development profile, use `dotnet dev-certs https --trust` if needed.
This workspace also has an ignored SDK in `.tools/dotnet`; you can invoke
`.\.tools\dotnet\dotnet.exe` instead of `dotnet`.

The default connection uses Windows authentication and the original LocalDB
database name, so an existing database is preserved. A new database is created
and seeded with sample data only when both tables are empty. Seeded company
founders are deleted along with their client. Dates are managed by the server
in UTC; forms accept only the client's name, tax number and organization type.

For another SQL Server (including one accessible from Linux/macOS), override
`ConnectionStrings__EntityEditorContext` with your connection string. The
default `TrustServerCertificate=True` is for local development; use a valid
server certificate and `TrustServerCertificate=False` for remote deployments.
Startup fails with a logged error if the database cannot be initialized.

This is a local demonstration with no authentication. The application uses
`EnsureCreated`, not schema migrations: it will not upgrade an incompatible
existing schema. Back up existing data before making future schema changes.

## Verify

```powershell
dotnet build EntityEditor.sln --configuration Release --no-restore
dotnet test EntityEditor.sln --configuration Release --no-build
dotnet list EntityEditor.sln package --vulnerable --include-transitive
```

Tests use real SQL Server and real HTTP Razor Pages requests. Each test creates
and deletes its own randomly named `EntityEditorTests_*` database. They never
use the application's database. LocalDB is the default; set
`ENTITYEDITOR_TEST_CONNECTION` to use another test SQL Server. The test login
needs permission to create and drop databases; the database name in the supplied
connection string is replaced with a unique test name.

## Dependency maintenance

Only the EF Core SQL Server provider is a direct application package. Obsolete
scaffolding/tooling packages and the Bootstrap/jQuery frontend libraries have
been removed. The interface uses local CSS and server-side validation.

NuGet lock files record transitive dependencies. Known vulnerable packages cause
restore to fail. GitHub Actions builds and tests on Windows and runs weekly;
Dependabot checks NuGet and GitHub Actions weekly. After intentionally changing
package versions, regenerate locks with `dotnet restore --force-evaluate`,
then run the verification commands.

Generated `bin`, `obj`, `.vs` and user-specific project files are excluded
from version control. These changes must reach the GitHub default branch before
GitHub can reevaluate its dependency alerts.
