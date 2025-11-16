import express from "express";
import cors from "cors";
import { appendNumbers, getOrInitBucket } from "./utils.js";
import { getDb } from './db.js';
import { appendNumbersDb, getBucketDb, getAllBucketsDb } from "./storage.js";

const app = express();
app.use(cors());
app.use(express.json());

getDb().catch((err) => {
  console.error('MSSQL connection failed:', err);
  process.exit(1);
});

app.get('/db-version', async (req, res) => {
  try {
    const pool = await getDb();
    const result = await pool.request().query('SELECT @@VERSION AS version');
    res.json(result.recordset[0]);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

app.get("/storage", async (_req, res) => {
  try {
    const { count, data } = await getAllBucketsDb();
    res.status(200).json({ count, data });
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});


app.get("/storage/:key", async (req, res) => {
  const { key } = req.params;
  try {
    const bucket = await getBucketDb(key);
    if (!bucket) return res.status(404).json({ error: "Not found", key });
    res.status(200).json(bucket);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

app.post("/storage/:key", async (req, res) => {
  const { key } = req.params;
  const { value } = req.body;

  if (typeof value === "undefined") {
    return res.status(400).json({ error: 'Missing "value" in request body' });
  }

  try {
    const result = await appendNumbersDb(key, value);
    if (result.error) return res.status(400).json({ error: result.error });
    res.status(200).json(result);
  } catch (e) {
    res.status(500).json({ error: e.message });
  }
});

const PORT = process.env.PORT || 4000;
app.listen(PORT, () => console.log(`API on http://localhost:${PORT}`));
