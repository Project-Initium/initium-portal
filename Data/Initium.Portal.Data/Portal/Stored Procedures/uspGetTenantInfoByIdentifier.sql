CREATE PROCEDURE [Portal].[uspGetTenantInfoByIdentifier]
	@identifier NVARCHAR(MAX)
AS
BEGIN
	SELECT 
		t.Id
	,	t.Identifier
	,	t.Name
	,	t.ConnectionString
	FROM [Admin].[Tenant] t
	WHERE t.Identifier = @identifier
END
