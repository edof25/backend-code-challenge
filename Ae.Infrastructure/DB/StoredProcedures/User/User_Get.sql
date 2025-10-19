CREATE OR ALTER PROCEDURE dbo.User_Get
    @Id INT = NULL,
    @Username NVARCHAR(255) = NULL,
    @SearchTerm NVARCHAR(255) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

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

    -- Get paged results
    SELECT
        u.Id,
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
    ORDER BY u.CreatedAt DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO