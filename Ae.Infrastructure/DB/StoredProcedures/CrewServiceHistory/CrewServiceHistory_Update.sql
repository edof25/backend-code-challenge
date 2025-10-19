CREATE OR ALTER PROCEDURE dbo.CrewServiceHistory_Update
    @Id INT,
    @RankId TINYINT,
    @SignOnDate DATETIME2,
    @SignOffDate DATETIME2 = NULL,
    @EndOfContractDate DATETIME2,
    @UpdatedBy NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if record exists and is active
    IF NOT EXISTS (SELECT 1 FROM dbo.CrewServiceHistories WHERE Id = @Id AND RecordStatusId != 3)
    BEGIN
        RAISERROR('Crew service history not found or is deleted', 16, 1);
        RETURN;
    END

    -- Check if rank exists
    IF NOT EXISTS (SELECT 1 FROM dbo.Ranks WHERE Id = @RankId)
    BEGIN
        RAISERROR('Rank not found', 16, 1);
        RETURN;
    END

    UPDATE dbo.CrewServiceHistories
    SET
        RankId = @RankId,
        SignOnDate = @SignOnDate,
        SignOffDate = @SignOffDate,
        EndOfContractDate = @EndOfContractDate,
        UpdatedAt = SYSDATETIME(),
        UpdatedBy = @UpdatedBy
    WHERE Id = @Id;

    -- Return the updated record
    EXEC dbo.CrewServiceHistory_Get @Id = @Id;
END
GO
