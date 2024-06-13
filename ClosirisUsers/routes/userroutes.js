const express = require('express');
const router = express.Router();
const multer = require('multer');
const userAccount = require('../controllers/usercontroller');
const Authorize = require('../middlewares/authmiddleware');

const upload = multer({
  limits: { fileSize: 4 * 1024 * 1024 }, 
});

const uploadImage = upload.single('imageProfile');

// POST: api/users/userAccount
router.post('/userAccount', uploadImage, userAccount.createUserAccount);
// POST: api/users/user
router.post('/user', userAccount.createUser);
// PUT: api/users/putUserAccount
router.put('/putUserAccount', Authorize('Premium,Básico'), userAccount.updateUserAccount);
// PATCH: api/users/patchPassword
router.patch('/patchPassword', userAccount.updatePassword);
// PATCH: api/users/patchPlan
router.patch('/patchPlan', Authorize('Premium,Básico'), userAccount.updatePlan);
// PATCH: api/users/patchFreeStorage
router.patch('/patchFreeStorage', Authorize('Administrador,Premium,Básico'), userAccount.updateFreeStorage);
// GET: api/users/validateEmailDuplicity
router.get('/validateEmailDuplicity/:email', userAccount.validateEmailDuplicity);
// GET: api/users/GetUserInfoById
router.get('/GetUserInfoById', Authorize('Premium,Básico'), userAccount.getUserInfoById);
// GET: api/users/GetInfoByEmail
router.get('/GetInfoByEmail/:email', Authorize('Premium,Básico'), userAccount.getUserInfoByEmail);
// GET: api/users/GetListUsers
router.get('/GetListUsers',Authorize('Administrador'), userAccount.getListUsers);

module.exports = router;