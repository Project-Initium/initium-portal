CREATE VIEW [Messaging].[vwUserNotification]
AS
SELECT 
		uN.NotificationId
	,	uN.UserId as Id
FROM [Messaging].[UserNotification] uN