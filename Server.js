const express = require('express');
const sql = require('mssql');

const app = express();
app.use(express.json());

// SQL Server config
const dbConfig = {
  user: 'sa',
  password: 'P@ssw0rd!123',  // must match docker-compose password
  server: 'localhost',
  database: 'AdditionDB',
  options: {
    trustServerCertificate: true
  }
};

// Helper function to create DB and table if they don't exist
async function initializeDB() {
  try {
    // Connect to master first to create DB if needed
    const pool = await sql.connect({
      ...dbConfig,
      database: 'master' // connect to master first
    });

    // Create database if it doesn't exist
    await pool.request().query(`
      IF DB_ID('AdditionDB') IS NULL
      BEGIN
        CREATE DATABASE AdditionDB;
      END
    `);

    // Connect to AdditionDB
    await sql.close();
    const dbPool = await sql.connect(dbConfig);

    // Create Numbers table if not exists
    await dbPool.request().query(`
      IF OBJECT_ID('Numbers', 'U') IS NULL
      CREATE TABLE Numbers (
        Id INT IDENTITY PRIMARY KEY,
        Num1 INT,
        Num2 INT
      );
    `);

    console.log('Database and table ready.');
  } catch (err) {
    console.error('DB Initialization Error:', err);
  }
}

// POST endpoint to store two numbers
app.post('/add', async (req, res) => {
  const { num1, num2 } = req.body;

  if (num1 == null || num2 == null) {
    return res.status(400).json({ error: 'num1 and num2 are required' });
  }

  try {
    const pool = await sql.connect(dbConfig);
    await pool.request()
      .input('num1', sql.Int, num1)
      .input('num2', sql.Int, num2)
      .query('INSERT INTO Numbers (Num1, Num2) VALUES (@num1, @num2)');

    res.status(201).json({ message: 'Numbers saved successfully' });
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

// GET endpoint to retrieve last numbers and sum
app.get('/add', async (req, res) => {
  try {
    const pool = await sql.connect(dbConfig);
    const result = await pool.request()
      .query('SELECT TOP 1 * FROM Numbers ORDER BY Id DESC');

    if (result.recordset.length === 0) {
      return res.status(404).json({ error: 'No numbers found' });
    }

    const row = result.recordset[0];
    res.json({
      num1: row.Num1,
      num2: row.Num2,
      sum: row.Num1 + row.Num2
    });
  } catch (err) {
    res.status(500).json({ error: err.message });
  }
});

// Start server
const PORT = 3000;
app.listen(PORT, async () => {
  await initializeDB();
  console.log(`Server running at http://localhost:${PORT}`);
});