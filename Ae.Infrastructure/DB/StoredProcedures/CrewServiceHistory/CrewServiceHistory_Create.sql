CREATE OR ALTER PROCEDURE dbo.CrewServiceHistory_Create
    @UserId INT,
    @RankId TINYINT,
    @ShipId INT,
    @SignOnDate DATETIME2,
    @SignOffDate DATETIME2 = NULL,
    @EndOfContractDate DATETIME2,
    @CreatedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if user exists and is active
    IF NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Id = @UserId AND RecordStatusId != 3)
    BEGIN
        RAISERROR('User not found or is deleted', 16, 1);
        RETURN;
    END

    -- Check if ship exists and is active
    IF NOT EXISTS (SELECT 1 FROM dbo.Ships WHERE Id = @ShipId AND RecordStatusId != 3)
    BEGIN
        RAISERROR('Ship not found or is deleted', 16, 1);
        RETURN;
    END

    -- Check if rank exists
    IF NOT EXISTS (SELECT 1 FROM dbo.Ranks WHERE Id = @RankId)
    BEGIN
        RAISERROR('Rank not found', 16, 1);
        RETURN;
    END

    -- Check if there's an active assignment for this user on this ship
    IF EXISTS (
        SELECT 1
        FROM dbo.CrewServiceHistories
        WHERE UserId = @UserId
            AND ShipId = @ShipId
            AND RecordStatusId != 3
            AND SignOffDate IS NULL
    )
    BEGIN
        RAISERROR('User already has an active assignment on this ship', 16, 1);
        RETURN;
    END

    DECLARE @NewId INT;

    INSERT INTO dbo.CrewServiceHistories (
        UserId,
        RankId,
        ShipId,
        SignOnDate,
        SignOffDate,
        EndOfContractDate,
        RecordStatusId,
        CreatedAt,
        CreatedBy
    )
    VALUES (
        @UserId,
        @RankId,
        @ShipId,
        @SignOnDate,
        @SignOffDate,
        @EndOfContractDate,
        1, -- Active status
        SYSDATETIME(),
        @CreatedBy
    );

    SET @NewId = SCOPE_IDENTITY();

    -- Return the newly created record
    EXEC dbo.CrewServiceHistory_Get @Id = @NewId;
END
GO
