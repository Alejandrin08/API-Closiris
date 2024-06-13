'use strict';

const { DataTypes } = require('sequelize');

module.exports = function(sequelize) {
  const FileShared = sequelize.define('fileShared', {
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
    },
    date: {
      type: DataTypes.DATEONLY,
      allowNull: false
    }
  }, {
    tableName: 'fileShared',
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
        name: "fk_fileShared_user",
        using: "BTREE",
        fields: [
          { name: "idUser" },
        ]
      },
      {
        name: "fk_fileShared_file",
        using: "BTREE",
        fields: [
          { name: "idFile" },
        ]
      },
    ]
  });

  return FileShared;
};
