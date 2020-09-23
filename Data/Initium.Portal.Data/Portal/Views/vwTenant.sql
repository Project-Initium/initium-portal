CREATE VIEW [Portal].[vwTenant]
AS 
SELECT 
		t.Id
	,	t.Identifier
	,	t.Name
	,	t.ConnectionString
	,	t.WhenDisabled
FROM [Admin].[Tenant] t
