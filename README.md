# 📂 Assign 4: Adding database 

## 💡 Project Overview
The Assign 4 is a simple REST API built on .NET that provides Key-Value Storage functionality.

The primary goal of this API is to demonstrate the implementation of minimalist web services using the .NET Minimal APIs architecture and direct connection to a SQL Server database.

## ⚙️ Installation

*  9.0 .NET Version SDK

* Docker Desktop (Required to run the SQL Server container).

* HTTP Client: [Postman, Insomnia, or curl] for testing the endpoints.

## 🚀 Setup and Execution

The easiest way to launch both the API service and the database is by using docker-compose.



### 💿 Database Configuration
* The docker-compose.yml file sets up an MSSQL Server container and maps port 1433.

* Database Name: AdditionApiDB

* SA Password: password$123


### 🔧 Start Services (Recommended)

From the root directory containing the docker-compose.yml, run:

```bash
docker compose up --build
```
or

build docker and your project separably: 

```bash
docker compose up 
```

```bash
dotnet run
```
##### ‼️ Remember to run your project in your correct folder.

## 🧪 API Endpoints
The application exposes the following endpoints at *http://localhost:5262*


GET 
---
```bash
curl http://localhost:5262/api/addition/{key}
```

POST
---
```bash
curl -X POST http://localhost:5262/api/addition \
  -H "Content-Type: application/json" \
  -d '{
      "key": "1",
      "value": "1"
  }'
```

## 📝 5. Project: 

Application configuration and definition of all Minimal API endpoints and database only with development functions.
```