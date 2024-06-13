const express = require('express');
const multer = require('multer');
const router = express.Router();
const fileController = require('../controllers/filecontroller');
const Authorize = require('../middlewares/authmiddleware');
const file = require('../models/file');

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

router.post('/file',Authorize('Premium,Básico'), upload.single('file'), fileController.insertFile);
//POST: api/files/fileOwner

router.post('/fileOwner', Authorize('Premium,Básico'), fileController.insertFileOwner);
//POST: api/files/fileShared

router.post('/fileShared', Authorize('Premium'), fileController.insertFileShared);

//GET: api/files/getFile/:id

router.get('/getFile',Authorize('Premium,Básico'),  fileController.getFileFromServer);
//Delete: api/files/deleteFile/:id

router.delete('/deleteFileServer', Authorize('Premium,Básico'),  fileController.deleteFilefromServer);
//DELETE: api/files/deleteFileShared

router.delete('/deleteFileShared', Authorize('Premium,Básico') ,fileController.deletefileShared);
//DELETE: api/files/deleteFileRegistration

router.delete('/deleteFile', Authorize('Premium,Básico'), fileController.deleteFileRegistration);
//GET: api/users/getUsersShareFile

router.get('/getUsersShareFile',Authorize('Premium,Básico'), fileController.getUsersShareFile );
//GET: api/users/getUsersShareFile

router.get('/getUsersOwnerFile',Authorize('Premium,Básico'), fileController.getUsersOwnerFile );
//GET: api/files/getFoldersByUser

router.get('/getFoldersByUser',Authorize('Premium,Básico'), fileController.getFoldersByUser);
//GET: api/files/getListOfFileInfoByUser

router.get('/getListOfFileInfoByUser',Authorize('Premium,Básico'), fileController.getListOfFileInfoByUser);


router.get('/getListOfFileSharedByUser',Authorize('Premium,Básico'), fileController.getListOfFileSharedByUser);
module.exports = router;
