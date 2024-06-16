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

router.post('/file',[
    check('folder_name').matches(/^(?!Compartidos\b)[\w\-]+$/).withMessage('Please provide a valid folder name ')
    ], Authorize('Premium,Básico'), upload.single('file'), fileController.insertFile);


router.post('/fileOwner', [
    check('file_id').isInt().withMessage('File id is requiered')
], Authorize('Premium,Básico'), fileController.insertFileOwner);


router.post('/fileShared',[
    check('file_id').isInt().withMessage('File id is requiered'),
    check('shared_id').isInt().withMessage('Share id is requiered')
], Authorize('Premium'), fileController.insertFileShared);


router.get('/file', [
    check('file_id').isInt().withMessage('File id is requiered')
],Authorize('Premium,Básico'),  fileController.getFileFromServer);


router.delete('/fileServer',[
    check('file_id').isInt().withMessage('File id is requiered')
], Authorize('Premium,Básico'),  fileController.deleteFilefromServer);


router.delete('/fileShared',[
    check('file_id').isInt().withMessage('File id is requiered')
], Authorize('Premium,Básico') ,fileController.deletefileShared);


router.delete('/file',[
    check('file_id').isInt().withMessage('File id is requiered')
], Authorize('Premium,Básico'), fileController.deleteFileRegistration);


router.get('/usersShareFile', [
    check('file_id').isInt().withMessage('File id is requiered')
], Authorize('Premium,Básico'), fileController.getUsersShareFile );


router.get('/usersOwnerFile', [
    check('file_id').isInt().withMessage('File id is requiered')
],Authorize('Premium,Básico'), fileController.getUsersOwnerFile );


router.get('/foldersByUser',Authorize('Premium,Básico'), fileController.getFoldersByUser);


router.get('/listOfFileInfoByUser', [
    check('folder_name').matches(/^(?!Compartidos\b)[\w\-]+$/).withMessage('Please provide a valid folder name ')
    ],  Authorize('Premium,Básico'), fileController.getListOfFileInfoByUser);


router.get('/listOfFileSharedByUser',Authorize('Premium,Básico'), fileController.getListOfFileSharedByUser);
module.exports = router;