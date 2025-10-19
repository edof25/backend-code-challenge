CREATE TABLE dbo.Recordstatus (
    Id TINYINT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(10) NOT NULL,
);

CREATE TABLE dbo.Roles (
    Id TINYINT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
);

CREATE TABLE dbo.Ranks (
    Id TINYINT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(20) NOT NULL,
);

CREATE TABLE dbo.Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    RoleId TINYINT NOT NULL FOREIGN KEY REFERENCES dbo.Roles(Id),
    CrewMemberId NVARCHAR(100) NULL,
    Username NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    BirthDate DATETIME2 NOT NULL,
    Nationality NVARCHAR(100) NOT NULL,
    RecordStatusId TINYINT NOT NULL FOREIGN KEY REFERENCES dbo.Recordstatus(Id),
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CreatedBy NVARCHAR(100) NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy NVARCHAR(100) NULL,
    DeletedAt DATETIME2 NULL,
    DeletedBy NVARCHAR(100) NULL
);

CREATE INDEX IX_Users_Username ON dbo.Users (Username);
CREATE INDEX IX_Users_RecordStatus ON dbo.Users (RecordStatusId);

CREATE TABLE dbo.Ships (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Code NVARCHAR(20) NOT NULL UNIQUE,
    Name NVARCHAR(100) NOT NULL,
    FiscalYear NVARCHAR(100) NOT NULL,
    RecordStatusId TINYINT NOT NULL FOREIGN KEY REFERENCES dbo.Recordstatus(Id),
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CreatedBy NVARCHAR(100) NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy NVARCHAR(100) NULL,
    DeletedAt DATETIME2 NULL,
    DeletedBy NVARCHAR(100) NULL
);

CREATE INDEX IX_Ships_Code ON dbo.Ships (Code);
CREATE INDEX IX_Ships_RecordStatus ON dbo.Ships (RecordStatusId);

CREATE TABLE dbo.CrewServiceHistories (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL FOREIGN KEY REFERENCES dbo.Users(Id),
    RankId TINYINT NOT NULL FOREIGN KEY REFERENCES dbo.Ranks(Id),
    ShipId INT NOT NULL FOREIGN KEY REFERENCES dbo.Ships(Id),
    SignOnDate DATETIME2 NOT NULL,
    SignOffDate DATETIME2 NULL,
    EndOfContractDate DATETIME2 NOT NULL,
    RecordStatusId TINYINT NOT NULL FOREIGN KEY REFERENCES dbo.Recordstatus(Id),
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CreatedBy NVARCHAR(100) NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy NVARCHAR(100) NULL,
    DeletedAt DATETIME2 NULL,
    DeletedBy NVARCHAR(100) NULL
);

CREATE INDEX IX_CrewServiceHistories_RecordStatus ON dbo.CrewServiceHistories (RecordStatusId);

CREATE TABLE dbo.AccountTypes (
    Id TINYINT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(20) NOT NULL UNIQUE
);

CREATE TABLE dbo.ChartOfAccounts (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Number NVARCHAR(20) NOT NULL UNIQUE,
    Name NVARCHAR(100) NOT NULL,
    ParentId INT NULL FOREIGN KEY REFERENCES dbo.ChartOfAccounts(Id),
    AccountTypeId TINYINT NOT NULL FOREIGN KEY REFERENCES dbo.AccountTypes(Id),
    Level INT NOT NULL CHECK (Level >= 1 AND Level <= 5),
    RecordStatusId TINYINT NOT NULL FOREIGN KEY REFERENCES dbo.Recordstatus(Id),
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CreatedBy NVARCHAR(100) NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy NVARCHAR(100) NULL,
    DeletedAt DATETIME2 NULL,
    DeletedBy NVARCHAR(100) NULL
);

CREATE INDEX IX_ChartOfAccounts_RecordStatus ON dbo.ChartOfAccounts (RecordStatusId);
CREATE INDEX IX_ChartOfAccounts_ParentId ON dbo.ChartOfAccounts (ParentId) WHERE ParentId IS NOT NULL;
CREATE INDEX IX_ChartOfAccounts_Level ON dbo.ChartOfAccounts (Level);
CREATE INDEX IX_ChartOfAccounts_AccountType ON dbo.ChartOfAccounts (AccountTypeId);

CREATE TABLE dbo.Budgets (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ShipId INT NOT NULL FOREIGN KEY REFERENCES dbo.Ships(Id),
    ChartOfAccountId INT NOT NULL FOREIGN KEY REFERENCES dbo.ChartOfAccounts(Id),
    AccountPeriod DATETIME2 NOT NULL,
    BudgetValue DECIMAL NOT NULL,
    RecordStatusId TINYINT NOT NULL FOREIGN KEY REFERENCES dbo.Recordstatus(Id),
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CreatedBy NVARCHAR(100) NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy NVARCHAR(100) NULL,
    DeletedAt DATETIME2 NULL,
    DeletedBy NVARCHAR(100) NULL
);

CREATE INDEX IX_Budgets_RecordStatus ON dbo.Budgets (RecordStatusId);

CREATE TABLE dbo.AccountTransactions (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ShipId INT NOT NULL FOREIGN KEY REFERENCES dbo.Ships(Id),
    ChartOfAccountId INT NOT NULL FOREIGN KEY REFERENCES dbo.ChartOfAccounts(Id),
    AccountPeriod DATETIME2 NOT NULL,
    ActualValue DECIMAL NOT NULL,
    RecordStatusId TINYINT NOT NULL FOREIGN KEY REFERENCES dbo.Recordstatus(Id),
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    CreatedBy NVARCHAR(100) NULL,
    UpdatedAt DATETIME2 NULL,
    UpdatedBy NVARCHAR(100) NULL,
    DeletedAt DATETIME2 NULL,
    DeletedBy NVARCHAR(100) NULL
);

CREATE INDEX IX_AccountTransactions_RecordStatus ON dbo.AccountTransactions (RecordStatusId);

INSERT INTO dbo.Recordstatus (Name) 
VALUES 
    ('Active'), 
    ('Inactive'), 
    ('Deleted');

INSERT INTO dbo.Roles (Name) 
VALUES 
    ('Administrator'), 
    ('Staff');

INSERT INTO dbo.Ranks (Name) 
VALUES 
    ('Master'), 
    ('Chief Engineer'), 
    ('Chief Officer'), 
    ('Cadet'), 
    ('Oiler');

INSERT INTO dbo.AccountTypes (Name) 
VALUES 
    ('Summary'), 
    ('Detail');

INSERT INTO dbo.Users (RoleId, CrewMemberId, Username, Password, FirstName, LastName, BirthDate, Nationality, RecordStatusId, CreatedAt, CreatedBy)
values 
    (1, NULL, 'admin', '$2a$11$NPBpfbLsTqZSG2TG8RrlsOzsx4suFZJ2ezKl/iE7yTA5rNpqB.fe.', 'Admin', '1', '1995-05-25', 'Indonesia', 1, SYSDATETIME(), 'System'),
    (2, 'CREW001', 'james.wilson', '$2a$11$NPBpfbLsTqZSG2TG8RrlsOzsx4suFZJ2ezKl/iE7yTA5rNpqB.fe.', 'James', 'Wilson', '1985-01-10', 'Philippines', 1, SYSDATETIME(), 'System'),
    (2, 'CREW002', 'michael.brown', '$2a$11$NPBpfbLsTqZSG2TG8RrlsOzsx4suFZJ2ezKl/iE7yTA5rNpqB.fe.', 'Michael', 'Brown', '1987-05-20', 'India', 1, SYSDATETIME(), 'System'),
    (2, 'CREW003', 'robert.davis', '$2a$11$NPBpfbLsTqZSG2TG8RrlsOzsx4suFZJ2ezKl/iE7yTA5rNpqB.fe.', 'Robert', 'Davis', '1992-09-14', 'Indonesia', 1, SYSDATETIME(), 'System'),
    (2, 'CREW004', 'william.garcia', '$2a$11$NPBpfbLsTqZSG2TG8RrlsOzsx4suFZJ2ezKl/iE7yTA5rNpqB.fe.', 'William', 'Garcia', '1989-11-30', 'Philippines', 1, SYSDATETIME(), 'System'),
    (2, 'CREW005', 'david.martinez', '$2a$11$NPBpfbLsTqZSG2TG8RrlsOzsx4suFZJ2ezKl/iE7yTA5rNpqB.fe.', 'David', 'Martinez', '1991-02-18', 'China', 1, SYSDATETIME(), 'System'),
    (2, 'CREW006', 'richard.anderson', '$2a$11$NPBpfbLsTqZSG2TG8RrlsOzsx4suFZJ2ezKl/iE7yTA5rNpqB.fe.', 'Richard', 'Anderson', '1993-06-25', 'Myanmar', 1, SYSDATETIME(), 'System'),
    (2, 'CREW007', 'joseph.thomas', '$2a$11$NPBpfbLsTqZSG2TG8RrlsOzsx4suFZJ2ezKl/iE7yTA5rNpqB.fe.', 'Joseph', 'Thomas', '1986-08-08', 'Vietnam', 1, SYSDATETIME(), 'System'),
    (2, 'CREW008', 'charles.lee', '$2a$11$NPBpfbLsTqZSG2TG8RrlsOzsx4suFZJ2ezKl/iE7yTA5rNpqB.fe.', 'Charles', 'Lee', '1994-12-03', 'India', 1, SYSDATETIME(), 'System'),
    (2, 'CREW009', 'thomas.white', '$2a$11$NPBpfbLsTqZSG2TG8RrlsOzsx4suFZJ2ezKl/iE7yTA5rNpqB.fe.', 'Thomas', 'White', '1990-04-17', 'Indonesia', 1, SYSDATETIME(), 'System'),
    (2, 'CREW010', 'daniel.harris', '$2a$11$NPBpfbLsTqZSG2TG8RrlsOzsx4suFZJ2ezKl/iE7yTA5rNpqB.fe.', 'Daniel', 'Harris', '1988-10-22', 'Philippines', 1, SYSDATETIME(), 'System');

INSERT INTO dbo.Ships (Code, Name, FiscalYear, RecordStatusId, CreatedAt, CreatedBy)
VALUES
    ('SHP001', 'Pacific Explorer', '0112', 1, SYSDATETIME(), 'Admin'),
    ('SHP002', 'Atlantic Voyager', '0309', 1, SYSDATETIME(), 'Admin'),
    ('SHP003', 'Indian Ocean Star', '0602', 1, SYSDATETIME(), 'Admin'),
    ('SHP004', 'Mediterranean Pearl', '0805', 1, SYSDATETIME(), 'Admin'),
    ('SHP005', 'Arctic Navigator', '0108', 1, SYSDATETIME(), 'Admin'),
    ('SHP006', 'Baltic Trader', '0512', 1, SYSDATETIME(), 'Admin');

-- Pacific Explorer (Ship Id: 1) - Current Active Crew
INSERT INTO dbo.CrewServiceHistories (UserId, RankId, ShipId, SignOnDate, SignOffDate, EndOfContractDate, RecordStatusId, CreatedAt, CreatedBy)
VALUES
    (2, 1, 1, '2025-01-15', NULL, '2026-02-14', 1, SYSDATETIME(), 'Admin'),  -- James Wilson as Master
    (3, 2, 1, '2025-01-15', NULL, '2026-02-14', 1, SYSDATETIME(), 'Admin'),  -- Michael Brown as Chief Engineer
    (4, 3, 1, '2025-11-05', NULL, '2026-02-14', 1, SYSDATETIME(), 'Admin'),  -- Robert Davis as Chief Officer
    (5, 4, 1, '2025-06-01', NULL, '2026-05-31', 1, SYSDATETIME(), 'Admin');  -- William Garcia as Cadet

-- Atlantic Voyager (Ship Id: 2) - Current Active Crew
INSERT INTO dbo.CrewServiceHistories (UserId, RankId, ShipId, SignOnDate, SignOffDate, EndOfContractDate, RecordStatusId, CreatedAt, CreatedBy)
VALUES
    (6, 1, 2, '2025-03-02', NULL, '2025-09-01', 1, SYSDATETIME(), 'Admin'),   -- David Martinez as Master
    (7, 2, 2, '2025-03-02', NULL, '2025-09-01', 1, SYSDATETIME(), 'Admin'),   -- Richard Anderson as Chief Engineer
    (8, 5, 2, '2025-03-02', '2025-05-02', '2025-05-31', 1, SYSDATETIME(), 'Admin');  -- Joseph Thomas as Oiler

-- Indian Ocean Star (Ship Id: 3) - Current Active Crew
INSERT INTO dbo.CrewServiceHistories (UserId, RankId, ShipId, SignOnDate, SignOffDate, EndOfContractDate, RecordStatusId, CreatedAt, CreatedBy)
VALUES
    (9, 1, 3, '2025-06-10', NULL, '2026-02-10', 1, SYSDATETIME(), 'Admin'),  -- Charles Lee as Master
    (10, 3, 3, '2025-06-10', NULL, '2026-02-10', 1, SYSDATETIME(), 'Admin'),  -- Thomas White as Chief Officer
    (11, 4, 3, '2025-06-01', NULL, '2025-08-10', 1, SYSDATETIME(), 'Admin');  -- Daniel Harris as Cadet

-- Level 1: Summary Accounts (Account Type: Summary = 1)
INSERT INTO dbo.ChartOfAccounts (Number, Name, ParentId, AccountTypeId, Level, RecordStatusId, CreatedAt, CreatedBy)
VALUES
    ('1000', 'Assets', NULL, 1, 1, 1, SYSDATETIME(), 'Admin'),
    ('2000', 'Liabilities', NULL, 1, 1, 1, SYSDATETIME(), 'Admin'),
    ('3000', 'Equity', NULL, 1, 1, 1, SYSDATETIME(), 'Admin'),
    ('4000', 'Revenue', NULL, 1, 1, 1, SYSDATETIME(), 'Admin'),
    ('5000', 'Operating Expenses', NULL, 1, 1, 1, SYSDATETIME(), 'Admin');

-- Level 2: Sub-categories
INSERT INTO dbo.ChartOfAccounts (Number, Name, ParentId, AccountTypeId, Level, RecordStatusId, CreatedAt, CreatedBy)
VALUES
    ('1100', 'Current Assets', 1, 1, 2, 1, SYSDATETIME(), 'Admin'),
    ('1200', 'Fixed Assets', 1, 1, 2, 1, SYSDATETIME(), 'Admin'),
    ('2100', 'Current Liabilities', 2, 1, 2, 1, SYSDATETIME(), 'Admin'),
    ('2200', 'Long-term Liabilities', 2, 1, 2, 1, SYSDATETIME(), 'Admin'),
    ('4100', 'Operating Revenue', 4, 1, 2, 1, SYSDATETIME(), 'Admin'),
    ('4200', 'Other Revenue', 4, 1, 2, 1, SYSDATETIME(), 'Admin'),
    ('5100', 'Crew Expenses', 5, 1, 2, 1, SYSDATETIME(), 'Admin'),
    ('5200', 'Vessel Operating Expenses', 5, 1, 2, 1, SYSDATETIME(), 'Admin'),
    ('5300', 'Administrative Expenses', 5, 1, 2, 1, SYSDATETIME(), 'Admin');

-- Level 3: Detail accounts (Account Type: Detail = 2)
INSERT INTO dbo.ChartOfAccounts (Number, Name, ParentId, AccountTypeId, Level, RecordStatusId, CreatedAt, CreatedBy)
VALUES
    ('1110', 'Cash and Cash Equivalents', 6, 2, 3, 1, SYSDATETIME(), 'Admin'),
    ('1120', 'Accounts Receivable', 6, 2, 3, 1, SYSDATETIME(), 'Admin'),
    ('1130', 'Inventory', 6, 2, 3, 1, SYSDATETIME(), 'Admin'),
    ('1210', 'Vessels', 7, 2, 3, 1, SYSDATETIME(), 'Admin'),
    ('1220', 'Equipment', 7, 2, 3, 1, SYSDATETIME(), 'Admin'),
    ('1230', 'Accumulated Depreciation', 7, 2, 3, 1, SYSDATETIME(), 'Admin'),
    ('2110', 'Accounts Payable', 8, 2, 3, 1, SYSDATETIME(), 'Admin'),
    ('2120', 'Accrued Expenses', 8, 2, 3, 1, SYSDATETIME(), 'Admin'),
    ('4110', 'Freight Revenue', 10, 2, 3, 1, SYSDATETIME(), 'Admin'),
    ('4120', 'Charter Revenue', 10, 2, 3, 1, SYSDATETIME(), 'Admin'),
    ('5110', 'Crew Wages', 12, 2, 3, 1, SYSDATETIME(), 'Admin'),
    ('5120', 'Crew Benefits', 12, 2, 3, 1, SYSDATETIME(), 'Admin'),
    ('5130', 'Crew Training', 12, 2, 3, 1, SYSDATETIME(), 'Admin'),
    ('5210', 'Fuel Costs', 13, 2, 3, 1, SYSDATETIME(), 'Admin'),
    ('5220', 'Port Charges', 13, 2, 3, 1, SYSDATETIME(), 'Admin'),
    ('5230', 'Maintenance and Repairs', 13, 2, 3, 1, SYSDATETIME(), 'Admin'),
    ('5240', 'Insurance', 13, 2, 3, 1, SYSDATETIME(), 'Admin'),
    ('5310', 'Office Supplies', 14, 2, 3, 1, SYSDATETIME(), 'Admin'),
    ('5320', 'IT and Communications', 14, 2, 3, 1, SYSDATETIME(), 'Admin');

INSERT INTO dbo.Budgets (ShipId, ChartOfAccountId, AccountPeriod, BudgetValue, RecordStatusId, CreatedAt, CreatedBy)
VALUES
    (1, 25, '2025-01-01', 50000, 1, SYSDATETIME(), 'Admin'),
    (1, 25, '2025-02-01', 50000, 1, SYSDATETIME(), 'Admin'),
    (1, 25, '2025-03-01', 50000, 1, SYSDATETIME(), 'Admin'),
    (1, 26, '2025-01-01', 10000, 1, SYSDATETIME(), 'Admin'),
    (1, 26, '2025-02-01', 10000, 1, SYSDATETIME(), 'Admin'),
    (1, 27, '2025-01-01', 5000, 1, SYSDATETIME(), 'Admin'),
    (1, 28, '2025-01-01', 80000, 1, SYSDATETIME(), 'Admin'),
    (1, 28, '2025-02-01', 85000, 1, SYSDATETIME(), 'Admin'),
    (1, 28, '2025-03-01', 82000, 1, SYSDATETIME(), 'Admin'),
    (1, 29, '2025-01-01', 15000, 1, SYSDATETIME(), 'Admin'),
    (1, 29, '2025-02-01', 15000, 1, SYSDATETIME(), 'Admin'),
    (1, 30, '2025-01-01', 20000, 1, SYSDATETIME(), 'Admin'),
    (1, 31, '2025-01-01', 12000, 1, SYSDATETIME(), 'Admin');

INSERT INTO dbo.Budgets (ShipId, ChartOfAccountId, AccountPeriod, BudgetValue, RecordStatusId, CreatedAt, CreatedBy)
VALUES
    (2, 25, '2025-03-01', 48000, 1, SYSDATETIME(), 'Admin'),
    (2, 25, '2025-04-01', 48000, 1, SYSDATETIME(), 'Admin'),
    (2, 26, '2025-03-01', 9500, 1, SYSDATETIME(), 'Admin'),
    (2, 28, '2025-03-01', 75000, 1, SYSDATETIME(), 'Admin'),
    (2, 28, '2025-04-01', 78000, 1, SYSDATETIME(), 'Admin'),
    (2, 29, '2025-03-01', 14000, 1, SYSDATETIME(), 'Admin'),
    (2, 30, '2025-04-01', 18000, 1, SYSDATETIME(), 'Admin');

INSERT INTO dbo.Budgets (ShipId, ChartOfAccountId, AccountPeriod, BudgetValue, RecordStatusId, CreatedAt, CreatedBy)
VALUES
    (3, 25, '2025-06-01', 52000, 1, SYSDATETIME(), 'Admin'),
    (3, 25, '2025-07-01', 52000, 1, SYSDATETIME(), 'Admin'),
    (3, 26, '2025-06-01', 10500, 1, SYSDATETIME(), 'Admin'),
    (3, 28, '2025-07-01', 90000, 1, SYSDATETIME(), 'Admin'),
    (3, 28, '2025-08-01', 88000, 1, SYSDATETIME(), 'Admin'),
    (3, 29, '2025-08-01', 16000, 1, SYSDATETIME(), 'Admin'),
    (3, 30, '2025-09-01', 22000, 1, SYSDATETIME(), 'Admin');

INSERT INTO dbo.AccountTransactions (ShipId, ChartOfAccountId, AccountPeriod, ActualValue, RecordStatusId, CreatedAt, CreatedBy)
VALUES
    (1, 25, '2025-01-01', 49500.00, 1, SYSDATETIME(), 'Admin'),
    (1, 25, '2025-02-01', 51200.00, 1, SYSDATETIME(), 'Admin'),
    (1, 25, '2025-03-01', 50300.00, 1, SYSDATETIME(), 'Admin'),
    (1, 26, '2025-01-01', 9800.00, 1, SYSDATETIME(), 'Admin'),
    (1, 26, '2025-02-01', 10200.00, 1, SYSDATETIME(), 'Admin'),
    (1, 27, '2025-01-01', 4800.00, 1, SYSDATETIME(), 'Admin'),
    (1, 28, '2025-01-01', 82500.00, 1, SYSDATETIME(), 'Admin'),
    (1, 28, '2025-02-01', 84200.00, 1, SYSDATETIME(), 'Admin'),
    (1, 28, '2025-03-01', 81000.00, 1, SYSDATETIME(), 'Admin'),
    (1, 29, '2025-01-01', 14800.00, 1, SYSDATETIME(), 'Admin'),
    (1, 29, '2025-02-01', 15200.00, 1, SYSDATETIME(), 'Admin'),
    (1, 30, '2025-01-01', 19500.00, 1, SYSDATETIME(), 'Admin'),
    (1, 31, '2025-01-01', 12000.00, 1, SYSDATETIME(), 'Admin'),
    (1, 23, '2025-01-01', 250000.00, 1, SYSDATETIME(), 'Admin'),
    (1, 23, '2025-02-01', 260000.00, 1, SYSDATETIME(), 'Admin'),
    (1, 24, '2025-01-01', 50000.00, 1, SYSDATETIME(), 'Admin');

INSERT INTO dbo.AccountTransactions (ShipId, ChartOfAccountId, AccountPeriod, ActualValue, RecordStatusId, CreatedAt, CreatedBy)
VALUES
    (2, 25, '2025-03-01', 47800.00, 1, SYSDATETIME(), 'Admin'),
    (2, 25, '2025-04-01', 48500.00, 1, SYSDATETIME(), 'Admin'),
    (2, 26, '2025-03-01', 9400.00, 1, SYSDATETIME(), 'Admin'),
    (2, 28, '2025-03-01', 76500.00, 1, SYSDATETIME(), 'Admin'),
    (2, 28, '2025-04-01', 77200.00, 1, SYSDATETIME(), 'Admin'),
    (2, 29, '2025-03-01', 13800.00, 1, SYSDATETIME(), 'Admin'),
    (2, 30, '2025-04-01', 17500.00, 1, SYSDATETIME(), 'Admin'),
    (2, 23, '2025-03-01', 230000.00, 1, SYSDATETIME(), 'Admin'),
    (2, 23, '2025-04-01', 240000.00, 1, SYSDATETIME(), 'Admin');

INSERT INTO dbo.AccountTransactions (ShipId, ChartOfAccountId, AccountPeriod, ActualValue, RecordStatusId, CreatedAt, CreatedBy)
VALUES
    (3, 25, '2025-06-01', 51500.00, 1, SYSDATETIME(), 'Admin'),    -- Crew Wages Jan
    (3, 25, '2025-07-01', 52300.00, 1, SYSDATETIME(), 'Admin'),    -- Crew Wages Feb
    (3, 26, '2025-06-01', 10300.00, 1, SYSDATETIME(), 'Admin'),    -- Crew Benefits Jan
    (3, 28, '2025-07-01', 91000.00, 1, SYSDATETIME(), 'Admin'),    -- Fuel Costs Jan
    (3, 28, '2025-08-01', 87500.00, 1, SYSDATETIME(), 'Admin'),    -- Fuel Costs Feb
    (3, 29, '2025-08-01', 15800.00, 1, SYSDATETIME(), 'Admin'),    -- Port Charges Jan
    (3, 30, '2025-09-01', 21500.00, 1, SYSDATETIME(), 'Admin'),    -- Maintenance Jan
    (3, 23, '2025-06-01', 280000.00, 1, SYSDATETIME(), 'Admin'),   -- Freight Revenue Jan
    (3, 23, '2025-07-01', 290000.00, 1, SYSDATETIME(), 'Admin'),   -- Freight Revenue Feb
    (3, 24, '2025-08-01', 60000.00, 1, SYSDATETIME(), 'Admin');    -- Charter Revenue Jan