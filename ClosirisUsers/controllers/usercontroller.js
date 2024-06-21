const ClaimTypes = require('../config/claimtypes');
const initModels = require('../models/init-models');
const sequelize = require('../models/sequelize');
const models = initModels(sequelize);
const bcrypt = require('bcrypt');
const { validationResult } = require('express-validator');
const UserAccount = models.userAccount;
const User = models.user;
const { Op, Sequelize } = require('sequelize');

let self = {};

self.createUserAccount = async function (req, res) {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
        return res.status(400).json({ errors: errors.array() });
    }
    try {
        const hashedPassword = await bcrypt.hash(req.body.password, 10);

        let imageBuffer = null;
        if (req.body.imageProfile) {
            imageBuffer = Buffer.from(req.body.imageProfile, 'base64');
        }

        let data = await UserAccount.create({
            email: req.body.email,
            password: hashedPassword,
            name: req.body.name,
            imageProfile: imageBuffer
        });
        console.log(data);

        return res.status(201).json({ user: data });
    } catch (error) {
        console.error(error);
        return res.status(400).json({ message: "Error creating user", error });
    }
};

self.createUser = async function (req, res) {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
        return res.status(400).json({ errors: errors.array() });
    }
    try {
        let data = await User.create({
            email: req.body.email,
            plan: req.body.plan,
            freeStorage: req.body.freeStorage,
        });

        return res.status(201).json({ user: data });
    } catch (error) {
        return res.status(400).json(error);
    }
}

self.updateUserAccount = async function (req, res) {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
        return res.status(400).json({ errors: errors.array() });
    }
    const userId = req.decoded[ClaimTypes.Id];
    const { email, name, imageProfile } = req.body;

    try {
        let user = await UserAccount.findByPk(userId);
        if (!user) {
            return res.status(404).json({ message: "User not found" });
        } else {
            user.email = email;
            user.name = name;
            user.imageProfile = Buffer.from(imageProfile, 'base64');
            await user.save();

            req.Audit("ActualizarCuenta", user.id);

            return res.status(200).json({ message: "User updated successfully", user: user });
        }
    } catch (error) {
        return res.status(400).json(error);
    }
};

self.updatePassword = async function (req, res) {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
        return res.status(400).json({ errors: errors.array() });
    }
    const { email, password } = req.body;

    try {
        const hashedPassword = await bcrypt.hash(password, 10);
        let user = await UserAccount.findOne({ where: { email: email } });
        if (!user) {
            return res.status(404).json({ message: "User not found" });
        } else {
            user.password = hashedPassword;
            await user.save();

            return res.status(200).json({ message: "Password updated successfully" });
        }

    } catch (error) {
        return res.status(400).json(error);
    }
};

self.updatePlan = async function (req, res) {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
        return res.status(400).json({ errors: errors.array() });
    }
    const userId = req.decoded[ClaimTypes.Id];
    const { plan, freeStorage } = req.body;

    try {
        let user = await User.findByPk(userId);
        if (!user) {
            return res.status(404).json({ message: "User not found" });
        } else {
            user.plan = plan;
            user.freeStorage = freeStorage;
            await user.save();

            return res.status(200).json(user);
        }
    } catch (error) {
        return res.status(400).json(error);
    }
};

self.updateFreeStorage = async function (req, res) {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
        return res.status(400).json({ errors: errors.array() });
    }
    const userId = req.decoded[ClaimTypes.Id];
    const { freeStorage } = req.body;

    try {
        let user = await User.findByPk(userId);
        if (!user) {
            return res.status(404).json({ message: "User not found" });
        } else {
            user.freeStorage = freeStorage;
            await user.save();
            return res.status(200).json(user.freeStorage);
        }

    } catch (error) {
        return res.status(400).json(error);
    }
};

self.validateEmailDuplicity = async function (req, res) {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
        return res.status(400).json({ errors: errors.array() });
    }
    const email = req.params.email;
    try {
        let user = await UserAccount.findOne({ where: { email: email } });
        if (user) {
            return res.status(200).json({ user: user.email });
        } else {
            return res.status(200).json({ message: "Email available" });
        }
    } catch (error) {
        return res.status(400).json(error);
    }
};

self.getUserInfoById = async function (req, res) {
    const userId = req.decoded[ClaimTypes.Id];
    try {
        const userAccount = await UserAccount.findByPk(userId, {
            include: [
                {
                    model: User,
                    attributes: ['plan', 'freeStorage'],
                    as: 'user'
                }
            ],
            attributes: ['email', 'name', 'imageProfile'],
        });

        if (userAccount) {
            const user = userAccount.user; 
            const userData = {
                email: userAccount.email,
                name: userAccount.name,
                plan: user.plan,
                freeStorage:user.freeStorage,
                imageProfile: userAccount.imageProfile ? userAccount.imageProfile.toString('base64') : null
            };
            console.log(userData);
            return res.status(200).json(userData);
        } else {
            return res.status(404).json({ message: "User not found" });
        }

    } catch (error) {
        console.error(error); 
        return res.status(400).json(error);
    }
};

self.getUserInfoByEmail = async function (req, res) {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
        return res.status(400).json({ errors: errors.array() });
    }
    const emailUser = req.params.email;
    try {

        const userAccount = await UserAccount.findOne({
            attributes: ['id', 'name'],
            where: {
                email: emailUser
            }
        });
        if (userAccount) {
            const userData = {
                id: userAccount.id,
                name: userAccount.name
            };

            return res.status(200).json(userData);
        } else {
            return res.status(404).json({ message: "User not found" });

        }
    } catch (error) {
        console.error('Error:', error);
        return res.status(500).json({ error: 'Error al procesar la solicitud' });
    }
}

self.getListUsers = async function (req, res) {
    try {

        const users = await User.findAll({
            attributes: ['email', 'plan', 'freeStorage'],
            where: {
                plan: {
                    [Op.ne]: 'Administrador'
                }
            },
            include: [{
                model: UserAccount,
                as: 'userAccount',
                attributes: ['name'],
                required: true,
                email: {
                    [Op.eq]: sequelize.col('userAccount.email')
                }
            }]
        });
        if (users) {
            const transformedResult = users.map(user => {
                const userData = user.get({ plain: true });
                return {
                    name: userData.userAccount.name,
                    email: userData.email,
                    plan: userData.plan,
                    freeStorage: userData.freeStorage
                };
            });

            return res.status(201).json(transformedResult);
        } else {
            return res.status(404).json({ message: "User not found" });

        }
    } catch (error) {
        console.error('Error:', error);
        return res.status(500).json({ error: 'Error al procesar la solicitud' });
    }
}

module.exports = self;