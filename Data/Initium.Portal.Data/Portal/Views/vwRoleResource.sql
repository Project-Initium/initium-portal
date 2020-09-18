CREATE VIEW [Portal].[vwRoleResource]
AS
SELECT rr.RoleId,
       rr.ResourceId,
       rr.TenantId
FROM AccessProtection.RoleResource rr;
