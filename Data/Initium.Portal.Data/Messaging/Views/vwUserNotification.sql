CREATE VIEW [Messaging].[vwUserSystemNotification]
AS
SELECT 
		uN.NotificationId
	,	uN.UserId as Id
	,	uN.TenantId
FROM [Messaging].[UserSystemNotification] uN