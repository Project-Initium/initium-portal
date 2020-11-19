CREATE VIEW [Admin].[vwTenant]
AS 
WITH TenantLogins
AS ( SELECT ROW_NUMBER() over (
	PARTITION BY d.Id
	ORDER BY d.WhenLoggedIn desc
) as ref, *
FROM (SELECT 
		t.Id
	,	t.Name
	,	t.Identifier
	,	t.WhenDisabled
	,	p.UserId as LastLoggedInUserId
	,	p.FirstName + ' ' + p.LastName as LastLoggedInUser
	,	aH.WhenHappened as WhenLoggedIn
	,	t.SystemFeaturesJson
FROM [Admin].[Tenant] t
LEFT JOIN [Identity].[User] u
	ON t.Id = u.TenantId
LEFT JOIN [Identity].Profile p
	ON u.Id = p.UserId AND p.TenantId = u.TenantId
LEFT JOIN [Identity].[AuthenticationHistory] aH
	ON aH.UserId = u.Id AND aH.AuthenticationHistoryType = 1 and ah.TenantId = u.TenantId
) as d
)
SELECT
		tL.Id
	,	tL.Name
	,	tL.Identifier
	,	tL.WhenDisabled
	,	tL.LastLoggedInUserId	
	,	tl.LastLoggedInUser
	,	tL.WhenLoggedIn
	,	tL.SystemFeaturesJson
FROM TenantLogins tL
WHERE ref = 1