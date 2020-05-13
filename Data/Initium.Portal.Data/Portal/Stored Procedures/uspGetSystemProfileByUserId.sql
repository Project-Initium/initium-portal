CREATE PROCEDURE [Portal].[uspGetSystemProfileByUserId]
	@userId UNIQUEIDENTIFIER
AS
BEGIN
	SELECT
			u.EmailAddress
		,	p.FirstName
		,	p.LastName
		,	u.IsAdmin
	FROM [Identity].[User] u
	LEFT JOIN [Identity].[Profile] p
		ON u.Id = p.UserId
	WHERE u.Id = @userId;
	
	SELECT r.NormalizedName
	FROM [Identity].[UserRole] uR
	JOIN [AccessProtection].[RoleResource] rR
		ON rR.RoleId = uR.RoleId
	JOIN [AccessProtection].[Resource] r
		ON r.Id = rR.ResourceId
	WHERE uR.UserId = @userId;
END
