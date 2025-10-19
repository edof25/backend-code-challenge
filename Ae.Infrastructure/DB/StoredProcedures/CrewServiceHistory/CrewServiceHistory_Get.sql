CREATE OR ALTER PROCEDURE dbo.CrewServiceHistory_Get
    @Id INT = NULL,
    @UserId INT = NULL,
    @ShipId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- Validate that at least one parameter is provided
    IF @Id IS NULL AND @UserId IS NULL AND @ShipId IS NULL
    BEGIN
        RAISERROR('At least one parameter (@Id, @UserId, or @ShipId) must be provided', 16, 1);
        RETURN;
    END

    SELECT
        csh.Id,
        csh.UserId,
        csh.RankId,
        csh.ShipId,
        csh.SignOnDate,
        csh.SignOffDate,
        csh.EndOfContractDate,
        csh.RecordStatusId,
        csh.CreatedAt,
        csh.CreatedBy,
        csh.UpdatedAt,
        csh.UpdatedBy,
        csh.DeletedAt,
        csh.DeletedBy,
        -- User information
        u.CrewMemberId,
        u.FirstName AS UserFirstName,
        u.LastName AS UserLastName,
        u.BirthDate,
        DATEDIFF(YEAR, u.BirthDate, GETDATE()) -
            CASE
                WHEN (MONTH(u.BirthDate) > MONTH(GETDATE())) OR
                     (MONTH(u.BirthDate) = MONTH(GETDATE()) AND DAY(u.BirthDate) > DAY(GETDATE()))
                THEN 1
                ELSE 0
            END AS Age,
        u.Nationality,
        -- Rank information
        r.Name AS RankName,
        -- Ship information
        s.Code AS ShipCode,
        s.Name AS ShipName,
        -- Record status
        rs.Name AS RecordStatusName,
        -- Calculated field for AssignedDate (for backwards compatibility)
        csh.SignOnDate AS AssignedDate
    FROM dbo.CrewServiceHistories csh
    INNER JOIN dbo.Users u ON csh.UserId = u.Id
    INNER JOIN dbo.Ranks r ON csh.RankId = r.Id
    INNER JOIN dbo.Ships s ON csh.ShipId = s.Id
    INNER JOIN dbo.Recordstatus rs ON csh.RecordStatusId = rs.Id
    WHERE csh.RecordStatusId != 3 -- Exclude deleted records
      AND (@Id IS NULL OR csh.Id = @Id)
      AND (@UserId IS NULL OR csh.UserId = @UserId)
      AND (@ShipId IS NULL OR csh.ShipId = @ShipId)
    ORDER BY csh.SignOnDate DESC;
END
GO
