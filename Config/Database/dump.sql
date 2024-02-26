CREATE DATABASE IF NOT EXISTS AccountDB;

CREATE DATABASE IF NOT EXISTS GameDB;

CREATE TABLE AccountDB.ACCOUNT
(
    `user_id`              BIGINT              AUTO_INCREMENT PRIMARY KEY,
    `email`                VARCHAR(50),
    `password`             VARCHAR(100),
    `salt_value`           VARCHAR(100),
    `created_at`           DATETIME
);

CREATE TABLE GameDB.USER_INFO 
(
    `uid`                  BIGINT              AUTO_INCREMENT PRIMARY KEY,
    `user_id`              BIGINT,
    `user_name`            VARCHAR(27),
    `level`                INT,
    `exp`                  INT,
    `money`                INT,
    `max_score`            INT,
    `acquired_cookie_id`   INT,
    `diamond`              INT
);

CREATE TABLE GameDB.FRIEND_RELATIONSHIP 
(
    `relationship_id`      BIGINT              AUTO_INCREMENT PRIMARY KEY,
    `from_user_name`       VARCHAR(27),
    `to_user_name`         VARCHAR(27)
);

CREATE TABLE GameDB.FRIEND_REQUEST
(
    `request_id`           BIGINT              AUTO_INCREMENT PRIMARY KEY,
    `from_user_name`       VARCHAR(27),
    `to_user_name`         VARCHAR(27)
);

CREATE TABLE GameDB.ATTENDANCE_INFO
(
  `attendance_id`  BIGINT              AUTO_INCREMENT PRIMARY KEY,
  `uid` BIGINT NOT NULL,
  `attendance_count` INT NOT NULL,
  `attendance_date` DATE NOT NULL
);
