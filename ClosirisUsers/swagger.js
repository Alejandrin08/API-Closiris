const swaggerAutogen = require('swagger-autogen')();

const doc = {
    info: {
        title: "Closiris Users API",
        description: "API for Closiris Users",
    },
    host: "localhost:3000",
};

const outputFile = './swagger-output.json';
const routes = ['./app.js'];

swaggerAutogen(outputFile, routes, doc);