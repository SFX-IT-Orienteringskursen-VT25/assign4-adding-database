const sql = require('mssql');

const config = {
  user: 'sa',
  password: 'YourStrong@Passw0rd1', // replace with your Docker SA password
  server: 'localhost',
  port: 1433,
  database: 'master', // can create your own DB later
  options: {
    encrypt: false,
    trustServerCertificate: true
  }
};

async function testConnection() {
  try {
    await sql.connect(config);
    const result = await sql.query`SELECT @@VERSION as version`;
    console.log('Connected to SQL Server!');
    console.log(result.recordset);
  } catch (err) {
    console.error('Connection failed:', err);
  } finally {
    sql.close();
  }
}

testConnection();
