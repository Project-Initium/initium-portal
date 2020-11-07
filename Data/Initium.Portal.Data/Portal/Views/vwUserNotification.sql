CREATE VIEW [Portal].[vwUserSystemNotification]
AS
SELECT 
		uN.NotificationId
	,	uN.UserId
	,	n.WhenNotified
	,	n.SystemNotificationTypeId
	,	n.SerializedEventData
	,	n.Subject
	,	n.Message
	,	un.WhenViewed	
	,	un.TenantId
FROM [Messaging].[UserSystemNotification] uN
JOIN [Messaging].[SystemNotification] n
	ON uN.NotificationId = n.Id
WHERE uN.WhenDismissed is null