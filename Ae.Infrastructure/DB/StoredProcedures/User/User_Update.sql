CREATE OR ALTER PROCEDURE dbo.User_Update
    @Id INT,
    @RoleId TINYINT,
    @Username NVARCHAR(255),
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @BirthDate DATE,
    @Nationality NVARCHAR(100),
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

    -- Check if username is taken by another user
    IF EXISTS (SELECT 1 FROM dbo.Users WHERE Username = @Username AND Id != @Id)
    BEGIN
        RAISERROR('Username already exists', 16, 1);
        RETURN;
    END

    UPDATE dbo.Users
    SET
        RoleId = @RoleId,
        Username = @Username,
        FirstName = @FirstName,
        LastName = @LastName,
        BirthDate = @BirthDate,
        Nationality = @Nationality,
        UpdatedAt = SYSDATETIME(),
        UpdatedBy = @UpdatedBy
    WHERE Id = @Id;

    -- Return the updated user
    EXEC dbo.User_Get @Id = @Id;
END
GO
