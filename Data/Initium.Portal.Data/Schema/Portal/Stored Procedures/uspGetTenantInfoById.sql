CREATE PROCEDURE [Portal].[uspGetTenantInfoById]
	@id UNIQUEIDENTIFIER
AS
BEGIN
	SELECT 
			t.Id
		,	t.Identifier
		,	t.Name
		,	t.ConnectionString
	FROM [Admin].[Tenant] t
	WHERE t.Id = @id
END
