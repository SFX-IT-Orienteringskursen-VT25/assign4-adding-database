import express from "express";
import cors from "cors";
import { appendNumbers, getOrInitBucket } from "./utils.js";

const app = express();
app.use(cors());
app.use(express.json());

const store = new Map();

app.get("/storage", (req, res) => {
  const data = Object.fromEntries(store.entries());
  res.status(200).json({ count: store.size, data });
});


app.get("/storage/:key", (req, res) => {
  const { key } = req.params;
  if (!store.has(key)) {
    return res.status(404).json({ error: "Not found", key });
  }
  const bucket = store.get(key); 
  res.status(200).json({ key, ...bucket });
});

app.post("/storage/:key", (req, res) => {
  const { key } = req.params;
  const { value } = req.body;

  if (typeof value === "undefined") {
    return res.status(400).json({ error: 'Missing "value" in request body' });
  }

  const existed = store.has(key);
  const result = appendNumbers(store, key, value);

  if (result.error) {
    return res.status(400).json({ error: result.error });
  }

  const { bucket, appended } = result; 
  res.status(existed ? 200 : 201).json({
    key,
    values: bucket.values,
    sum: bucket.sum,
    appended
  });
});

const PORT = process.env.PORT || 3000;
app.listen(PORT, () => console.log(`API on http://localhost:${PORT}`));
