const initModels = require('../models/init-models');
const sequelize = require('../models/sequelize');
const models = initModels(sequelize);
const UserAccount = models.userAccount;
const User = models.user;
const Audit = models.audit;
const bcrypt = require('bcrypt');
const { GenerateToken, VerifyToken } = require('../services/jwttokenservice');
const { validationResult } = require('express-validator');

let self = {};
//POST: api/auth/
self.login = async function (req, res) {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
        return res.status(400).json({ errors: errors.array() });
    }
    try {
        let data = await UserAccount.findOne({
            where: { email: req.body.email },
            include: [{
                model: User,
                as: 'user',
                attributes: []
            }],
            raw: true
        });

        if (!data) {
            return res.status(401).json({ message: "Usuario o contraseña incorrectos" });
        }

        const userData = await User.findOne({
            where: { email: req.body.email },
            attributes: ['plan'],
            raw: true
        });

        const passwordMatch = await bcrypt.compare(req.body.password, data.password);
        if (!passwordMatch) {
            return res.status(401).json({ message: "Usuario o contraseña incorrectos" });
        }

        const token = GenerateToken(data.email, data.id, data.name, userData ? userData.plan : 'Sin rol');

        req.Audit("Login", data.id);

        return res.status(200).json({
            email: data.email,
            name: data.name,
            role: userData ? userData.plan : 'Sin rol',
            token: token
        });
    } catch (error) {
        console.error(error);
        return res.status(500).json({ message: "Error" });
    }
};

//GET: api/auth/time
self.getTime = async function (req, res) {
    const time = VerifyToken(req);
    if (time == null)
        return res.status(404).send("null")
    return res.status(200).send(time)
}

//GET: api/GetListAudit
self.getListAudit = async function (req, res){
    try {    

        const audits = await Audit.findAll();
        if (audits) {

            return res.status(201).json( audits );
        } else {
            return res.status(404).json({ message: "User not found" });
        
        }
    } catch (error) {
        console.error('Error:', error);
        return res.status(500).json({ error: 'Error al procesar la solicitud' });
    }
}

module.exports = self;