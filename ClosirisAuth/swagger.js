const swaggerAutogen = require('swagger-autogen')();

const doc = {
    info: {
        title: "Closiris Auth API",
        description: "API for Closiris Auth",
    },
    host: "localhost:3001",
};

const outputFile = './swagger-output.json';
const routes = ['./app.js'];

swaggerAutogen(outputFile, routes, doc);