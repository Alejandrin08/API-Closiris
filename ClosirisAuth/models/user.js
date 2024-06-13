'use strict';

const { DataTypes } = require('sequelize');

module.exports = function(sequelize) {
  const User = sequelize.define('User', {
    id: {
      autoIncrement: true,
      type: DataTypes.INTEGER,
      allowNull: false,
      primaryKey: true
    },
    email: {
      type: DataTypes.STRING(150),
      allowNull: false,
      references: {
        model: 'userAccount',
        key: 'email'
      },
      unique: "fk_user_userAccount"
    },
    plan: {
      type: DataTypes.STRING(50),
      allowNull: false
    },
    freeStorage: {
      type: DataTypes.DECIMAL(13,2),
      allowNull: false
    }
  }, {
    sequelize,
    tableName: 'user',
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
        name: "email",
        unique: true,
        using: "BTREE",
        fields: [
          { name: "email" },
        ]
      },
    ]
  });

  return User;
};