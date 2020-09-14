CREATE VIEW [Portal].[vwRoleResource]
AS
SELECT rr.RoleId,
       rr.ResourceId
FROM AccessProtection.RoleResource rr;
