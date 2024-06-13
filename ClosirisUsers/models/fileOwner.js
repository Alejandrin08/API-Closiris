'use strict';

const { DataTypes } = require('sequelize');

module.exports = function(sequelize) {
  const FileOwner = sequelize.define('fileOwner', {
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
    idFile: {
      type: DataTypes.INTEGER,
      allowNull: false,
      references: {
        model: 'file',
        key: 'id'
      }
    }
  }, {
    tableName: 'fileOwner',
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
        name: "fk_fileOwner_user",
        using: "BTREE",
        fields: [
          { name: "idUser" },
        ]
      },
      {
        name: "fk_fileOwner_file",
        using: "BTREE",
        fields: [
          { name: "idFile" },
        ]
      },
    ]
  });

  return FileOwner;
};
