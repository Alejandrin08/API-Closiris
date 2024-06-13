const express = require('express');
const cors = require("cors");
const dotenv = require('dotenv');
const app = express();

const swaggerUi = require('swagger-ui-express');
const swaggerFile = require('./swagger-output.json');

dotenv.config();

app.use(express.json());
app.use(express.urlencoded({ extended: false }));

var corsOptions = {
    origin: ["http://localhost:5000", "http://localhost:50051"],
    methods: "GET,PUT,PATCH,POST,DELETE",
};
app.use(cors(corsOptions));

app.use(require("./middlewares/auditmiddleware"))

app.use("/api/files", require('./routes/fileroutes'));
app.use("/swagger", swaggerUi.serve, swaggerUi.setup(swaggerFile));
app.get('*', (req, res) => { res.status(404).send('404 Not Found');});

const errorlogger = require('./middlewares/errorloggermiddleware');
const errorhandler = require('./middlewares/errorhandlermiddleware');
app.use(errorlogger, errorhandler);

app.listen(process.env.SERVER_PORT, () => {
    console.log(`Server is running on port ${process.env.SERVER_PORT}`);
    console.log(`http://localhost:${process.env.SERVER_PORT}`);
});