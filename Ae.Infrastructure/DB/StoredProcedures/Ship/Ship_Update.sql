CREATE OR ALTER PROCEDURE dbo.Ship_Update
    @Id INT,
    @Code NVARCHAR(50),
    @Name NVARCHAR(100),
    @FiscalYear NVARCHAR(10),
    @UpdatedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if ship exists and is not deleted
    IF NOT EXISTS (SELECT 1 FROM dbo.Ships WHERE Id = @Id AND RecordStatusId != 3)
    BEGIN
        RAISERROR('Ship not found', 16, 1);
        RETURN;
    END

    -- Check if ship code already exists for another ship
    IF EXISTS (SELECT 1 FROM dbo.Ships WHERE Code = @Code AND Id != @Id AND RecordStatusId != 3)
    BEGIN
        RAISERROR('Ship code already exists', 16, 1);
        RETURN;
    END

    UPDATE dbo.Ships
    SET
        Code = @Code,
        Name = @Name,
        FiscalYear = @FiscalYear,
        UpdatedAt = SYSDATETIME(),
        UpdatedBy = @UpdatedBy
    WHERE Id = @Id;

    -- Return the updated ship
    EXEC dbo.Ship_Get @Id = @Id;
END
GO
