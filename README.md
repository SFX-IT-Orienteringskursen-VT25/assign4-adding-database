# assign4-adding-database
The assignment was introduced in Class #8. In the link you can find the slides from the presentation as well as a recording of the class.
https://classroom.google.com/c/MjI5OTg3MzMwODFa/p/MjM0NjU0MTY0MTVa/details

Class #9 contains my aggregated feedback on your first attempts at the assignment.
https://classroom.google.com/c/MjI5OTg3MzMwODFa/p/ODA3MDA5NDI3MjE5/details

This simple example shows you how to setup MS SQL server in Docker from your application code
https://github.com/SFX-IT-Orienteringskursen-VT25/setup-mssql-with-docker-example

## Instructions
Don't start this assignment until you have finshed assignment 3!

Clone this repository

Reuse your application from assignment 3.

Create a branch

Requirements:
- The Api should use MS SQL as the persistence layer
- The MS SQL server should be hosted in Docker
- The methods for saving and retrieving data should be done towards the MS SQL database

Commit

Push your branch

Create a pull request `<your branch> => <main>`

Done!
## Assignment 4 — Addition API with SQL Server and Docker

This project extends Assignment 3 by storing numbers in Microsoft SQL Server instead of returning hardcoded values.

### Requirements

- .NET 10 SDK
- Docker Desktop
- PowerShell

### Database setup

1. Clone the repository.
2. Create a `.env` file in the repository root:

   ```dotenv
   MSSQL_SA_PASSWORD=YOUR_STRONG_PASSWORD
   ```

3. Start SQL Server and initialize the database:

   ```powershell
   powershell.exe -NoProfile -ExecutionPolicy Bypass -File .\Database\init.ps1
   ```

   This creates the Docker container, the `AdditionDb` database, and the `Numbers` table if they don't already exist.

### Configure the API

Navigate to the API directory:

```powershell
cd Server\AdditionApi
```

Configure the connection string using .NET User Secrets:

```powershell
dotnet user-secrets set "ConnectionStrings:AdditionDb" "Server=localhost,1433;Database=AdditionDb;User Id=sa;Password=YOUR_STRONG_PASSWORD;Encrypt=True;TrustServerCertificate=True;"
```

Use the same password as in `.env`.

### Run the API

```powershell
dotnet run
```

### API endpoints

**GET `/api/numbers`**

Returns the numbers stored in SQL Server.

**PUT `/api/numbers`**

Replaces the existing numbers with the supplied JSON array.

Example request body:

```json
[5, 10, 3]
```

### Stop SQL Server

From the repository root:

```powershell
docker compose down
```

The database remains stored in the persistent Docker volume.