CREATE OR ALTER PROCEDURE dbo.CrewServiceHistory_Delete
    @UserId INT,
    @ShipId INT,
    @DeletedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    -- Find the active crew service history record for this user and ship
    DECLARE @Id INT;

    SELECT @Id = Id
    FROM dbo.CrewServiceHistories
    WHERE UserId = @UserId
        AND ShipId = @ShipId
        AND RecordStatusId != 3
        AND SignOffDate IS NULL; -- Only active assignments

    IF @Id IS NULL
    BEGIN
        -- Return success = 0 to indicate record not found
        SELECT 0 AS Success;
        RETURN;
    END

    -- Soft delete the record
    UPDATE dbo.CrewServiceHistories
    SET
        RecordStatusId = 3, -- Deleted status
        DeletedAt = SYSDATETIME(),
        DeletedBy = @DeletedBy,
        SignOffDate = SYSDATETIME() -- Automatically sign off when deleting
    WHERE Id = @Id;

    -- Return success = 1
    SELECT 1 AS Success;
END
GO
