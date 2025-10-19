CREATE OR ALTER PROCEDURE dbo.FinancialReport_GetSummaryByShipAndPeriod
    @ShipId INT,
    @AccountPeriod DATETIME2
AS
BEGIN
    SET NOCOUNT ON;

    -- Verify ship exists and is active
    DECLARE @FiscalYearFormat NVARCHAR(100);
    DECLARE @ShipRecordStatusId TINYINT;

    SELECT
        @FiscalYearFormat = FiscalYear,
        @ShipRecordStatusId = RecordStatusId
    FROM dbo.Ships
    WHERE Id = @ShipId;

    -- If ship doesn't exist or is not active, return empty result
    IF @FiscalYearFormat IS NULL OR @ShipRecordStatusId <> 1
    BEGIN
        SELECT
            '' AS ChartOfAccountNumber,
            '' AS ChartOfAccountName,
            0 AS Actual,
            0 AS Budget,
            0 AS VarianceActual,
            0 AS ActualYTD,
            0 AS BudgetYTD,
            0 AS VarianceYTD
        WHERE 1 = 0; -- Return empty result set with proper columns
        RETURN;
    END

    -- Parse fiscal year format (MMDD where MM is start month, DD is end month)
    -- Examples: '0112' = Jan-Dec, '0403' = Apr-Mar
    DECLARE @FiscalStartMonth INT;
    DECLARE @FiscalEndMonth INT;
    DECLARE @FiscalYearStart DATETIME2;

    -- Extract start and end months from the format MMDD
    SET @FiscalStartMonth = TRY_CAST(LEFT(@FiscalYearFormat, 2) AS INT);
    SET @FiscalEndMonth = TRY_CAST(RIGHT(@FiscalYearFormat, 2) AS INT);

    -- Validate fiscal year format
    IF @FiscalStartMonth IS NULL OR @FiscalEndMonth IS NULL
       OR @FiscalStartMonth < 1 OR @FiscalStartMonth > 12
       OR @FiscalEndMonth < 1 OR @FiscalEndMonth > 12
    BEGIN
        -- Default to January if format is invalid
        SET @FiscalStartMonth = 1;
    END

    -- Calculate fiscal year start date based on the account period
    -- If the account period month is >= fiscal start month, fiscal year started this calendar year
    -- Otherwise, fiscal year started last calendar year
    IF MONTH(@AccountPeriod) >= @FiscalStartMonth
    BEGIN
        SET @FiscalYearStart = DATEFROMPARTS(YEAR(@AccountPeriod), @FiscalStartMonth, 1);
    END
    ELSE
    BEGIN
        SET @FiscalYearStart = DATEFROMPARTS(YEAR(@AccountPeriod) - 1, @FiscalStartMonth, 1);
    END

    -- Calculate period start (first day of the month)
    DECLARE @PeriodStart DATETIME2 = DATEFROMPARTS(YEAR(@AccountPeriod), MONTH(@AccountPeriod), 1);

    -- Calculate period end (last day of the month)
    DECLARE @PeriodEnd DATETIME2 = EOMONTH(@AccountPeriod);

    WITH CurrentPeriodData AS (
        -- Get current period actuals for summary accounts
        -- Summary accounts aggregate values from their detail children
        SELECT
            coa.Id AS ChartOfAccountId,
            coa.Number AS ChartOfAccountNumber,
            coa.Name AS ChartOfAccountName,
            ISNULL(SUM(at.ActualValue), 0) AS Actual,
            ISNULL(SUM(b.BudgetValue), 0) AS Budget
        FROM dbo.ChartOfAccounts coa
        -- Get all child detail accounts (recursively)
        INNER JOIN dbo.ChartOfAccounts child ON
            (child.ParentId = coa.Id OR child.Id = coa.Id)
            AND child.AccountTypeId = 2 -- Detail accounts only
            AND child.RecordStatusId = 1
        LEFT JOIN dbo.AccountTransactions at ON child.Id = at.ChartOfAccountId
            AND at.ShipId = @ShipId
            AND at.AccountPeriod >= @PeriodStart
            AND at.AccountPeriod <= @PeriodEnd
            AND at.RecordStatusId = 1 -- Active
        LEFT JOIN dbo.Budgets b ON child.Id = b.ChartOfAccountId
            AND b.ShipId = @ShipId
            AND b.AccountPeriod >= @PeriodStart
            AND b.AccountPeriod <= @PeriodEnd
            AND b.RecordStatusId = 1 -- Active
        WHERE coa.AccountTypeId = 1 -- Summary accounts only
            AND coa.RecordStatusId = 1 -- Active
        GROUP BY coa.Id, coa.Number, coa.Name
    ),
    YTDData AS (
        -- Get YTD (Year-To-Date from fiscal year start) actuals and budgets for summary accounts
        SELECT
            coa.Id AS ChartOfAccountId,
            ISNULL(SUM(at.ActualValue), 0) AS ActualYTD,
            ISNULL(SUM(b.BudgetValue), 0) AS BudgetYTD
        FROM dbo.ChartOfAccounts coa
        -- Get all child detail accounts (recursively)
        INNER JOIN dbo.ChartOfAccounts child ON
            (child.ParentId = coa.Id OR child.Id = coa.Id)
            AND child.AccountTypeId = 2 -- Detail accounts only
            AND child.RecordStatusId = 1
        LEFT JOIN dbo.AccountTransactions at ON child.Id = at.ChartOfAccountId
            AND at.ShipId = @ShipId
            AND at.AccountPeriod >= @FiscalYearStart
            AND at.AccountPeriod <= @PeriodEnd
            AND at.RecordStatusId = 1 -- Active
        LEFT JOIN dbo.Budgets b ON child.Id = b.ChartOfAccountId
            AND b.ShipId = @ShipId
            AND b.AccountPeriod >= @FiscalYearStart
            AND b.AccountPeriod <= @PeriodEnd
            AND b.RecordStatusId = 1 -- Active
        WHERE coa.AccountTypeId = 1 -- Summary accounts only
            AND coa.RecordStatusId = 1 -- Active
        GROUP BY coa.Id
    )
    SELECT
        cp.ChartOfAccountNumber,
        cp.ChartOfAccountName,
        cp.Actual,
        cp.Budget,
        (cp.Actual - cp.Budget) AS VarianceActual,
        ytd.ActualYTD,
        ytd.BudgetYTD,
        (ytd.ActualYTD - ytd.BudgetYTD) AS VarianceYTD
    FROM CurrentPeriodData cp
    INNER JOIN YTDData ytd ON cp.ChartOfAccountId = ytd.ChartOfAccountId
    ORDER BY cp.ChartOfAccountNumber;
END
