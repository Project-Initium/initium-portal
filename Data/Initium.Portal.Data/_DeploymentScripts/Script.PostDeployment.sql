DECLARE @usersParent uniqueidentifier = '5424fc1b-c4aa-46d1-9c58-c22d55df6b4e';
DECLARE @rolesParent uniqueidentifier = '5d9b3d87-febc-42b0-ac9d-d86c7b8b3d95';
DECLARE @systemAlertParent uniqueidentifier = 'babac264-ef5d-4222-9522-ff625a5495f5';
DECLARE @tenantParent uniqueidentifier = '4a5eba70-a762-4067-86fb-88cec3ebc841';

MERGE INTO [AccessProtection].[Resource] AS Target
USING (VALUES
  (@usersParent, N'Users', 'users', null, null),
  ('d4b70a73-6065-451b-85b4-54f5f544c3e5', N'View Users', 'user-view', @usersParent, null),
  ('e457df69-7e2f-437a-84b8-2b5b0678cef7', N'Edit Users', 'user-edit', @usersParent, null),
  ('0407e48b-f454-4af6-9874-7927e3cbeb7f', N'Create Users', 'user-create', @usersParent, null),
  ('a96ae244-136d-4aba-a4d8-02a72f9a6ead', N'Delete Users', 'user-delete', @usersParent, null),
  ('8171D89E-C875-4344-A74F-CDF31EFEEFC2', N'Eanble Users', 'user-enable', @usersParent, null),
  ('904984E6-6CF0-4923-90E5-652380EF7435', N'Disale Users', 'user-disable', @usersParent, null),
  ('09BB06D1-AB81-44FA-A03A-2CB5795E4BA8', N'Unlock Users', 'user-unlock', @usersParent, null),
  (@rolesParent, N'Roles', 'roles', null, null),
  ('523e1710-0861-49a4-9643-861344745ba5', N'View Roles', 'role-view', @rolesParent, null),
  ('f2d4dbb1-48d4-43bb-a1fa-6c3f44328e15', N'Edit Roles', 'role-edit', @rolesParent, null),
  ('dfec9fb4-3a69-4d9a-bf78-ccd142e7209e', N'Create Roles', 'role-create', @rolesParent, null),
  ('e200fdf9-a6e6-4fad-890a-6c5577f42c7c', N'Delete Roles', 'role-delete', @rolesParent, null),
  (@systemAlertParent, N'System Alerts', 'system-alert', null, 'SystemAlerts'),
  ('7ca14f6f-9912-40e0-9db3-82cfb3f01c9d', N'View System Alerts', 'system-alert-view', @systemAlertParent, 'SystemAlerts'),
  ('b905a312-c794-4ea5-a4a2-0aad6bcfa31d', N'Edit System Alerts', 'system-alert-edit', @systemAlertParent, 'SystemAlerts'),
  ('b6ea76a8-aea5-4e5c-9111-598c59944596', N'Create System Alerts', 'system-alert-create', @systemAlertParent, 'SystemAlerts'),
  ('3d3b6b94-6ebf-45ed-adc7-b427dd29323b', N'Delete System Alerts', 'system-alert-delete', @systemAlertParent, 'SystemAlerts'),
  (@tenantParent, N'Tenants', 'tenant', null, 'Tenants'),
  ('27368a6a-da07-4f21-b7a0-8bc3d14f53e7', N'View Tenants', 'tenant-view', @systemAlertParent, 'Tenants'),
  ('685450f3-cd8f-4de1-8b5b-b585f8b3ef15', N'Edit Tenants', 'tenant-edit', @systemAlertParent, 'Tenants'),
  ('c6d2cfe7-03b1-4c43-9b97-51622e7c8ad0', N'Create Tenants', 'tenant-create', @systemAlertParent, 'Tenants'),
  ('d926364e-cc02-42d9-8b46-8046badbb569', N'Delete Tenants', 'tenant-delete', @systemAlertParent, 'Tenants')
 )
AS Source (Id, Name, NormalizedName, ParentResourceId, FeatureCode)
ON Target.Id = Source.Id
-- update matched rows
WHEN MATCHED THEN
UPDATE SET 
        Name = Source.Name
    ,   NormalizedName = Source.NormalizedName
    ,   ParentResourceId = Source.ParentResourceId
    ,   FeatureCode = Source.FeatureCode
--insert new rows
WHEN NOT MATCHED BY TARGET THEN
INSERT (Id, Name, NormalizedName, ParentResourceId, FeatureCode)
VALUES (Id, Name, NormalizedName, ParentResourceId, FeatureCode)
-- delete rows that are in the target but not the source
WHEN NOT MATCHED BY SOURCE THEN
DELETE;

MERGE INTO [Admin].[Tenant] AS Target
USING (VALUES
    ('9836546d-cca4-4f4d-8e2a-3882f00c36f0', 'default', 'default', '')
)
AS SOURCE (Id, Identifier, Name, ConnectionString)
ON Target.Id = Source.Id
WHEN NOT MATCHED BY TARGET THEN
INSERT (Id, Identifier, Name, ConnectionString)
VALUES (Id, Identifier, Name, ConnectionString);