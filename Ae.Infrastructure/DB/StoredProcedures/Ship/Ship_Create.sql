CREATE OR ALTER PROCEDURE dbo.Ship_Create
    @Code NVARCHAR(50),
    @Name NVARCHAR(100),
    @FiscalYear NVARCHAR(10),
    @CreatedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if ship code already exists
    IF EXISTS (SELECT 1 FROM dbo.Ships WHERE Code = @Code AND RecordStatusId != 3)
    BEGIN
        RAISERROR('Ship code already exists', 16, 1);
        RETURN;
    END

    DECLARE @NewId INT;

    INSERT INTO dbo.Ships (
        Code,
        Name,
        FiscalYear,
        RecordStatusId,
        CreatedAt,
        CreatedBy
    )
    VALUES (
        @Code,
        @Name,
        @FiscalYear,
        1, -- Active status
        SYSDATETIME(),
        @CreatedBy
    );

    SET @NewId = SCOPE_IDENTITY();

    -- Return the newly created ship
    EXEC dbo.Ship_Get @Id = @NewId;
END
GO
