$ErrorActionPreference = "Stop"

# Find the repository root
$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

# Read the password from .env
$envFile = Join-Path $root ".env"
$line = Get-Content $envFile |
    Where-Object { $_ -match '^MSSQL_SA_PASSWORD=' } |
    Select-Object -First 1

if (-not $line) {
    throw "MSSQL_SA_PASSWORD is missing from .env"
}

$password = $line.Substring("MSSQL_SA_PASSWORD=".Length)
$env:MSSQL_SA_PASSWORD = $password

# Start SQL Server
docker compose up -d
if ($LASTEXITCODE -ne 0) {
    throw "Docker Compose failed."
}

# Wait until SQL Server accepts connections
$ready = $false

for ($i = 0; $i -lt 30; $i++) {
    $result = cmd /c "docker exec -e SQLCMDPASSWORD=$password addition-sql /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -C -l 2 -Q `"SELECT 1`" -b 2>nul"

    if ($LASTEXITCODE -eq 0) {
        $ready = $true
        break
    }

    Write-Host "Waiting for SQL Server... attempt $($i + 1)/30"
    Start-Sleep -Seconds 2
}

if (-not $ready) {
    throw "SQL Server did not become ready."
}

# Initialize the database
docker exec -e "SQLCMDPASSWORD=$password" addition-sql `
    /opt/mssql-tools18/bin/sqlcmd `
    -S localhost -U sa -C -b -i /scripts/init.sql

if ($LASTEXITCODE -ne 0) {
    throw "Database initialization failed."
}

Write-Host "SQL Server and AdditionDb are ready!"