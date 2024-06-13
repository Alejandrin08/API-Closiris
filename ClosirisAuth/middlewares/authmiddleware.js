const jwt = require('jsonwebtoken');
const jwtsecret = process.env.JWT_SECRET;
const ClaimTypes = require('../config/claimtypes');

const Authorize = (rol) => {
    return async (req, res, next) => {
        try {
            const authHeader = req.header('Authorization');
            if (!authHeader.startsWith('Bearer ')) {
                return res.status(401).json({ message: "Unauthorized" });
            }

            const token = authHeader.split(' ')[1];

            const decoded = jwt.verify(token, jwtsecret);

            if (rol.split(',').indexOf(decoded[ClaimTypes.Role]) === -1) {
                return res.status(403).json({ message: "Forbidden" });
            }

            const restMinutes = (decoded.exp - (Date.now() / 1000)) / 60;
            if (restMinutes < 5) {
                res.setHeader("Set-Authorization", authHeader);
            }

            req.decoded = decoded;

            return next();
        } catch (error) {
            return res.status(401).json({ message: "Unauthorized" });
        }
    };
};

module.exports = Authorize;
