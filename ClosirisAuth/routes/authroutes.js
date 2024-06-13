const router = require('express').Router();
const authcontroller = require('../controllers/authcontroller');
const Authorize = require('../middlewares/authmiddleware');

//POST: api/users
router.post('/', authcontroller.login);
//GET: api/users/time
router.get('/time', authcontroller.getTime);
//GET: api/GetListAudit
router.get('/GetListAudit',Authorize('Administrador'), authcontroller.getListAudit);

module.exports = router;