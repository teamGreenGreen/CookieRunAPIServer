CREATE DATABASE IF NOT EXISTS AccountDB;

USE AccountDB;

CREATE TABLE IF NOT EXISTS AccountDB.`Account`
(
    Id BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY COMMENT '계정번호',
    LoginId VARCHAR(50) NOT NULL UNIQUE COMMENT '계정',
    SaltValue VARCHAR(100) NOT NULL COMMENT  '암호화 값',
    HashedPassword VARCHAR(100) NOT NULL COMMENT '해싱된 비밀번호',
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP  COMMENT '생성 날짜'
) COMMENT '계정 정보 테이블';


INSERT INTO AccountDB.`Account`(LoginId, SaltValue, HashedPassword)
VALUES('더미1', 1234, 1234);

INSERT INTO AccountDB.`Account`(LoginId, SaltValue, HashedPassword)
VALUES('더미2', 1234, 1234);

INSERT INTO AccountDB.`Account`(LoginId, SaltValue, HashedPassword)
VALUES('더미3', 1234, 1234);