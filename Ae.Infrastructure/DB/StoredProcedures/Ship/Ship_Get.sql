CREATE OR ALTER PROCEDURE dbo.Ship_Get
    @Id INT = NULL,
    @Code NVARCHAR(50) = NULL,
    @UserId INT = NULL,
    @SearchTerm NVARCHAR(255) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    -- Get total count
    SELECT COUNT(s.Id) AS TotalCount
    FROM dbo.Ships s
    INNER JOIN dbo.Recordstatus rs ON s.RecordStatusId = rs.Id
    WHERE s.RecordStatusId != 3 -- Exclude deleted ships
        AND (@Id IS NULL OR s.Id = @Id)
        AND (@Code IS NULL OR s.Code = @Code)
        AND (
            @UserId IS NULL
            OR EXISTS (
                SELECT 1
                FROM dbo.CrewServiceHistories csh
                WHERE csh.ShipId = s.Id
                    AND csh.UserId = @UserId
                    AND csh.RecordStatusId != 3
            )
        )
        AND (
            @SearchTerm IS NULL
            OR s.Code LIKE '%' + @SearchTerm + '%'
            OR s.Name LIKE '%' + @SearchTerm + '%'
            OR CAST(s.FiscalYear AS NVARCHAR) LIKE '%' + @SearchTerm + '%'
        );

    -- Get paged results
    SELECT
        s.Id,
        s.Code,
        s.Name,
        s.FiscalYear,
        s.RecordStatusId,
        s.CreatedAt,
        s.CreatedBy,
        s.UpdatedAt,
        s.UpdatedBy,
        s.DeletedAt,
        s.DeletedBy,
        rs.Name AS RecordStatusName
    FROM dbo.Ships s
    INNER JOIN dbo.Recordstatus rs ON s.RecordStatusId = rs.Id
    WHERE s.RecordStatusId != 3 -- Exclude deleted ships
        AND (@Id IS NULL OR s.Id = @Id)
        AND (@Code IS NULL OR s.Code = @Code)
        AND (
            @UserId IS NULL
            OR EXISTS (
                SELECT 1
                FROM dbo.CrewServiceHistories csh
                WHERE csh.ShipId = s.Id
                    AND csh.UserId = @UserId
                    AND csh.RecordStatusId != 3
            )
        )
        AND (
            @SearchTerm IS NULL
            OR s.Code LIKE '%' + @SearchTerm + '%'
            OR s.Name LIKE '%' + @SearchTerm + '%'
            OR CAST(s.FiscalYear AS NVARCHAR) LIKE '%' + @SearchTerm + '%'
        )
    ORDER BY s.CreatedAt DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO