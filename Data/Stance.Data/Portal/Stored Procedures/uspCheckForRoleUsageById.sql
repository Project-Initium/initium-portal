CREATE PROCEDURE [Portal].[uspCheckForRoleUsageById]
	@roleId UNIQUEIDENTIFIER
AS
BEGIN
  SELECT RoleId
  FROM AccessProtection.RoleResource rR
  WHERE rR.RoleId = @roleId
END
	
