CREATE PROCEDURE [Portal].[uspGetDeviceInfoForCurrentUser]
	@userId UNIQUEIDENTIFIER
AS
BEGIN
	SELECT 
			Id
		,	Name
		,	WhenEnrolled
		,	WhenLastUsed
	FROM [Identity].[AuthenticatorDevice]
	WHERE
			UserId = @userId
		AND WhenRevoked IS NULL
END
