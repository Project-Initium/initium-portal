CREATE VIEW [ReadAggregation].[vwSystemAlert]
AS
SELECT
        sA.Id
    ,   sA.Name   
    ,   sA.Message
    ,   sA.Type
    ,   sA.WhenToShow
    ,   sA.WhenToHide
    ,   Cast(sA.IsActive as bit) as IsActive
FROM (
SELECT 
        Id
    ,   Name   
    ,   Message
    ,   Type
    ,   WhenToShow
    ,   WhenToHide
    ,   CASE
            WHEN WhenToShow is null AND WhenToHide is null THEN 1
            WHEN WhenToShow < GETUTCDATE() and WhenToHide is null THEN 1
            WHEN WhenToShow is null and WhenToHide > GETUTCDATE() THEN 1
            WHEN WhenToShow > GETUTCDATE() and WhenToHide < GETUTCDATE() THEN 1
            ELSE 0
        END AS IsActive
FROM Messaging.SystemAlert)  sA
