CREATE PROCEDURE [Portal].[uspGetDetailsOfUserById]
	@userId UNIQUEIDENTIFIER
AS
BEGIN
	SELECT
			u.Id
		,	u.EmailAddress
		,	p.FirstName
		,	p.LastName
		,	u.IsLockable
		,	u.WhenCreated
		,	u.WhenLastAuthenticated
		,	u.WhenLocked
		,	u.IsAdmin
	FROM [Identity].[User] u
	JOIN [identity].[Profile] p
		ON u.Id = p.UserId
	WHERE u.Id = @userId;

	SELECT uR.RoleId
	FROM [Identity].[UserRole] uR
	WHERE uR.UserId = @userId;
END
