CREATE OR ALTER PROCEDURE dbo.Ship_Get
    @Id INT = NULL,
    @Code NVARCHAR(50) = NULL,
    @UserId INT = NULL,
    @SearchTerm NVARCHAR(255) = NULL,
    @SortBy NVARCHAR(50) = NULL,
    @SortOrder NVARCHAR(4) = 'ASC',
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

    -- Set default sort order if not provided or invalid
    IF @SortOrder IS NULL OR @SortOrder NOT IN ('ASC', 'DESC')
        SET @SortOrder = 'ASC';

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

    -- Get paged results with dynamic sorting
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
    ORDER BY
        CASE WHEN @SortBy = 'Code' AND @SortOrder = 'ASC' THEN s.Code END ASC,
        CASE WHEN @SortBy = 'Code' AND @SortOrder = 'DESC' THEN s.Code END DESC,
        CASE WHEN @SortBy = 'Name' AND @SortOrder = 'ASC' THEN s.Name END ASC,
        CASE WHEN @SortBy = 'Name' AND @SortOrder = 'DESC' THEN s.Name END DESC,
        CASE WHEN @SortBy = 'FiscalYear' AND @SortOrder = 'ASC' THEN s.FiscalYear END ASC,
        CASE WHEN @SortBy = 'FiscalYear' AND @SortOrder = 'DESC' THEN s.FiscalYear END DESC,
        CASE WHEN @SortBy = 'CreatedAt' AND @SortOrder = 'ASC' THEN s.CreatedAt END ASC,
        CASE WHEN @SortBy = 'CreatedAt' AND @SortOrder = 'DESC' THEN s.CreatedAt END DESC,
        CASE WHEN @SortBy IS NULL THEN s.CreatedAt END DESC -- Default sort
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO
