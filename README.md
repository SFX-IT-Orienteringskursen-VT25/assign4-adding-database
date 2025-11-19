# 📘 Addition Storage API (Assignment 4)

This project provides a simple REST API for storing numeric values under named keys, retrieving stored data, and appending new values.
Originally implemented with an in-memory store with Map in assignment 3, the project has now been refactored to use **SQL Server** for persistent storage.


---
## Running with Docker (API + MS SQL + SQLPad)

This project is fully dockerized and uses MS SQL Server as the persistence layer.

### Prerequisites
- Docker and Docker Compose installed

### Start the stack

```bash
docker compose up --build
```

---

# 🚀 Endpoints Overview

## ### **POST `/storage/:key`**

Append one or more numeric values to a given key.
If the key does not exist, it is automatically created.

### **Example Request**

```
POST http://localhost:3000/storage/enteredNumbers

{
  "value": ["10", 23]
}
```

### **Example Response (201 Created or 200 OK)**

```json
{
  "key": "enteredNumbers",
  "values": [35, 10, 23],
  "sum": 68,
  "appended": [10, 23]
}
```

### Behavior

* Converts all inputs to numeric values.
* Invalid numbers are ignored.
* Returns:

  * All stored values
  * Updated sum
  * Appended values

---

## ### **GET `/storage/:key`**

Retrieve all values for a given key.

### **Example Success (200 OK)**

```json
{
  "key": "enteredNumbers",
  "values": [35, 10, 23],
  "sum": 68
}
```

### **Example Error (404 Not Found)**

```json
{
  "error": "Not found",
  "key": "enteredNumbers_3"
}
```

---

## ### **GET `/storage`**

Return all stored keys along with their values and sums.

### **Example Response**

```json
{
  "count": 2,
  "data": {
    "enteredNumbers": {
      "values": [35, 10, 23],
      "sum": 68
    },
    "enteredNumbers_2": {
      "values": [1, 2, 4, 10, -3],
      "sum": 14
    }
  }
}
```

---

# 🗄️ Database Design (Updated Implementation)

The API now uses **SQL Server** instead of an in-memory Map.
Two tables are used:

## ### **Table: `buckets`**

Stores metadata for each key.

| Column       | Type             | Description                        |
| ------------ | ---------------- | ---------------------------------- |
| `key`        | NVARCHAR(256) PK | Identifier used in the API         |
| `sum`        | DECIMAL(18,4)    | Running total of all stored values |
| `created_at` | DATETIME2        | Row creation timestamp             |
| `updated_at` | DATETIME2        | Last modified timestamp            |

---

## ### **Table: `bucket_entries`**

Stores individual numeric values appended to a key.

| Column       | Type             | Description                          |
| ------------ | ---------------- | ------------------------------------ |
| `id`         | INT IDENTITY PK  | Auto-incrementing identifier         |
| `key`        | NVARCHAR(256) FK | Foreign key mapping to `buckets.key` |
| `value`      | DECIMAL(18,4)    | Stored numeric value                 |
| `created_at` | DATETIME2        | Insert timestamp                     |

---

# 🔄 Transaction Flow (How Writes Work)

When handling a `POST /storage/:key` request:

1. **Begin SQL Transaction**
2. **Ensure bucket exists**

   ```sql
   IF NOT EXISTS (...) INSERT ...
   ```
3. **Insert each numeric value** into `bucket_entries`
4. **Update the bucket sum** in `buckets.sum`
5. **Commit the transaction**
6. On any error → **Rollback**

This guarantees atomic updates and prevents partial writes.

---

# 🧪 Testing

The project includes **unit tests for utility functions and database logic** using:

* **Vitest** for test execution
* **Mocked SQL Server (`mssql`) objects** to avoid real DB connections
* **Mocked `getDb()` pool**, `Transaction`, and `Request` objects
* Full coverage of:

  * Value normalization
  * In-memory bucket logic
  * Database write/read flow
  * Transaction commit/rollback behavior

---

# 🤖 AI Disclosure

Parts of this project—specifically:

* Unit test generation
* Mocking strategy for SQL Server
* Test structure and refactoring help
* This README
