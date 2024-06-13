var DataTypes = require("sequelize").DataTypes;
var _audit = require("./audit");
var _file = require("./file");
var _fileOwner = require("./fileOwner");
var _fileShared = require("./fileShared");
var _user = require("./user");
var _userAccount = require("./userAccount");

function initModels(sequelize) {
  var audit = _audit(sequelize, DataTypes);
  var file = _file(sequelize, DataTypes);
  var fileOwner = _fileOwner(sequelize, DataTypes);
  var fileShared = _fileShared(sequelize, DataTypes);
  var user = _user(sequelize, DataTypes);
  var userAccount = _userAccount(sequelize, DataTypes);

  fileOwner.belongsTo(file, { as: "idFile_file", foreignKey: "idFile"});
  file.hasMany(fileOwner, { as: "fileOwners", foreignKey: "idFile"});
  fileShared.belongsTo(file, { as: "idFile_file", foreignKey: "idFile"});
  file.hasMany(fileShared, { as: "fileShareds", foreignKey: "idFile"});
  audit.belongsTo(user, { as: "idUser_user", foreignKey: "idUser"});
  user.hasMany(audit, { as: "audits", foreignKey: "idUser"});
  fileOwner.belongsTo(user, { as: "idUser_user", foreignKey: "idUser"});
  user.hasMany(fileOwner, { as: "fileOwners", foreignKey: "idUser"});
  fileShared.belongsTo(user, { as: "idUser_user", foreignKey: "idUser"});
  user.hasMany(fileShared, { as: "fileShareds", foreignKey: "idUser"});
  user.belongsTo(userAccount, { as: "email_userAccount", foreignKey: "email"});
  userAccount.hasOne(user, { as: "user", foreignKey: "email"});

  return {
    audit,
    file,
    fileOwner,
    fileShared,
    user,
    userAccount,
  };
}
module.exports = initModels;
module.exports.initModels = initModels;
module.exports.default = initModels;