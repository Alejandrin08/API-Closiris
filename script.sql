CREATE DATABASE "closirisdb" /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE closirisdb;

CREATE TABLE "userAccount" (
  "id" int NOT NULL AUTO_INCREMENT,
  "email" varchar(150) NOT NULL,
  "password" varchar(100) NOT NULL,
  "name" varchar(100) NOT NULL,
  "imageProfile" mediumblob,
  PRIMARY KEY ("id"),
  UNIQUE KEY "email" ("email")
);

CREATE TABLE "user" (
  "id" int NOT NULL AUTO_INCREMENT,
  "email" varchar(150) NOT NULL,
  "plan" varchar(50) NOT NULL,
  "freeStorage" decimal(13,2) NOT NULL,
  PRIMARY KEY ("id"),
  UNIQUE KEY "email" ("email"),
  CONSTRAINT "fk_user_userAccount" FOREIGN KEY ("email") REFERENCES "userAccount" ("email") ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE "audit" (
  "id" int NOT NULL AUTO_INCREMENT,
  "idUser" int NOT NULL,
  "action" varchar(50) NOT NULL,
  "user" varchar(150) NOT NULL,
  "ip" varchar(50) NOT NULL,
  "initDate" datetime NOT NULL,
  PRIMARY KEY ("id"),
  KEY "fk_audit_user" ("idUser"),
  CONSTRAINT "fk_audit_user" FOREIGN KEY ("idUser") REFERENCES "user" ("id") ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE "file" (
  "id" int NOT NULL AUTO_INCREMENT,
  "fileName" varchar(150) NOT NULL,
  "location" varchar(150) NOT NULL,
  "size" decimal(13,2) NOT NULL,
  "creationDate" date NOT NULL,
  PRIMARY KEY ("id"),
  UNIQUE KEY "location_UNIQUE" ("location")
);

CREATE TABLE "fileOwner" (
  "id" int NOT NULL AUTO_INCREMENT,
  "idUser" int NOT NULL,
  "idFile" int NOT NULL,
  PRIMARY KEY ("id"),
  KEY "fk_fileOwner_user" ("idUser"),
  KEY "fk_fileOwner_file" ("idFile"),
  CONSTRAINT "fk_fileOwner_file" FOREIGN KEY ("idFile") REFERENCES "file" ("id") ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT "fk_fileOwner_user" FOREIGN KEY ("idUser") REFERENCES "user" ("id") ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE "fileShared" (
  "id" int NOT NULL AUTO_INCREMENT,
  "idUser" int NOT NULL,
  "idFile" int NOT NULL,
  "date" date NOT NULL,
  PRIMARY KEY ("id"),
  KEY "fk_fileShared_user" ("idUser"),
  KEY "fk_fileShared_file" ("idFile"),
  CONSTRAINT "fk_fileShared_file" FOREIGN KEY ("idFile") REFERENCES "file" ("id") ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT "fk_fileShared_user" FOREIGN KEY ("idUser") REFERENCES "user" ("id") ON DELETE CASCADE ON UPDATE CASCADE
);





