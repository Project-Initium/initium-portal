CREATE VIEW [Portal].[vwUser]
AS
SELECT 
		u.Id
	,	u.EmailAddress
	,	Cast(Case when u.WhenLocked is null then 0 else 1 end as bit) as IsLocked
	,	u.IsAdmin
	,	Cast(Case when u.WhenVerified is null then 0 else 1 end as bit) as IsVerified
	,	u.IsLockable
	,	u.WhenLastAuthenticated
	,	u.WhenCreated
	,	p.FirstName
	,	p.LastName
	,   u.WhenLocked
	,   u.WhenDisabled
FROM [Identity].[User] u
LEFT JOIN [Identity].Profile p
	ON u.Id = p.UserId