CREATE OR ALTER PROCEDURE dbo.User_Delete
    @Id INT,
    @DeletedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if user exists
    IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Id = @Id AND RecordStatusId != 3)
    BEGIN
        RAISERROR('User not found', 16, 1);
        RETURN;
    END

    -- Soft delete by updating RecordStatusId to 3 (Deleted)
    UPDATE dbo.Users
    SET
        RecordStatusId = 3,
        DeletedAt = SYSDATETIME(),
        DeletedBy = @DeletedBy
    WHERE Id = @Id;

    -- Return success indicator
    SELECT 1 AS Success;
END
GO
