DECLARE @usersParent uniqueidentifier = '5424fc1b-c4aa-46d1-9c58-c22d55df6b4e';
DECLARE @rolesParent uniqueidentifier = '5d9b3d87-febc-42b0-ac9d-d86c7b8b3d95';

MERGE INTO [AccessProtection].[Resource] AS Target
USING (VALUES
  (@usersParent, N'Users', 'users', null),
  ('d4b70a73-6065-451b-85b4-54f5f544c3e5', N'View Users', 'user-view', @usersParent),
  ('e457df69-7e2f-437a-84b8-2b5b0678cef7', N'Edit Users', 'user-edit', @usersParent),
  ('0407e48b-f454-4af6-9874-7927e3cbeb7f', N'Create Users', 'user-create', @usersParent),
  ('a96ae244-136d-4aba-a4d8-02a72f9a6ead', N'Delete Users', 'user-delete', @usersParent),
  (@rolesParent, N'Roles', 'roles', null),
  ('523e1710-0861-49a4-9643-861344745ba5', N'View Roles', 'role-view', @rolesParent),
  ('f2d4dbb1-48d4-43bb-a1fa-6c3f44328e15', N'Edit Roles', 'role-edit', @rolesParent),
  ('dfec9fb4-3a69-4d9a-bf78-ccd142e7209e', N'Create Roles', 'role-create', @rolesParent),
  ('e200fdf9-a6e6-4fad-890a-6c5577f42c7c', N'Delete Roles', 'role-delete', @rolesParent)
 )
AS Source (Id, Name, NormalizedName, ParentResourceId)
ON Target.Id = Source.Id
-- update matched rows
WHEN MATCHED THEN
UPDATE SET 
        Name = Source.Name
    ,   NormalizedName = Source.NormalizedName
    ,   ParentResourceId = Source.ParentResourceId
--insert new rows
WHEN NOT MATCHED BY TARGET THEN
INSERT (Id, Name, NormalizedName, ParentResourceId)
VALUES (Id, Name, NormalizedName, ParentResourceId)
-- delete rows that are in the target but not the source
WHEN NOT MATCHED BY SOURCE THEN
DELETE;