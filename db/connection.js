const sql = require('mssql');

const dbConfig = {
    server: 'localhost',
    database: 'master',
    user: 'sa',
    password: 'YourPassword123!',
    port: 1433,
    options: {
        enableArithAbort: true,
        trustServerCertificate: true,
        encrypt: false
    },
    pool: {
        max: 10,
        min: 0,
        idleTimeoutMillis: 30000
    }
};

let pool;

const getPool = async () => {
    if (!pool) {
        pool = new sql.ConnectionPool(dbConfig);
        await pool.connect();
        console.log('Connected to SQL Server');
        
        // Initialize database and table
        await initializeDatabase();
    }
    return pool;
};

const initializeDatabase = async () => {
    try {
        const request = pool.request();
        
        // Create database if it doesn't exist
        await request.query(`
            IF NOT EXISTS(SELECT name FROM master.dbo.sysdatabases WHERE name = 'StorageDB')
            CREATE DATABASE StorageDB
        `);
        
        // Switch to StorageDB
        await request.query('USE StorageDB');
        
        // Create table if it doesn't exist
        await request.query(`
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Storage' and xtype='U')
            CREATE TABLE Storage (
                id INT IDENTITY(1,1) PRIMARY KEY,
                [key] NVARCHAR(255) NOT NULL UNIQUE,
                value NVARCHAR(MAX) NOT NULL,
                created_at DATETIME2 DEFAULT GETDATE(),
                updated_at DATETIME2 DEFAULT GETDATE()
            )
        `);
        
        console.log('Database initialized successfully');
    } catch (error) {
        console.error('Database initialization error:', error);
    }
};

module.exports = { getPool, sql };