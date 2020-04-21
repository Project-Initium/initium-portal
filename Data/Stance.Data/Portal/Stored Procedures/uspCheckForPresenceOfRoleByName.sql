CREATE PROCEDURE [Portal].[uspCheckForPresenceOfRoleByName]
	@name NVARCHAR(100)
AS
BEGIN
	SELECT TOP 1 Name
	FROM [AccessProtection].[Role]
	WHERE Name = @name
END
	
