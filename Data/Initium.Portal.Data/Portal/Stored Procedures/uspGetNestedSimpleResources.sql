CREATE PROCEDURE [Portal].[uspGetNestedSimpleResources]
AS
BEGIN
	SELECT 
			Id
		,	Name
		,	ParentResourceId
	FROM [AccessProtection].[Resource]
END
