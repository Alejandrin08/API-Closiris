const jwt = require('jsonwebtoken')
const initModels = require('../models/init-models');
const sequelize = require('../models/sequelize');
const models = initModels(sequelize);
const requestIp = require('request-ip');
const ClaimTypes = require('../config/claimtypes')
const Audit = models.audit;

const auditlogger = (req, res, next) => {
    const ip = requestIp.getClientIp(req)
    let email = 'invitado'

    req.Audit = async (action, id) => {
        const authHeader = req.header('Authorization')
        if (authHeader) {
            if (authHeader.startsWith('Bearer ')) {
                const token = authHeader.split(' ')[1]
                const decodedToken = jwt.decode(token)
                email = decodedToken[ClaimTypes.Name] ?? id
                id = decodedToken[ClaimTypes.Id] ?? id
            }
        }

        await Audit.create({
            idUser: id, action: action, user: email, ip: ip, initDate: new Date()
        })
    }
    next()
}

module.exports = auditlogger