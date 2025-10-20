CREATE OR ALTER PROCEDURE dbo.User_Get
    @Id INT = NULL,
    @Username NVARCHAR(255) = NULL,
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
    SELECT COUNT(*) AS TotalCount
    FROM dbo.Users u
    WHERE u.RecordStatusId != 3 -- Exclude deleted users
        AND (@Id IS NULL OR u.Id = @Id)
        AND (@Username IS NULL OR u.Username = @Username)
        AND (
            @SearchTerm IS NULL
            OR u.Username LIKE '%' + @SearchTerm + '%'
            OR u.FirstName LIKE '%' + @SearchTerm + '%'
            OR u.LastName LIKE '%' + @SearchTerm + '%'
            OR u.Nationality LIKE '%' + @SearchTerm + '%'
        );

    -- Get paged results with dynamic sorting
    SELECT
        u.Id,
        u.CrewMemberId,
        u.RoleId,
        u.Username,
        u.Password,
        u.FirstName,
        u.LastName,
        u.BirthDate,
        u.Nationality,
        u.RecordStatusId,
        u.CreatedAt,
        u.CreatedBy,
        u.UpdatedAt,
        u.UpdatedBy,
        u.DeletedAt,
        u.DeletedBy,
        r.Name AS RoleName,
        rs.Name AS RecordStatusName
    FROM dbo.Users u
    INNER JOIN dbo.Roles r ON u.RoleId = r.Id
    INNER JOIN dbo.Recordstatus rs ON u.RecordStatusId = rs.Id
    WHERE u.RecordStatusId != 3 -- Exclude deleted users
        AND (@Id IS NULL OR u.Id = @Id)
        AND (@Username IS NULL OR u.Username = @Username)
        AND (
            @SearchTerm IS NULL
            OR u.Username LIKE '%' + @SearchTerm + '%'
            OR u.FirstName LIKE '%' + @SearchTerm + '%'
            OR u.LastName LIKE '%' + @SearchTerm + '%'
            OR u.Nationality LIKE '%' + @SearchTerm + '%'
        )
    ORDER BY
        CASE WHEN @SortBy = 'Username' AND @SortOrder = 'ASC' THEN u.Username END ASC,
        CASE WHEN @SortBy = 'Username' AND @SortOrder = 'DESC' THEN u.Username END DESC,
        CASE WHEN @SortBy = 'FirstName' AND @SortOrder = 'ASC' THEN u.FirstName END ASC,
        CASE WHEN @SortBy = 'FirstName' AND @SortOrder = 'DESC' THEN u.FirstName END DESC,
        CASE WHEN @SortBy = 'LastName' AND @SortOrder = 'ASC' THEN u.LastName END ASC,
        CASE WHEN @SortBy = 'LastName' AND @SortOrder = 'DESC' THEN u.LastName END DESC,
        CASE WHEN @SortBy = 'RoleName' AND @SortOrder = 'ASC' THEN r.Name END ASC,
        CASE WHEN @SortBy = 'RoleName' AND @SortOrder = 'DESC' THEN r.Name END DESC,
        CASE WHEN @SortBy = 'Nationality' AND @SortOrder = 'ASC' THEN u.Nationality END ASC,
        CASE WHEN @SortBy = 'Nationality' AND @SortOrder = 'DESC' THEN u.Nationality END DESC,
        CASE WHEN @SortBy = 'BirthDate' AND @SortOrder = 'ASC' THEN u.BirthDate END ASC,
        CASE WHEN @SortBy = 'BirthDate' AND @SortOrder = 'DESC' THEN u.BirthDate END DESC,
        CASE WHEN @SortBy = 'CreatedAt' AND @SortOrder = 'ASC' THEN u.CreatedAt END ASC,
        CASE WHEN @SortBy = 'CreatedAt' AND @SortOrder = 'DESC' THEN u.CreatedAt END DESC,
        CASE WHEN @SortBy IS NULL THEN u.CreatedAt END DESC -- Default sort
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO
