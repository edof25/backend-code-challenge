CREATE OR ALTER PROCEDURE dbo.User_Create
    @RoleId TINYINT,
    @CrewMemberId NVARCHAR(100),
    @Username NVARCHAR(100),
    @Password NVARCHAR(255),
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @BirthDate DATE,
    @Nationality NVARCHAR(100),
    @CreatedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if username already exists
    IF EXISTS (SELECT 1 FROM dbo.Users WHERE Username = @Username)
    BEGIN
        RAISERROR('Username already exists', 16, 1);
        RETURN;
    END

    DECLARE @NewId INT;

    INSERT INTO dbo.Users (
        RoleId,
        CrewMemberId,
        Username,
        Password,
        FirstName,
        LastName,
        BirthDate,
        Nationality,
        RecordStatusId,
        CreatedAt,
        CreatedBy
    )
    VALUES (
        @RoleId,
        @CrewMemberId,
        @Username,
        @Password,
        @FirstName,
        @LastName,
        @BirthDate,
        @Nationality,
        1, -- Active status
        SYSDATETIME(),
        @CreatedBy
    );

    SET @NewId = SCOPE_IDENTITY();

    -- Return the newly created user
    EXEC dbo.User_Get @Id = @NewId;
END
GO
