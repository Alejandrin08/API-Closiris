const ClaimTypes = require('../config/claimtypes');
const initModels = require('../models/init-models');
const sequelize = require('../models/sequelize');
const models = initModels(sequelize);
const { Op, Sequelize } = require('sequelize');
const dotenv = require('dotenv');
dotenv.config();
const File = models.file;
const FileOwner = models.fileOwner;
const FileShared = models.fileShared;
const UserAccount = models.userAccount;
const User = models.user;
const host = process.env.SERVER_FILES_HOST;
const port = process.env.SERVER_FILES_PORT;

FileShared.belongsTo(File, { foreignKey: 'idFile' });
File.hasMany(FileShared, { foreignKey: 'idFile' });

FileShared.belongsTo(User, { foreignKey: 'idUser' });
User.hasMany(FileShared, { foreignKey: 'idUser' });

const grpc = require('@grpc/grpc-js');
const protoLoader = require('@grpc/proto-loader');
const path = require('path');
const fs = require('fs');
const axios = require('axios');
const { response } = require('express');
const { Console } = require('console');

const PROTO_PATH = path.join(__dirname, '../protos/file.proto');
const packageDefinition = protoLoader.loadSync(PROTO_PATH);
const protoDescriptor = grpc.loadPackageDefinition(packageDefinition);
const fileTransferClient = new protoDescriptor.FileTransfer(`${host}:${port}`, grpc.credentials.createInsecure());

let self = {};

async function sendFileToServer(userId, folderName, fileName, fileData) {
    const metadata = new grpc.Metadata();
    metadata.set('user_id', userId);
    metadata.set('folder_name', folderName);

    const call = fileTransferClient.UploadFile({
        filename: fileName,
        data: fileData
    }, metadata, (error, response) => {
        if (error) {
            console.error('Error:', error);
        } else {
            console.log('File uploaded successfully');
        }
    });

    call.on('status', (status) => {
        console.log('Upload status:', status);
    });

    call.on('end', () => {
        console.log('Upload finished');
    });
}



async function deleteFileInServer(fileLocation) {
    const metadata = new grpc.Metadata();

    const call = fileTransferClient.DeleteFile({
        fileLocation: fileLocation
    }, metadata, (error, response) => {
        if (error) {
            console.error('Error:', error);
        } else {
            console.log('File deleted successfully');
        }
    });

    call.on('status', (status) => {
        console.log('Upload status:', status);
    });

    call.on('end', () => {
        console.log('Upload finished');
    });
}

async function receiveFileFromServer(locationFile) {
    return new Promise((resolve, reject) => {
        const metadata = new grpc.Metadata();
        const encodedLocation = encodeURIComponent(locationFile);
        metadata.set('location_file', encodedLocation);

        console.log(`Ubicación del archivo enviada: ${encodedLocation}`);

        const call = fileTransferClient.DownloadFile({
            fileLocation: encodedLocation
        }, metadata, (error, response) => {
            if (error) {
                console.error('Error:', error);
                reject(error);
            } else {
                let fileData = Buffer.alloc(0);
                fileData = Buffer.concat([fileData, response.data]);
                console.log('File received successfully');
                resolve(fileData);
            }
        });

        call.on('status', (status) => {
            console.log('Upload status:', status);
        });

        call.on('end', () => {
            console.log('Upload finished');
        });
    });
}

//POST: api/files/file
self.insertFile = async function (req, res) {
    const MAX_FILE_SIZE = 4 * 1024 * 1024;
    const userId = req.decoded[ClaimTypes.Id];
    const folderName = req.headers['folder_name'];
    const fileName = req.file.originalname;
    const fileData = req.file.buffer;

    if (fileData.length > MAX_FILE_SIZE) {
        return res.status(413).json({ error: 'El archivo excede el tamaño máximo permitido de 4 MB' });
    }

    try {
        const location = `ClosirisFiles/${userId}/${folderName}/${fileName}`;
        const today = new Date();
        const creationDate = new Date(today.getFullYear(), today.getMonth(), today.getDate());
        const file = await File.create({
            fileName: fileName,
            location: location,
            size: fileData.length,
            creationDate: creationDate,
        });

        await sendFileToServer(userId, folderName, fileName, fileData);

        req.Audit("AgregarArchivo", userId);

        return res.status(201).json({ message: 'Archivo registrado con éxito', id: file.id });
    } catch (error) {
        console.error('Error:', error);
        return res.status(500).json({ error: 'Error al procesar la solicitud' });
    }
};

//GET: api/files/getFile/:id
self.getFileFromServer = async function (req, res) {
    const fileId = req.headers['file_id'];

    if (!fileId) {
        return res.status(400).json({ message: "file_id header is missing" });
    }

    try {
        const fileRecord = await File.findOne({
            where: { id: fileId },
            attributes: ['location']
        });

        if (!fileRecord) {
            return res.status(404).json({ message: "File not found" });
        }

        const fileLocation = fileRecord.location;

        console.log(`Ubicación del archivo obtenida de la base de datos: ${fileLocation}`);

        const files = await receiveFileFromServer(fileLocation);
        const fileBase64 = files.toString('base64');

        return res.status(200).json({ message: "File received successfully", fileBase64 });
    } catch (error) {
        console.error("Error retrieving file:", error);
        return res.status(500).json({ message: "Internal server error", error });
    }
};

//Delete: api/files/deleteFile/:id
self.deleteFilefromServer = async function (req, res) {
    const fileId = req.headers['file_id'];

    try {
        const fileRecord = await File.findOne({
            where: { id: fileId },
            attributes: ['location']
        });

        if (!fileRecord) {
            return res.status(404).json({ message: "File not found" });
        }

        const fileLocation = fileRecord.location;
        await deleteFileInServer(fileLocation);

        return res.status(200).json({ message: "File deleted successfully"});
    } catch (error) {
        return res.status(400).json(error);
    }
}

//POST: api/files/fileOwner
self.insertFileOwner = async function (req, res) {
    const userId = req.decoded[ClaimTypes.Id];
    const fileId = req.headers['file_id'];

    if (!fileId) {
        return res.status(400).json({ message: "File ID is required" });
    }

    try {
        let data = await FileOwner.create({
            idUser: userId,
            idFile: fileId,
        });

        req.Audit("AgregarArchivoPropio", userId);

        return res.status(201).json({ message: "File owner registered successfully", file: data });
    } catch (error) {
        console.error('Error creating file owner:', error);
        return res.status(500).json({ message: "Internal server error", error: error.message });
    }
};

//POST: api/files/fileShared
self.insertFileShared = async function (req, res) {
    const userId = req.headers['shared_id'];
    const fileId = req.headers['file_id'];

    try {

        const existingRecord = await FileShared.findOne({
            where: {
                idUser: userId,
                idFile: fileId
            }
        });

        if (existingRecord) {
            return res.status(409).json();
        }

        const today = new Date();
        const creationDate = new Date(today.getFullYear(), today.getMonth(), today.getDate());

        let data = await FileShared.create({
            idUser: userId,
            idFile: fileId,
            date: creationDate
        });

        req.Audit("AgregarArchivoCompartido", userId);

        return res.status(201).json({ message: "File shared register successfully", file: data });
    } catch (error) {
        return res.status(400).json(error);
    }
};

//DELETE: api/files/deleteFileShared
self.deletefileShared = async function (req, res) {
    const userId = req.decoded[ClaimTypes.Id];
    const fileId = req.headers['file_id'];

    try {
        const file = await FileShared.findOne({ where: { idFile: fileId, idUser: userId } });

        if (!file) {
            return res.status(404).json({ message: "file not found" });
        }

        await file.destroy();

        req.Audit("EliminarArchivoCompartido", userId);

        return res.status(200).json({ message: "file deleted successfully" });
    } catch (error) {
        return res.status(400).json(error);
    }
};

//DELETE: api/files/deleteFileRegistration
self.deleteFileRegistration = async function (req, res) {

    const fileId = req.headers['file_id'];

    try {
        const fileShared = await FileShared.findOne({ where: { idFile: fileId } });
        if (fileShared) {
            await fileShared.destroy();
        }

        const fileOwner = await FileOwner.findOne({ where: { idFile: fileId } });
        if (fileOwner) {
            await fileOwner.destroy();
        }

        const file = await File.findOne({ where: { id: fileId } });
        if (file) {
            await file.destroy();
        }

        return res.status(200).json({ message: "file deleted successfully" });
    } catch (error) {
        return res.status(400).json(error);
    }
};

//GET: api/users/getUsersShareFile
self.getUsersShareFile = async function (req, res) {
    const fileId = req.headers['file_id'];
    try {
        const users = await UserAccount.findAll({
            attributes: ['name'],
            include: [{
                model: User,
                as: 'user',
                attributes: [],
                required: true,
                on: {
                    '$userAccount.email$': { [Op.eq]: sequelize.col('user.email') }
                },
                include: [{
                    model: FileShared,
                    as: 'fileShareds',
                    where: { idFile: fileId },
                    attributes: [],
                    required: true,
                    on: {
                        '$user.id$': { [Op.eq]: sequelize.col('user->fileShareds.idUser') }
                    }
                }]
            }]
        });

        if (!users) {
            return res.status(404).json({ message: 'No users found for this file.' });
        }

        return res.status(200).json(users);
    } catch (error) {
        console.error(error);
        return res.status(500).json({ message: 'An error occurred while fetching users.', error: error.message });
    }
};

self.getUsersOwnerFile = async function (req, res) {
    const fileId = req.headers['file_id'];
    try {
        const users = await UserAccount.findAll({
            attributes: ['name'],
            include: [{
                model: User,
                as: 'user',
                attributes: [],
                required: true,
                on: {
                    '$userAccount.email$': { [Op.eq]: sequelize.col('user.email') }
                },
                include: [{
                    model: FileOwner,
                    as: 'fileOwners',
                    where: { idFile: fileId },
                    attributes: [],
                    required: true,
                    on: {
                        '$user.id$': { [Op.eq]: sequelize.col('user->fileOwners.idUser') }
                    }
                }]
            }]
        });

        if (!users) {
            return res.status(404).json({ message: 'No users found for this file.' });
        }

        return res.status(200).json(users);
    } catch (error) {
        console.error(error);
        return res.status(500).json({ message: 'An error occurred while fetching users.', error: error.message });
    }
};

//GET: api/files/getFoldersByUser
self.getFoldersByUser = async function (req, res) {
    const userId = req.decoded[ClaimTypes.Id];
    try {
        const files = await File.findAll({
            attributes: [
                [sequelize.fn('DISTINCT', sequelize.col('location')), 'location']
            ],
            where: {
                location: {
                    [Op.regexp]: `.*${userId}/.*`
                }
            }
        });

        const folders = files.map(file => {
            const locationParts = file.location.split('/');
            return locationParts[locationParts.length - 2];
        });

        const uniqueFolders = [...new Set(folders)];

        req.Audit("ObtenerCarpetas", userId);

        return res.status(200).json(uniqueFolders);
    } catch (error) {
        console.error(error);
        return res.status(500).json({ message: 'An error occurred while fetching folders.', error: error.message });
    }
};

//GET: api/files/getListOfFileInfoByUser
self.getListOfFileInfoByUser = async function (req, res) {
    const userId = req.decoded[ClaimTypes.Id];
    const folderName = req.headers['folder_name'];
    try {
        const files = await File.findAll({
            attributes: [
                'id',
                'fileName',
                'size',
                'creationDate'
            ],
            where: {
                location: {
                    [Op.regexp]: `.*${userId}/${folderName}/.*`
                }
            }
        });

        req.Audit("ObtenerArchivos", userId);

        return res.status(200).json(files);
    } catch (error) {
        console.error(error);
        return res.status(500).json({ message: 'An error occurred while fetching files.', error: error.message });
    }
};

self.getListOfFileSharedByUser = async function (req, res) {
    const userId = req.decoded[ClaimTypes.Id];
    try {
        const files = await File.findAll({
            attributes: ['id', 'fileName', 'size'],
            include: [{
                model: FileShared,
                attributes: [['date', 'creationDate']],
                where: { idUser: userId }
            }]
        });

        if (files.length === 0) {
            return res.status(404).json({ message: 'No files found for this user.' });
        }
        //cambiar el formato para que regrese todo junto
        const transformedFiles = files.map(file => {
            const fileData = file.get({ plain: true });
            const creationDate = fileData.fileShareds[0].creationDate;
            delete fileData.fileShareds;
            return { ...fileData, creationDate };
        });

        req.Audit("ObtenerArchivosCompartidos", userId);

        return res.status(200).json(transformedFiles);
    } catch (error) {
        console.error(error);
        return res.status(500).json({ message: 'An error occurred while fetching files.', error: error.message });
    }

}

module.exports = self;