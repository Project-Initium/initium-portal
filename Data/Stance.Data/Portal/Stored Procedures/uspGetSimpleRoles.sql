CREATE PROCEDURE [Portal].[uspGetSimpleRoles]
AS
BEGIN
	SELECT 
			Id
		,	Name
	FROM [AccessProtection].[Role]
END
