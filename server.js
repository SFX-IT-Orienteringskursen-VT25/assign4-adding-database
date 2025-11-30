
import express from "express";
import bodyParser from "body-parser";
import sql from "mssql";

const app = express();
app.use(bodyParser.json());

// SQL Server configuration
const dbConfig = {
  user: "nodeuser",
  password: "password123",
  server: "localhost",
  port: 59496,
  database: "AssignmentDB",
  options: {
    encrypt: false,
    trustServerCertificate: true,
  }
};

let pool;

// Connect to the database
async function connectToDb() {
  try {
    pool = await sql.connect(dbConfig);
    console.log("Connected to SQL Server");
  } catch (err) {
    console.error("Database connection failed:", err);
  }
}

connectToDb();

// GET /numbers → return all numbers
app.get("/numbers", async (req, res) => {
  try {
    const result = await pool.request().query("SELECT * FROM Numbers");
    res.json(result.recordset);
  } catch (err) {
    res.status(500).send("Error fetching numbers");
  }
});

// POST /numbers → add a number
app.post("/numbers", async (req, res) => {
  const { value } = req.body;

  if (typeof value !== "number") {
    return res.status(400).send("Value must be a number");
  }

  try {
    await pool
      .request()
      .input("value", sql.Int, value)
      .query("INSERT INTO Numbers (value) VALUES (@value)");

    res.status(201).send("Number added");
  } catch (err) {
    res.status(500).send("Error saving number");
  }
});

// DELETE /numbers/:id → delete a number
app.delete("/numbers/:id", async (req, res) => {
  const id = req.params.id;

  try {
    const result = await pool
      .request()
      .input("id", sql.Int, id)
      .query("DELETE FROM Numbers WHERE id = @id");

    if (result.rowsAffected[0] === 0) {
      return res.status(404).send("Number not found");
    }

    res.send("Number deleted");
  } catch (err) {
    res.status(500).send("Error deleting number");
  }
});

const PORT = 3000;
app.listen(PORT, () => {
  console.log(`API running at http://localhost:${PORT}`);
});