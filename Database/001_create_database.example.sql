IF DB_ID(N'HrManagement') IS NULL
BEGIN
    CREATE DATABASE [HrManagement];
END;
GO

IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'hr_app')
BEGIN
    CREATE LOGIN [hr_app]
        WITH PASSWORD = N'REPLACE_WITH_DATABASE_PASSWORD',
             CHECK_POLICY = ON,
             CHECK_EXPIRATION = OFF;
END;
GO

USE [HrManagement];
GO

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'hr_app')
BEGIN
    CREATE USER [hr_app] FOR LOGIN [hr_app];
END;
GO

ALTER ROLE [db_owner] ADD MEMBER [hr_app];
GO
