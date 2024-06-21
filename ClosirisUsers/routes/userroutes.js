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

router.post('/userAccount', [
  check('email').isEmail().withMessage('Please provide a valid email address'),
  check('password').matches(/^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[\W_]).{5,15}$/).withMessage('Password must comply with a password policy '),
  check('name').matches(/^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ\s]*$/).withMessage('Name is required'),
  check('imageProfile').optional()
  ], uploadImage, userAccount.createUserAccount);


router.post('/user', [
  check('email').isEmail().withMessage('Please provide a valid email address'),
  check('freeStorage').isDecimal().withMessage('Free storage must be decimal number'),
  check('plan').matches(/^(Básico|Premium|Administrador)$/).withMessage('Please provide a valid plan')
 ], userAccount.createUser);


router.put('/userAccount', [
  check('email').isEmail().withMessage('Please provide a valid email address'),
  check('name').matches(/^[a-zA-ZáéíóúÁÉÍÓÚüÜñÑ\s]*$/).withMessage('Name is required'),
  check('imageProfile').optional().isBase64().withMessage('Image profile must be a base64 string')
  ], Authorize('Premium,Básico'), userAccount.updateUserAccount);


router.patch('/password', [
  check('email').isEmail().withMessage('Please provide a valid email address'),
  check('password').matches(/^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[\W_]).{5,15}$/).withMessage('Password must comply with a password policy ')
 ], userAccount.updatePassword);


router.patch('/plan', [
  check('freeStorage').isDecimal().withMessage('Free storage must be decimal number'),
  check('plan').matches(/^(Básico|Premium)$/).withMessage('Please provide a valid plan')
 ], Authorize('Premium,Básico'), userAccount.updatePlan);


router.patch('/freeStorage',[
  check('freeStorage').isDecimal().withMessage('Free storage must be decimal number')
 ], Authorize('Administrador,Premium,Básico'), userAccount.updateFreeStorage);


router.get('/validateEmailDuplicity/:email', [
  check('email').isEmail().withMessage('Please provide a valid email address')
  ], userAccount.validateEmailDuplicity);


router.get('/userInfoById', Authorize('Premium,Básico'), userAccount.getUserInfoById);


router.get('/infoByEmail/:email',  [
  check('email').isEmail().withMessage('Please provide a valid email address')
  ], Authorize('Premium,Básico'), userAccount.getUserInfoByEmail);

  
router.get('/listUsers', Authorize('Administrador'), userAccount.getListUsers);

module.exports = router;