CREATE VIEW [Portal].[vwUserNotification]
AS
SELECT 
		uN.NotificationId
	,	uN.UserId
	,	n.WhenNotified
	,	n.Type
	,	n.SerializedEventData
	,	n.Subject
	,	n.Message
	,	un.WhenViewed	
	,	un.TenantId
FROM [Messaging].[UserNotification] uN
JOIN [Messaging].[Notification] n
	ON uN.NotificationId = n.Id
WHERE uN.WhenDismissed is null