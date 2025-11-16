const request = require('supertest');
const app = require('../server');

describe('Storage API Endpoints', () => {
    beforeEach(() => {
        // Reset storage before each test
        Object.keys(require('../routes/storage').storage).forEach(key => {
            delete require('../routes/storage').storage[key];
        });
    });

    describe('POST /api/storage/:key (setItem equivalent)', () => {
        test('should create new item and return 201', async () => {
            const response = await request(app)
                .post('/api/storage/testKey')
                .send({ value: 'testValue' })
                .expect(201);
            
            expect(response.body.key).toBe('testKey');
            expect(response.body.value).toBe('testValue');
        });

        test('should update existing item and return 200', async () => {
            await request(app).post('/api/storage/testKey').send({ value: 'initialValue' });
            
            const response = await request(app)
                .post('/api/storage/testKey')
                .send({ value: 'updatedValue' })
                .expect(200);
            
            expect(response.body.value).toBe('updatedValue');
        });

        test('should return 400 when value is missing', async () => {
            const response = await request(app)
                .post('/api/storage/testKey')
                .send({})
                .expect(400);
            
            expect(response.body.error).toBe('Bad Request');
        });
    });

    describe('GET /api/storage/:key (getItem equivalent)', () => {
        test('should retrieve existing item and return 200', async () => {
            await request(app).post('/api/storage/testKey').send({ value: 'testValue' });
            
            const response = await request(app)
                .get('/api/storage/testKey')
                .expect(200);
            
            expect(response.body.value).toBe('testValue');
        });

        test('should return 404 for non-existent key', async () => {
            const response = await request(app)
                .get('/api/storage/nonExistentKey')
                .expect(404);
            
            expect(response.body.error).toBe('Key not found');
        });
    });
});