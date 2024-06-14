const express = require('express');
const router = express.Router();
const multer = require('multer');
const userAccount = require('../controllers/usercontroller');
const Authorize = require('../middlewares/authmiddleware');
const { check, validationResult } = require('express-validator');

const upload = multer({
  limits: { fileSize: 4 * 1024 * 1024 }, 
});

const uploadImage = upload.single('imageProfile');

// POST: api/users/userAccount
router.post('/userAccount', [
  check('email').isEmail().withMessage('Please provide a valid email address'),
  check('password').matches(/^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[\W_]).{5,15}$/).withMessage('Password must comply with a password policy '),
  check('name').matches(/^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ\s]*$/).withMessage('Name is required'),
  check('imageProfile').optional().isBase64().withMessage('Image profile must be a base64 string')
  ], uploadImage, userAccount.createUserAccount);
// POST: api/users/user
router.post('/user', [
  check('email').isEmail().withMessage('Please provide a valid email address'),
  check('freeStorage').isDecimal().withMessage('Free storage must be decimal number'),
  check('plan').matches(/^(Básico|Premium|Administrador)$/).withMessage('Please provide a valid plan')
 ], userAccount.createUser);
// PUT: api/users/putUserAccount
router.put('/putUserAccount', [
  check('email').isEmail().withMessage('Please provide a valid email address'),
  check('name').matches(/^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ\s]*$/).withMessage('Name is required'),
  check('imageProfile').optional().isBase64().withMessage('Image profile must be a base64 string')
  ], Authorize('Premium,Básico'), userAccount.updateUserAccount);
// PATCH: api/users/patchPassword
router.patch('/patchPassword', [
  check('email').isEmail().withMessage('Please provide a valid email address'),
  check('password').matches(/^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[\W_]).{5,15}$/).withMessage('Password must comply with a password policy ')
 ], userAccount.updatePassword);
// PATCH: api/users/patchPlan
router.patch('/patchPlan', [
  check('freeStorage').isDecimal().withMessage('Free storage must be decimal number'),
  check('plan').matches(/^(Básico|Premium)$/).withMessage('Please provide a valid plan')
 ], Authorize('Premium,Básico'), userAccount.updatePlan);
// PATCH: api/users/patchFreeStorage
router.patch('/patchFreeStorage',[
  check('freeStorage').isDecimal().withMessage('Free storage must be decimal number')
 ], Authorize('Administrador,Premium,Básico'), userAccount.updateFreeStorage);
// GET: api/users/validateEmailDuplicity
router.get('/validateEmailDuplicity/:email', [
  check('email').isEmail().withMessage('Please provide a valid email address')
  ], userAccount.validateEmailDuplicity);
// GET: api/users/GetUserInfoById
router.get('/GetUserInfoById', Authorize('Premium,Básico'), userAccount.getUserInfoById);
// GET: api/users/GetInfoByEmail
router.get('/GetInfoByEmail/:email',  [
  check('email').isEmail().withMessage('Please provide a valid email address')
  ], Authorize('Premium,Básico'), userAccount.getUserInfoByEmail);
// GET: api/users/GetListUsers
router.get('/GetListUsers', Authorize('Administrador'), userAccount.getListUsers);

module.exports = router;