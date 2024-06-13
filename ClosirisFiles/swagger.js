const swaggerAutogen = require('swagger-autogen')();

const doc = {
    info: {
        title: "Closiris Users API",
        description: "API for Closiris Files",
    },
    host: "localhost:3002",
};

const outputFile = './swagger-output.json';
const routes = ['./app.js'];

swaggerAutogen(outputFile, routes, doc);