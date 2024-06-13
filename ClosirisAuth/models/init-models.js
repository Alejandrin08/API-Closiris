var DataTypes = require("sequelize").DataTypes;
var _audit = require("./audit");
var _user = require("./user");
var _userAccount = require("./userAccount");

function initModels(sequelize) {
  var audit = _audit(sequelize, DataTypes);
  var user = _user(sequelize, DataTypes);
  var userAccount = _userAccount(sequelize, DataTypes);

  audit.belongsTo(user, { as: "idUser_user", foreignKey: "idUser"});
  user.hasMany(audit, { as: "audits", foreignKey: "idUser"});
  user.belongsTo(userAccount, { as: "email_userAccount", foreignKey: "email"});
  userAccount.hasOne(user, { as: "user", foreignKey: "email"});

  return {
    audit,
    user,
    userAccount,
  };
}
module.exports = initModels;
module.exports.initModels = initModels;
module.exports.default = initModels;