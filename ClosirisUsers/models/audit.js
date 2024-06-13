'use strict';

const { DataTypes } = require('sequelize');

module.exports = function(sequelize) {
  const Audit = sequelize.define('Audit', {
    id: {
      autoIncrement: true,
      type: DataTypes.INTEGER,
      allowNull: false,
      primaryKey: true
    },
    idUser: {
      type: DataTypes.INTEGER,
      allowNull: false,
      references: {
        model: 'user',
        key: 'id'
      }
    },
    action: {
      type: DataTypes.STRING(50),
      allowNull: false
    },
    user: {
      type: DataTypes.STRING(150),
      allowNull: false
    },
    ip: {
      type: DataTypes.STRING(50),
      allowNull: false
    },
    initDate: {
      type: DataTypes.DATE,
      allowNull: false
    }
  }, {
    tableName: 'audit',
    timestamps: false,
    freezeTableName: true,
    indexes: [
      {
        name: "PRIMARY",
        unique: true,
        using: "BTREE",
        fields: [
          { name: "id" },
        ]
      },
      {
        name: "fk_audit_user",
        using: "BTREE",
        fields: [
          { name: "idUser" },
        ]
      },
    ]
  });

  return Audit;
};