-- Полная модель кадрового учёта: 15 таблиц, 21 внешний ключ.
-- Выполнять в базе HrManagement после 001_create_database.sql.
-- Существующие таблицы и кадровые записи сохраняются.
SET NOCOUNT ON;
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID(N'dbo.Departments', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[Departments] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(120) NOT NULL,
    [Code] nvarchar(20) NOT NULL,
    [ParentDepartmentId] int NULL,
    CONSTRAINT [PK_Departments] PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_Departments_Name] UNIQUE ([Name]),
    CONSTRAINT [UQ_Departments_Code] UNIQUE ([Code])
);
END;

IF OBJECT_ID(N'dbo.Positions', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[Positions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(120) NOT NULL,
    [SalaryRate] decimal(12,2) NOT NULL,
    CONSTRAINT [PK_Positions] PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_Positions_Name] UNIQUE ([Name])
);
END;

IF OBJECT_ID(N'dbo.StaffUnits', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[StaffUnits] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [DepartmentId] int NOT NULL,
    [PositionId] int NOT NULL,
    [Quantity] decimal(5,2) NOT NULL,
    [SalaryMin] decimal(12,2) NOT NULL,
    [SalaryMax] decimal(12,2) NOT NULL,
    [EffectiveFrom] date NOT NULL,
    [EffectiveTo] date NULL,
    CONSTRAINT [PK_StaffUnits] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_StaffUnits_1] CHECK ([Quantity] > 0),
    CONSTRAINT [CK_StaffUnits_2] CHECK ([SalaryMin] >= 0 AND [SalaryMax] >= [SalaryMin]),
    CONSTRAINT [CK_StaffUnits_3] CHECK ([EffectiveTo] IS NULL OR [EffectiveTo] >= [EffectiveFrom])
);
END;

IF OBJECT_ID(N'dbo.Employees', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[Employees] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [PersonnelNumber] nvarchar(20) NOT NULL,
    [LastName] nvarchar(80) NOT NULL,
    [FirstName] nvarchar(80) NOT NULL,
    [MiddleName] nvarchar(80) NULL,
    [BirthDate] date NOT NULL,
    [HireDate] date NOT NULL,
    [Email] nvarchar(160) NOT NULL,
    [Phone] nvarchar(30) NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [DepartmentId] int NOT NULL,
    [PositionId] int NOT NULL,
    [Snils] char(11) NULL,
    [Inn] char(12) NULL,
    CONSTRAINT [PK_Employees] PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_Employees_PersonnelNumber] UNIQUE ([PersonnelNumber]),
    CONSTRAINT [UQ_Employees_Email] UNIQUE ([Email])
);
END;

IF OBJECT_ID(N'dbo.EmploymentContracts', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[EmploymentContracts] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [EmployeeId] int NOT NULL,
    [StaffUnitId] int NOT NULL,
    [ContractNumber] nvarchar(30) NOT NULL,
    [ContractDate] date NOT NULL,
    [StartDate] date NOT NULL,
    [EndDate] date NULL,
    [EmploymentType] nvarchar(30) NOT NULL,
    [Rate] decimal(4,2) NOT NULL,
    [Salary] decimal(12,2) NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    CONSTRAINT [PK_EmploymentContracts] PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_EmploymentContracts_ContractNumber] UNIQUE ([ContractNumber]),
    CONSTRAINT [CK_EmploymentContracts_1] CHECK ([Rate] > 0 AND [Rate] <= 2),
    CONSTRAINT [CK_EmploymentContracts_2] CHECK ([Salary] >= 0),
    CONSTRAINT [CK_EmploymentContracts_3] CHECK ([EndDate] IS NULL OR [EndDate] >= [StartDate])
);
END;

IF OBJECT_ID(N'dbo.PersonnelOrders', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[PersonnelOrders] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [EmployeeId] int NOT NULL,
    [ContractId] bigint NULL,
    [CreatedByUserId] int NOT NULL,
    [OrderNumber] nvarchar(30) NOT NULL,
    [OrderDate] date NOT NULL,
    [OrderType] nvarchar(30) NOT NULL,
    [EffectiveDate] date NOT NULL,
    [Reason] nvarchar(500) NOT NULL,
    [FilePath] nvarchar(500) NULL,
    CONSTRAINT [PK_PersonnelOrders] PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_PersonnelOrders_OrderNumber] UNIQUE ([OrderNumber])
);
END;

IF OBJECT_ID(N'dbo.LeaveTypes', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[LeaveTypes] (
    [Id] smallint IDENTITY(1,1) NOT NULL,
    [Code] nvarchar(20) NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [IsPaid] bit NOT NULL,
    CONSTRAINT [PK_LeaveTypes] PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_LeaveTypes_Code] UNIQUE ([Code])
);
END;

IF OBJECT_ID(N'dbo.Leaves', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[Leaves] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [EmployeeId] int NOT NULL,
    [LeaveTypeId] smallint NOT NULL,
    [OrderId] bigint NULL,
    [StartDate] date NOT NULL,
    [EndDate] date NOT NULL,
    [CalendarDays] smallint NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    CONSTRAINT [PK_Leaves] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_Leaves_1] CHECK ([EndDate] >= [StartDate]),
    CONSTRAINT [CK_Leaves_2] CHECK ([CalendarDays] > 0)
);
END;

IF OBJECT_ID(N'dbo.Absences', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[Absences] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [EmployeeId] int NOT NULL,
    [Type] nvarchar(40) NOT NULL,
    [StartDate] date NOT NULL,
    [EndDate] date NOT NULL,
    [DocumentNumber] nvarchar(60) NULL,
    [Comment] nvarchar(500) NULL,
    [LeaveId] bigint NULL,
    CONSTRAINT [PK_Absences] PRIMARY KEY ([Id])
);
END;

IF OBJECT_ID(N'dbo.EmployeeDocuments', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[EmployeeDocuments] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [EmployeeId] int NOT NULL,
    [DocumentType] nvarchar(50) NOT NULL,
    [Series] nvarchar(20) NULL,
    [Number] nvarchar(50) NOT NULL,
    [IssueDate] date NULL,
    [IssuedBy] nvarchar(300) NULL,
    [ExpiresAt] date NULL,
    [FilePath] nvarchar(500) NULL,
    CONSTRAINT [PK_EmployeeDocuments] PRIMARY KEY ([Id])
);
END;

IF OBJECT_ID(N'dbo.EducationRecords', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[EducationRecords] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [EmployeeId] int NOT NULL,
    [EducationLevel] nvarchar(50) NOT NULL,
    [Institution] nvarchar(250) NOT NULL,
    [Specialty] nvarchar(200) NOT NULL,
    [Qualification] nvarchar(200) NULL,
    [DocumentNumber] nvarchar(50) NOT NULL,
    [GraduationYear] smallint NOT NULL,
    CONSTRAINT [PK_EducationRecords] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_EducationRecords_1] CHECK ([GraduationYear] BETWEEN 1900 AND 2200)
);
END;

IF OBJECT_ID(N'dbo.UserAccounts', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[UserAccounts] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Login] nvarchar(60) NOT NULL,
    [PasswordHash] nvarchar(256) NOT NULL,
    [PasswordSalt] nvarchar(128) NOT NULL,
    [DisplayName] nvarchar(160) NOT NULL,
    [Role] nvarchar(40) NOT NULL,
    [IsActive] bit NOT NULL,
    [EmployeeId] int NULL,
    CONSTRAINT [PK_UserAccounts] PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_UserAccounts_Login] UNIQUE ([Login])
);
END;

IF OBJECT_ID(N'dbo.Roles', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[Roles] (
    [Id] smallint IDENTITY(1,1) NOT NULL,
    [Code] nvarchar(30) NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(500) NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([Id]),
    CONSTRAINT [UQ_Roles_Code] UNIQUE ([Code]),
    CONSTRAINT [UQ_Roles_Name] UNIQUE ([Name])
);
END;

IF OBJECT_ID(N'dbo.UserRoles', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[UserRoles] (
    [UserAccountId] int NOT NULL,
    [RoleId] smallint NOT NULL,
    [AssignedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_UserRoles] PRIMARY KEY ([UserAccountId], [RoleId])
);
END;

IF OBJECT_ID(N'dbo.AuditEntries', N'U') IS NULL
BEGIN
CREATE TABLE [dbo].[AuditEntries] (
    [Id] bigint IDENTITY(1,1) NOT NULL,
    [UserAccountId] int NULL,
    [Action] nvarchar(80) NOT NULL,
    [EntityName] nvarchar(80) NOT NULL,
    [EntityId] nvarchar(80) NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_AuditEntries] PRIMARY KEY ([Id])
);
END;

IF COL_LENGTH(N'dbo.Departments', N'ParentDepartmentId') IS NULL
    ALTER TABLE [dbo].[Departments] ADD [ParentDepartmentId] int NULL;
IF COL_LENGTH(N'dbo.Employees', N'Snils') IS NULL
    ALTER TABLE [dbo].[Employees] ADD [Snils] char(11) NULL;
IF COL_LENGTH(N'dbo.Employees', N'Inn') IS NULL
    ALTER TABLE [dbo].[Employees] ADD [Inn] char(12) NULL;
IF COL_LENGTH(N'dbo.Absences', N'LeaveId') IS NULL
    ALTER TABLE [dbo].[Absences] ADD [LeaveId] bigint NULL;
IF COL_LENGTH(N'dbo.UserAccounts', N'EmployeeId') IS NULL
    ALTER TABLE [dbo].[UserAccounts] ADD [EmployeeId] int NULL;
COMMIT TRANSACTION;
IF NOT EXISTS (SELECT 1 FROM sys.foreign_key_columns f WHERE f.parent_object_id = OBJECT_ID(N'dbo.Departments') AND COL_NAME(f.parent_object_id, f.parent_column_id) = N'ParentDepartmentId')
    EXEC(N'ALTER TABLE [dbo].[Departments] WITH CHECK ADD CONSTRAINT [FK_Departments_ParentDepartmentId_Departments] FOREIGN KEY ([ParentDepartmentId]) REFERENCES [dbo].[Departments] ([Id]) ON DELETE NO ACTION;');
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Departments') AND name = N'IX_Departments_ParentDepartmentId')
    EXEC(N'CREATE INDEX [IX_Departments_ParentDepartmentId] ON [dbo].[Departments] ([ParentDepartmentId]);');
IF NOT EXISTS (SELECT 1 FROM sys.foreign_key_columns f WHERE f.parent_object_id = OBJECT_ID(N'dbo.StaffUnits') AND COL_NAME(f.parent_object_id, f.parent_column_id) = N'DepartmentId')
    EXEC(N'ALTER TABLE [dbo].[StaffUnits] WITH CHECK ADD CONSTRAINT [FK_StaffUnits_DepartmentId_Departments] FOREIGN KEY ([DepartmentId]) REFERENCES [dbo].[Departments] ([Id]) ON DELETE NO ACTION;');
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.StaffUnits') AND name = N'IX_StaffUnits_DepartmentId')
    EXEC(N'CREATE INDEX [IX_StaffUnits_DepartmentId] ON [dbo].[StaffUnits] ([DepartmentId]);');
IF NOT EXISTS (SELECT 1 FROM sys.foreign_key_columns f WHERE f.parent_object_id = OBJECT_ID(N'dbo.StaffUnits') AND COL_NAME(f.parent_object_id, f.parent_column_id) = N'PositionId')
    EXEC(N'ALTER TABLE [dbo].[StaffUnits] WITH CHECK ADD CONSTRAINT [FK_StaffUnits_PositionId_Positions] FOREIGN KEY ([PositionId]) REFERENCES [dbo].[Positions] ([Id]) ON DELETE NO ACTION;');
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.StaffUnits') AND name = N'IX_StaffUnits_PositionId')
    EXEC(N'CREATE INDEX [IX_StaffUnits_PositionId] ON [dbo].[StaffUnits] ([PositionId]);');
IF NOT EXISTS (SELECT 1 FROM sys.foreign_key_columns f WHERE f.parent_object_id = OBJECT_ID(N'dbo.Employees') AND COL_NAME(f.parent_object_id, f.parent_column_id) = N'DepartmentId')
    EXEC(N'ALTER TABLE [dbo].[Employees] WITH CHECK ADD CONSTRAINT [FK_Employees_DepartmentId_Departments] FOREIGN KEY ([DepartmentId]) REFERENCES [dbo].[Departments] ([Id]) ON DELETE NO ACTION;');
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Employees') AND name = N'IX_Employees_DepartmentId')
    EXEC(N'CREATE INDEX [IX_Employees_DepartmentId] ON [dbo].[Employees] ([DepartmentId]);');
IF NOT EXISTS (SELECT 1 FROM sys.foreign_key_columns f WHERE f.parent_object_id = OBJECT_ID(N'dbo.Employees') AND COL_NAME(f.parent_object_id, f.parent_column_id) = N'PositionId')
    EXEC(N'ALTER TABLE [dbo].[Employees] WITH CHECK ADD CONSTRAINT [FK_Employees_PositionId_Positions] FOREIGN KEY ([PositionId]) REFERENCES [dbo].[Positions] ([Id]) ON DELETE NO ACTION;');
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Employees') AND name = N'IX_Employees_PositionId')
    EXEC(N'CREATE INDEX [IX_Employees_PositionId] ON [dbo].[Employees] ([PositionId]);');
IF NOT EXISTS (SELECT 1 FROM sys.foreign_key_columns f WHERE f.parent_object_id = OBJECT_ID(N'dbo.EmploymentContracts') AND COL_NAME(f.parent_object_id, f.parent_column_id) = N'EmployeeId')
    EXEC(N'ALTER TABLE [dbo].[EmploymentContracts] WITH CHECK ADD CONSTRAINT [FK_EmploymentContracts_EmployeeId_Employees] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([Id]) ON DELETE NO ACTION;');
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.EmploymentContracts') AND name = N'IX_EmploymentContracts_EmployeeId')
    EXEC(N'CREATE INDEX [IX_EmploymentContracts_EmployeeId] ON [dbo].[EmploymentContracts] ([EmployeeId]);');
IF NOT EXISTS (SELECT 1 FROM sys.foreign_key_columns f WHERE f.parent_object_id = OBJECT_ID(N'dbo.EmploymentContracts') AND COL_NAME(f.parent_object_id, f.parent_column_id) = N'StaffUnitId')
    EXEC(N'ALTER TABLE [dbo].[EmploymentContracts] WITH CHECK ADD CONSTRAINT [FK_EmploymentContracts_StaffUnitId_StaffUnits] FOREIGN KEY ([StaffUnitId]) REFERENCES [dbo].[StaffUnits] ([Id]) ON DELETE NO ACTION;');
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.EmploymentContracts') AND name = N'IX_EmploymentContracts_StaffUnitId')
    EXEC(N'CREATE INDEX [IX_EmploymentContracts_StaffUnitId] ON [dbo].[EmploymentContracts] ([StaffUnitId]);');
IF NOT EXISTS (SELECT 1 FROM sys.foreign_key_columns f WHERE f.parent_object_id = OBJECT_ID(N'dbo.PersonnelOrders') AND COL_NAME(f.parent_object_id, f.parent_column_id) = N'EmployeeId')
    EXEC(N'ALTER TABLE [dbo].[PersonnelOrders] WITH CHECK ADD CONSTRAINT [FK_PersonnelOrders_EmployeeId_Employees] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([Id]) ON DELETE NO ACTION;');
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.PersonnelOrders') AND name = N'IX_PersonnelOrders_EmployeeId')
    EXEC(N'CREATE INDEX [IX_PersonnelOrders_EmployeeId] ON [dbo].[PersonnelOrders] ([EmployeeId]);');
IF NOT EXISTS (SELECT 1 FROM sys.foreign_key_columns f WHERE f.parent_object_id = OBJECT_ID(N'dbo.PersonnelOrders') AND COL_NAME(f.parent_object_id, f.parent_column_id) = N'ContractId')
    EXEC(N'ALTER TABLE [dbo].[PersonnelOrders] WITH CHECK ADD CONSTRAINT [FK_PersonnelOrders_ContractId_EmploymentContracts] FOREIGN KEY ([ContractId]) REFERENCES [dbo].[EmploymentContracts] ([Id]) ON DELETE NO ACTION;');
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.PersonnelOrders') AND name = N'IX_PersonnelOrders_ContractId')
    EXEC(N'CREATE INDEX [IX_PersonnelOrders_ContractId] ON [dbo].[PersonnelOrders] ([ContractId]);');
IF NOT EXISTS (SELECT 1 FROM sys.foreign_key_columns f WHERE f.parent_object_id = OBJECT_ID(N'dbo.PersonnelOrders') AND COL_NAME(f.parent_object_id, f.parent_column_id) = N'CreatedByUserId')
    EXEC(N'ALTER TABLE [dbo].[PersonnelOrders] WITH CHECK ADD CONSTRAINT [FK_PersonnelOrders_CreatedByUserId_UserAccounts] FOREIGN KEY ([CreatedByUserId]) REFERENCES [dbo].[UserAccounts] ([Id]) ON DELETE NO ACTION;');
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.PersonnelOrders') AND name = N'IX_PersonnelOrders_CreatedByUserId')
    EXEC(N'CREATE INDEX [IX_PersonnelOrders_CreatedByUserId] ON [dbo].[PersonnelOrders] ([CreatedByUserId]);');
IF NOT EXISTS (SELECT 1 FROM sys.foreign_key_columns f WHERE f.parent_object_id = OBJECT_ID(N'dbo.Leaves') AND COL_NAME(f.parent_object_id, f.parent_column_id) = N'EmployeeId')
    EXEC(N'ALTER TABLE [dbo].[Leaves] WITH CHECK ADD CONSTRAINT [FK_Leaves_EmployeeId_Employees] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([Id]) ON DELETE NO ACTION;');
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Leaves') AND name = N'IX_Leaves_EmployeeId')
    EXEC(N'CREATE INDEX [IX_Leaves_EmployeeId] ON [dbo].[Leaves] ([EmployeeId]);');
IF NOT EXISTS (SELECT 1 FROM sys.foreign_key_columns f WHERE f.parent_object_id = OBJECT_ID(N'dbo.Leaves') AND COL_NAME(f.parent_object_id, f.parent_column_id) = N'LeaveTypeId')
    EXEC(N'ALTER TABLE [dbo].[Leaves] WITH CHECK ADD CONSTRAINT [FK_Leaves_LeaveTypeId_LeaveTypes] FOREIGN KEY ([LeaveTypeId]) REFERENCES [dbo].[LeaveTypes] ([Id]) ON DELETE NO ACTION;');
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Leaves') AND name = N'IX_Leaves_LeaveTypeId')
    EXEC(N'CREATE INDEX [IX_Leaves_LeaveTypeId] ON [dbo].[Leaves] ([LeaveTypeId]);');
IF NOT EXISTS (SELECT 1 FROM sys.foreign_key_columns f WHERE f.parent_object_id = OBJECT_ID(N'dbo.Leaves') AND COL_NAME(f.parent_object_id, f.parent_column_id) = N'OrderId')
    EXEC(N'ALTER TABLE [dbo].[Leaves] WITH CHECK ADD CONSTRAINT [FK_Leaves_OrderId_PersonnelOrders] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[PersonnelOrders] ([Id]) ON DELETE NO ACTION;');
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Leaves') AND name = N'IX_Leaves_OrderId')
    EXEC(N'CREATE INDEX [IX_Leaves_OrderId] ON [dbo].[Leaves] ([OrderId]);');
IF NOT EXISTS (SELECT 1 FROM sys.foreign_key_columns f WHERE f.parent_object_id = OBJECT_ID(N'dbo.Absences') AND COL_NAME(f.parent_object_id, f.parent_column_id) = N'EmployeeId')
    EXEC(N'ALTER TABLE [dbo].[Absences] WITH CHECK ADD CONSTRAINT [FK_Absences_EmployeeId_Employees] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([Id]) ON DELETE NO ACTION;');
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Absences') AND name = N'IX_Absences_EmployeeId')
    EXEC(N'CREATE INDEX [IX_Absences_EmployeeId] ON [dbo].[Absences] ([EmployeeId]);');
IF NOT EXISTS (SELECT 1 FROM sys.foreign_key_columns f WHERE f.parent_object_id = OBJECT_ID(N'dbo.Absences') AND COL_NAME(f.parent_object_id, f.parent_column_id) = N'LeaveId')
    EXEC(N'ALTER TABLE [dbo].[Absences] WITH CHECK ADD CONSTRAINT [FK_Absences_LeaveId_Leaves] FOREIGN KEY ([LeaveId]) REFERENCES [dbo].[Leaves] ([Id]) ON DELETE NO ACTION;');
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.Absences') AND name = N'IX_Absences_LeaveId')
    EXEC(N'CREATE INDEX [IX_Absences_LeaveId] ON [dbo].[Absences] ([LeaveId]);');
IF NOT EXISTS (SELECT 1 FROM sys.foreign_key_columns f WHERE f.parent_object_id = OBJECT_ID(N'dbo.EmployeeDocuments') AND COL_NAME(f.parent_object_id, f.parent_column_id) = N'EmployeeId')
    EXEC(N'ALTER TABLE [dbo].[EmployeeDocuments] WITH CHECK ADD CONSTRAINT [FK_EmployeeDocuments_EmployeeId_Employees] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([Id]) ON DELETE NO ACTION;');
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.EmployeeDocuments') AND name = N'IX_EmployeeDocuments_EmployeeId')
    EXEC(N'CREATE INDEX [IX_EmployeeDocuments_EmployeeId] ON [dbo].[EmployeeDocuments] ([EmployeeId]);');
IF NOT EXISTS (SELECT 1 FROM sys.foreign_key_columns f WHERE f.parent_object_id = OBJECT_ID(N'dbo.EducationRecords') AND COL_NAME(f.parent_object_id, f.parent_column_id) = N'EmployeeId')
    EXEC(N'ALTER TABLE [dbo].[EducationRecords] WITH CHECK ADD CONSTRAINT [FK_EducationRecords_EmployeeId_Employees] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([Id]) ON DELETE NO ACTION;');
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.EducationRecords') AND name = N'IX_EducationRecords_EmployeeId')
    EXEC(N'CREATE INDEX [IX_EducationRecords_EmployeeId] ON [dbo].[EducationRecords] ([EmployeeId]);');
IF NOT EXISTS (SELECT 1 FROM sys.foreign_key_columns f WHERE f.parent_object_id = OBJECT_ID(N'dbo.UserAccounts') AND COL_NAME(f.parent_object_id, f.parent_column_id) = N'EmployeeId')
    EXEC(N'ALTER TABLE [dbo].[UserAccounts] WITH CHECK ADD CONSTRAINT [FK_UserAccounts_EmployeeId_Employees] FOREIGN KEY ([EmployeeId]) REFERENCES [dbo].[Employees] ([Id]) ON DELETE NO ACTION;');
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.UserAccounts') AND name = N'IX_UserAccounts_EmployeeId')
    EXEC(N'CREATE INDEX [IX_UserAccounts_EmployeeId] ON [dbo].[UserAccounts] ([EmployeeId]);');
IF NOT EXISTS (SELECT 1 FROM sys.foreign_key_columns f WHERE f.parent_object_id = OBJECT_ID(N'dbo.UserRoles') AND COL_NAME(f.parent_object_id, f.parent_column_id) = N'UserAccountId')
    EXEC(N'ALTER TABLE [dbo].[UserRoles] WITH CHECK ADD CONSTRAINT [FK_UserRoles_UserAccountId_UserAccounts] FOREIGN KEY ([UserAccountId]) REFERENCES [dbo].[UserAccounts] ([Id]) ON DELETE NO ACTION;');
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.UserRoles') AND name = N'IX_UserRoles_UserAccountId')
    EXEC(N'CREATE INDEX [IX_UserRoles_UserAccountId] ON [dbo].[UserRoles] ([UserAccountId]);');
IF NOT EXISTS (SELECT 1 FROM sys.foreign_key_columns f WHERE f.parent_object_id = OBJECT_ID(N'dbo.UserRoles') AND COL_NAME(f.parent_object_id, f.parent_column_id) = N'RoleId')
    EXEC(N'ALTER TABLE [dbo].[UserRoles] WITH CHECK ADD CONSTRAINT [FK_UserRoles_RoleId_Roles] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([Id]) ON DELETE NO ACTION;');
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.UserRoles') AND name = N'IX_UserRoles_RoleId')
    EXEC(N'CREATE INDEX [IX_UserRoles_RoleId] ON [dbo].[UserRoles] ([RoleId]);');
IF NOT EXISTS (SELECT 1 FROM sys.foreign_key_columns f WHERE f.parent_object_id = OBJECT_ID(N'dbo.AuditEntries') AND COL_NAME(f.parent_object_id, f.parent_column_id) = N'UserAccountId')
    EXEC(N'ALTER TABLE [dbo].[AuditEntries] WITH CHECK ADD CONSTRAINT [FK_AuditEntries_UserAccountId_UserAccounts] FOREIGN KEY ([UserAccountId]) REFERENCES [dbo].[UserAccounts] ([Id]) ON DELETE NO ACTION;');
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.AuditEntries') AND name = N'IX_AuditEntries_UserAccountId')
    EXEC(N'CREATE INDEX [IX_AuditEntries_UserAccountId] ON [dbo].[AuditEntries] ([UserAccountId]);');

SELECT COUNT(*) AS TableCount FROM sys.tables WHERE is_ms_shipped = 0;
SELECT COUNT(*) AS ForeignKeyCount FROM sys.foreign_keys;
