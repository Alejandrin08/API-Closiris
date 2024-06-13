'use strict';

const { DataTypes } = require('sequelize');

module.exports = function(sequelize) {
  const File = sequelize.define('file', {
    id: {
      autoIncrement: true,
      type: DataTypes.INTEGER,
      allowNull: false,
      primaryKey: true
    },
    fileName: {
      type: DataTypes.STRING(150),
      allowNull: false
    },
    location: {
      type: DataTypes.STRING(150),
      allowNull: false,
      unique: "location_UNIQUE"
    },
    size: {
      type: DataTypes.DECIMAL(13, 2),
      allowNull: false
    },
    creationDate: {
      type: DataTypes.DATEONLY,
      allowNull: false
    }
  }, {
    tableName: 'file',
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
        name: "location_UNIQUE",
        unique: true,
        using: "BTREE",
        fields: [
          { name: "location" },
        ]
      },
    ]
  });

  return File;
};
