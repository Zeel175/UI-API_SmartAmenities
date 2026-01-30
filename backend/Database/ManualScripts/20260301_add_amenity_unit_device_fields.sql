IF COL_LENGTH('dbo.adm_AmenityUnitMaster', 'DeviceId') IS NULL
BEGIN
    ALTER TABLE dbo.adm_AmenityUnitMaster
        ADD DeviceId INT NULL;
END

IF COL_LENGTH('dbo.adm_AmenityUnitMaster', 'DeviceUserName') IS NULL
BEGIN
    ALTER TABLE dbo.adm_AmenityUnitMaster
        ADD DeviceUserName NVARCHAR(100) NULL;
END

IF COL_LENGTH('dbo.adm_AmenityUnitMaster', 'DevicePassword') IS NULL
BEGIN
    ALTER TABLE dbo.adm_AmenityUnitMaster
        ADD DevicePassword NVARCHAR(100) NULL;
END

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_adm_AmenityUnitMaster_hik_Devices'
)
BEGIN
    ALTER TABLE dbo.adm_AmenityUnitMaster WITH CHECK
        ADD CONSTRAINT FK_adm_AmenityUnitMaster_hik_Devices
        FOREIGN KEY (DeviceId)
        REFERENCES dbo.hik_Devices (Id);
END
