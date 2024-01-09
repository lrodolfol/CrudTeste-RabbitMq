create database TesteUser;
use TesteUser;
CREATE TABLE TesteUser.`user` (
	id INTEGER auto_increment NOT NULL PRIMARY KEY,
	name varchar(100) NOT NULL,
	email varchar(100) NOT NULL,
	document varchar(100) NOT NULL,
	phone varchar(100) NOT NULL,
	password varchar(255) NOT NULL
)
ENGINE=InnoDB
DEFAULT CHARSET=utf8mb4
COLLATE=utf8mb4_german2_ci;