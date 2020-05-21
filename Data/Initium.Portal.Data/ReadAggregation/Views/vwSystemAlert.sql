CREATE VIEW [ReadAggregation].[vwSystemAlert]
AS
SELECT 
        Id
    ,   Message
    ,   Type
    ,   WhenToShow
    ,   WhenToHide
FROM Messaging.SystemAlert
