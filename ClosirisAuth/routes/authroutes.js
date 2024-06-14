const router = require('express').Router();
const authcontroller = require('../controllers/authcontroller');
const Authorize = require('../middlewares/authmiddleware');
const { check, validationResult } = require('express-validator');

//POST: api/users
router.post('/', [
    check('email').isEmail().withMessage('Please provide a valid email address'),
    check('password').matches(/^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[\W_]).{5,15}$/).withMessage('Password must comply with a password policy '),
    ], authcontroller.login);
//GET: api/users/time
router.get('/time', authcontroller.getTime);
//GET: api/GetListAudit
router.get('/GetListAudit',Authorize('Administrador'), authcontroller.getListAudit);

module.exports = router;