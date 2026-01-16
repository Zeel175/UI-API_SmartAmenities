=> Database attached in API Repository
=> Make migration using "add-migration V1"
=> Update Database (change connection string as per your SQL server )
=> once DB done run below query

	-- Set IDENTITY_INSERT ON for the Permissions table
	SET IDENTITY_INSERT dbo.adm_Permission ON;

	-- Insert statements for the permissions data
	INSERT INTO dbo.adm_Permission (Id, Code, Name, IsDefault, PermissionTypeId, DisplayOrder, IsActive, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate)
	VALUES
	(1, 'PER_DASHBOARD', 'Dashboard', 1, 72, 1, 1, 1, '2020-05-30 06:08:01.6166667', 1, '2020-05-30 06:08:01.6166667'),
	(2, 'PER_DASHBOARD', 'Dashboard', 1, 73, 1, 1, 1, '2020-05-30 06:08:01.6166667', 1, '2020-05-30 06:08:01.6166667'),
	(3, 'PER_DASHBOARD', 'Dashboard', 1, 74, 1, 1, 1, '2020-05-30 06:08:01.6166667', 1, '2020-05-30 06:08:01.6166667'),
	(4, 'PER_DASHBOARD', 'Dashboard', 1, 75, 1, 1, 1, '2020-05-30 06:08:01.6166667', 1, '2020-05-30 06:08:01.6166667'),
	(5, 'PER_DASHBOARD', 'Dashboard', 1, 76, 1, 1, 1, '2020-05-30 06:08:01.6166667', 1, '2020-05-30 06:08:01.6166667'),
	(57, 'PER_USER', 'User', 1, 72, 1, 1, 1, '2019-12-10 00:00:00.0000000', 1, '2019-12-10 12:45:53.5700000'),
	(58, 'PER_USER', 'User', 1, 73, 1, 1, 1, '2019-12-10 12:46:50.9633333', 1, '2019-12-10 12:46:50.9633333'),
	(59, 'PER_USER', 'User', 1, 74, 1, 1, 1, '2019-12-10 12:47:31.7633333', 1, '2019-12-10 12:47:31.7633333'),
	(60, 'PER_USER', 'User', 1, 75, 1, 1, 1, '2019-12-10 12:48:05.1600000', 1, '2019-12-10 12:48:05.1600000'),
	(61, 'PER_ROLE', 'Role', 1, 72, 1, 1, 1, '2019-12-10 00:00:00.0000000', 1, '2019-12-10 12:45:53.5700000'),
	(62, 'PER_ROLE', 'Role', 1, 73, 1, 1, 1, '2019-12-10 12:46:50.9633333', 1, '2019-12-10 12:46:50.9633333'),
	(63, 'PER_ROLE', 'Role', 1, 74, 1, 1, 1, '2019-12-10 12:47:31.7633333', 1, '2019-12-10 12:47:31.7633333'),
	(64, 'PER_ROLE', 'Role', 1, 75, 1, 1, 1, '2019-12-10 12:48:05.1600000', 1, '2019-12-10 12:48:05.1600000'),
	(76, 'PER_USER', 'User', 1, 76, 1, 1, 1, '2020-05-30 06:08:01.6166667', 1, '2020-05-30 06:08:01.6166667'),
	(77, 'PER_ROLE', 'Role', 1, 76, 1, 1, 1, '2020-05-30 06:08:01.6166667', 1, '2020-05-30 06:08:01.6166667');

	-- Set IDENTITY_INSERT OFF for the Permissions table
	SET IDENTITY_INSERT dbo.adm_Permission OFF;

	-- Set IDENTITY_INSERT ON for the RolePermissionMap table
	SET IDENTITY_INSERT dbo.adm_RolePermissionMap ON;
	
	-- Insert statements for the RolePermissionMap data
	INSERT INTO dbo.adm_RolePermissionMap (Id, RoleId, PermissionId, HasMasterAccess)
	VALUES
	(94, 2, 3, 0),
	(95, 2, 59, 0),
	(96, 2, 63, 0),
	(106, 3, 3, 0),
	(107, 3, 1, 0),
	(108, 3, 2, 0),
	(109, 3, 59, 0),
	(110, 3, 57, 0),
	(111, 3, 58, 0),
	(112, 3, 63, 0),
	(113, 3, 61, 0),
	(114, 3, 62, 0),
	(240, 1, 3, 0),
	(241, 1, 59, 0),
	(242, 1, 57, 0),
	(243, 1, 58, 0),
	(244, 1, 60, 0),
	(245, 1, 63, 0),
	(246, 1, 61, 0),
	(247, 1, 62, 0),
	(248, 1, 64, 0);

	-- Set IDENTITY_INSERT OFF for the RolePermissionMap table
	SET IDENTITY_INSERT dbo.adm_RolePermissionMap OFF; 

=> for AUditLog
INSERT INTO adm_Permission (
    Code,
    Name,
    IsDefault,
    PermissionTypeId,
    DisplayOrder,
    Controller,
    ActionName,
     IsActive,
    CreatedDate,
    CreatedBy,
    ModifiedDate,
    ModifiedBy
)
VALUES
('PER_AUDITLOG', 'AuditLog', 1, 72, 1, NULL, NULL,1, GETUTCDATE(), 1, GETUTCDATE(), 1),
('PER_AUDITLOG', 'AuditLog', 1, 73, 1, NULL, NULL,1, GETUTCDATE(), 1, GETUTCDATE(), 1),
('PER_AUDITLOG', 'AuditLog', 1, 74, 1, NULL, NULL,1, GETUTCDATE(), 1, GETUTCDATE(), 1),
('PER_AUDITLOG', 'AuditLog', 1, 75, 1, NULL, NULL,1, GETUTCDATE(), 1, GETUTCDATE(), 1),
('PER_AUDITLOG', 'AuditLog', 1, 76, 1, NULL, NULL,1, GETUTCDATE(), 1, GETUTCDATE(), 1);

=> Now You can log in with 
	Ussername : admin@nutem.com
	Password : Admin@123

***Note***
if API URL change then you have to change in AUth.serve GetPermissionIdsByRoleIdAsync, GetAllPermissionsAsync 
both function
