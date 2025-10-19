CREATE OR ALTER PROCEDURE dbo.User_UpdatePassword
    @Id INT,
    @Password NVARCHAR(255),
    @UpdatedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if user exists
    IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Id = @Id AND RecordStatusId != 3)
    BEGIN
        RAISERROR('User not found', 16, 1);
        RETURN;
    END

    UPDATE dbo.Users
    SET
        Password = @Password,
        UpdatedAt = SYSDATETIME(),
        UpdatedBy = @UpdatedBy
    WHERE Id = @Id;

    -- Return success
    SELECT 1 AS Success;
END
GO
