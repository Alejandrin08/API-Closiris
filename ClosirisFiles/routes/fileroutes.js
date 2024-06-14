const express = require('express');
const multer = require('multer');
const router = express.Router();
const fileController = require('../controllers/filecontroller');
const Authorize = require('../middlewares/authmiddleware');
const file = require('../models/file');
const { check, validationResult } = require('express-validator');

const storage = multer.memoryStorage();
const upload = multer({ 
    storage: storage,
    limits: {
        fileSize: 4 * 1024 * 1024
    },
    fileFilter: (req, file, cb) => {
        const allowedTypes = [
            'application/pdf', 
            'application/vnd.openxmlformats-officedocument.wordprocessingml.document', 
            'text/plain',
            'text/csv', 
            'audio/mpeg', 
            'video/mp4', 
            'image/jpeg', 
            'image/jpg',
            'image/png',
            'image/gif',
        ];
        if (allowedTypes.includes(file.mimetype)) {
            cb(null, true);
        } else {
            cb(new Error('Tipo de archivo no permitido'));
        }
    }
});

// POST: api/files/file

router.post('/file',[
    check('folder_name').matches(/^(?!Compartidos\b)[\w\-]+$/).withMessage('Please provide a valid folder name ')
    ], Authorize('Premium,Básico'), upload.single('file'), fileController.insertFile);
//POST: api/files/fileOwner

router.post('/fileOwner', [
    check('file_id').isInt().withMessage('File id is requiered')
], Authorize('Premium,Básico'), fileController.insertFileOwner);
//POST: api/files/fileShared

router.post('/fileShared',[
    check('file_id').isInt().withMessage('File id is requiered'),
    check('shared_id').isInt().withMessage('Share id is requiered')
], Authorize('Premium'), fileController.insertFileShared);

//GET: api/files/getFile/:id

router.get('/getFile', [
    check('file_id').isInt().withMessage('File id is requiered')
],Authorize('Premium,Básico'),  fileController.getFileFromServer);
//Delete: api/files/deleteFile/:id

router.delete('/deleteFileServer',[
    check('file_id').isInt().withMessage('File id is requiered')
], Authorize('Premium,Básico'),  fileController.deleteFilefromServer);
//DELETE: api/files/deleteFileShared

router.delete('/deleteFileShared',[
    check('file_id').isInt().withMessage('File id is requiered')
], Authorize('Premium,Básico') ,fileController.deletefileShared);
//DELETE: api/files/deleteFileRegistration

router.delete('/deleteFile',[
    check('file_id').isInt().withMessage('File id is requiered')
], Authorize('Premium,Básico'), fileController.deleteFileRegistration);
//GET: api/users/getUsersShareFile

router.get('/getUsersShareFile', [
    check('file_id').isInt().withMessage('File id is requiered')
], Authorize('Premium,Básico'), fileController.getUsersShareFile );
//GET: api/users/getUsersShareFile

router.get('/getUsersOwnerFile', [
    check('file_id').isInt().withMessage('File id is requiered')
],Authorize('Premium,Básico'), fileController.getUsersOwnerFile );
//GET: api/files/getFoldersByUser

router.get('/getFoldersByUser',Authorize('Premium,Básico'), fileController.getFoldersByUser);
//GET: api/files/getListOfFileInfoByUser

router.get('/getListOfFileInfoByUser', [
    check('folder_name').matches(/^(?!Compartidos\b)[\w\-]+$/).withMessage('Please provide a valid folder name ')
    ],  Authorize('Premium,Básico'), fileController.getListOfFileInfoByUser);


router.get('/getListOfFileSharedByUser',Authorize('Premium,Básico'), fileController.getListOfFileSharedByUser);
module.exports = router;