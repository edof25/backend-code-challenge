CREATE OR ALTER PROCEDURE dbo.CrewServiceHistory_Get
    @Id INT = NULL,
    @UserId INT = NULL,
    @ShipId INT = NULL,
    @SearchTerm NVARCHAR(255) = NULL,
    @SortBy NVARCHAR(50) = NULL,
    @SortOrder NVARCHAR(4) = 'ASC',
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    -- Validate that at least one parameter is provided
    IF @Id IS NULL AND @UserId IS NULL AND @ShipId IS NULL
    BEGIN
        RAISERROR('At least one parameter (@Id, @UserId, or @ShipId) must be provided', 16, 1);
        RETURN;
    END

    -- Set default sort order if not provided or invalid
    IF @SortOrder IS NULL OR @SortOrder NOT IN ('ASC', 'DESC')
        SET @SortOrder = 'ASC';

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    -- Get total count
    SELECT COUNT(csh.Id) AS TotalCount
    FROM dbo.CrewServiceHistories csh
    INNER JOIN dbo.Users u ON csh.UserId = u.Id
    INNER JOIN dbo.Ranks r ON csh.RankId = r.Id
    INNER JOIN dbo.Ships s ON csh.ShipId = s.Id
    INNER JOIN dbo.Recordstatus rs ON csh.RecordStatusId = rs.Id
    WHERE csh.RecordStatusId != 3 -- Exclude deleted records
        AND (@Id IS NULL OR csh.Id = @Id)
        AND (@UserId IS NULL OR csh.UserId = @UserId)
        AND (@ShipId IS NULL OR csh.ShipId = @ShipId)
        AND (@ShipId IS NULL OR csh.SignOffDate IS NULL) -- Exclude signed-off crew when filtering by ShipId
        AND (
            @SearchTerm IS NULL
            OR r.Name LIKE '%' + @SearchTerm + '%'
            OR u.CrewMemberId LIKE '%' + @SearchTerm + '%'
            OR u.FirstName LIKE '%' + @SearchTerm + '%'
            OR u.LastName LIKE '%' + @SearchTerm + '%'
            OR u.Nationality LIKE '%' + @SearchTerm + '%'
            OR CONVERT(NVARCHAR, csh.SignOnDate, 106) LIKE '%' + @SearchTerm + '%'
        );

    -- Get results with dynamic sorting
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
      AND (@ShipId IS NULL OR csh.SignOffDate IS NULL) -- Exclude signed-off crew when filtering by ShipId
      AND (
          @SearchTerm IS NULL
          OR r.Name LIKE '%' + @SearchTerm + '%'
          OR u.CrewMemberId LIKE '%' + @SearchTerm + '%'
          OR u.FirstName LIKE '%' + @SearchTerm + '%'
          OR u.LastName LIKE '%' + @SearchTerm + '%'
          OR u.Nationality LIKE '%' + @SearchTerm + '%'
          OR CONVERT(NVARCHAR, csh.SignOnDate, 106) LIKE '%' + @SearchTerm + '%'
      )
    ORDER BY
        CASE WHEN @SortBy = 'RankName' AND @SortOrder = 'ASC' THEN r.Name END ASC,
        CASE WHEN @SortBy = 'RankName' AND @SortOrder = 'DESC' THEN r.Name END DESC,
        CASE WHEN @SortBy = 'CrewMemberId' AND @SortOrder = 'ASC' THEN u.CrewMemberId END ASC,
        CASE WHEN @SortBy = 'CrewMemberId' AND @SortOrder = 'DESC' THEN u.CrewMemberId END DESC,
        CASE WHEN @SortBy = 'FirstName' AND @SortOrder = 'ASC' THEN u.FirstName END ASC,
        CASE WHEN @SortBy = 'FirstName' AND @SortOrder = 'DESC' THEN u.FirstName END DESC,
        CASE WHEN @SortBy = 'LastName' AND @SortOrder = 'ASC' THEN u.LastName END ASC,
        CASE WHEN @SortBy = 'LastName' AND @SortOrder = 'DESC' THEN u.LastName END DESC,
        CASE WHEN @SortBy = 'Age' AND @SortOrder = 'ASC' THEN u.BirthDate END DESC, -- Reverse order for age
        CASE WHEN @SortBy = 'Age' AND @SortOrder = 'DESC' THEN u.BirthDate END ASC,
        CASE WHEN @SortBy = 'Nationality' AND @SortOrder = 'ASC' THEN u.Nationality END ASC,
        CASE WHEN @SortBy = 'Nationality' AND @SortOrder = 'DESC' THEN u.Nationality END DESC,
        CASE WHEN @SortBy = 'SignOnDate' AND @SortOrder = 'ASC' THEN csh.SignOnDate END ASC,
        CASE WHEN @SortBy = 'SignOnDate' AND @SortOrder = 'DESC' THEN csh.SignOnDate END DESC,
        CASE WHEN @SortBy IS NULL THEN csh.SignOnDate END DESC -- Default sort
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO
