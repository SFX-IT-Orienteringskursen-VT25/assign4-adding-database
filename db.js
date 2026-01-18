const sql = require('mssql');

const config = {
  user: 'SA',
  password: 'root',
  server: 'localhost',
  database: 'TestDb',
  options: {
    encrypt: false,
    trustServerCertificate: true
  }
};

const poolPromise = sql.connect(config)
  .then(pool => {
    console.log('Connected to MSSQL');
    return pool;
  })
  .catch(err => console.log('Database Connection Failed! ', err));

module.exports = {
  sql, poolPromise
};