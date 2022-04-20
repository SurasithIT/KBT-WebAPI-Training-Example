CREATE DATABASE Demo;
GO
USE Demo;
GO
CREATE TABLE Employee
(
    EmployeeKey INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    FirstName varchar(255) NOT NULL,
    LastName varchar(255) NOT NULL
);
GO
CREATE TABLE [User]
(
    UserKey INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    UserName varchar(255) NOT NULL,
    Password varchar(255) NOT NULL,
    IsActive BIT,
    EmployeeKey INT,
    FOREIGN KEY (EmployeeKey) REFERENCES Employee(EmployeeKey)
);
GO
CREATE TABLE [JWT]
(
    JWTKey INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    UserKey INT NOT NULL,
    AccessToken varchar(255) NOT NULL,
    RefreshToken varchar(255) NOT NULL,
    IssueAt DATETIME NOT NULL,
    ExpireAt DATETIME NOT NULL
);
GO