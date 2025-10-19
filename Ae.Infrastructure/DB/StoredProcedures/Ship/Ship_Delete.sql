CREATE OR ALTER PROCEDURE dbo.Ship_Delete
    @Id INT,
    @DeletedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if ship exists and is not already deleted
    IF NOT EXISTS (SELECT 1 FROM dbo.Ships WHERE Id = @Id AND RecordStatusId != 3)
    BEGIN
        RAISERROR('Ship not found', 16, 1);
        RETURN;
    END

    -- Soft delete by updating RecordStatusId to 3 (Deleted)
    UPDATE dbo.Ships
    SET
        RecordStatusId = 3,
        DeletedAt = SYSDATETIME(),
        DeletedBy = @DeletedBy
    WHERE Id = @Id;

    -- Return success indicator
    SELECT 1 AS Success;
END
GO
