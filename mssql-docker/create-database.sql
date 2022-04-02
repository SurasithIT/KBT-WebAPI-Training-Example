CREATE DATABASE Demo;
GO
USE Demo;
GO
CREATE TABLE Employee
(
    EmployeeKey INT NOT NULL PRIMARY KEY,
    FirstName varchar(255) NOT NULL,
    LastName varchar(255) NOT NULL
);
GO
CREATE TABLE [User]
(
    UserKey INT NOT NULL PRIMARY KEY,
    UserName varchar(255) NOT NULL,
    Password varchar(255) NOT NULL,
    IsActive BIT,
    EmployeeKey INT,
    FOREIGN KEY (EmployeeKey) REFERENCES Employee(EmployeeKey)
);
GO