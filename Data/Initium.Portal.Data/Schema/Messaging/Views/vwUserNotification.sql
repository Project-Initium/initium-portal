CREATE VIEW [Messaging].[vwUserNotification]
AS
SELECT 
		uN.NotificationId
	,	uN.UserId as Id
	,	uN.TenantId
FROM [Messaging].[UserNotification] uN