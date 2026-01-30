IF OBJECT_ID('dbo.adm_AmenityMaster', 'U') IS NOT NULL
BEGIN
    IF OBJECT_ID('FK_adm_AmenityMaster_adm_Building', 'F') IS NOT NULL
        ALTER TABLE dbo.adm_AmenityMaster DROP CONSTRAINT [FK_adm_AmenityMaster_adm_Building];

    IF OBJECT_ID('FK_adm_AmenityMaster_adm_Floor', 'F') IS NOT NULL
        ALTER TABLE dbo.adm_AmenityMaster DROP CONSTRAINT [FK_adm_AmenityMaster_adm_Floor];

    ALTER TABLE dbo.adm_AmenityMaster ALTER COLUMN BuildingId BIGINT NULL;
    ALTER TABLE dbo.adm_AmenityMaster ALTER COLUMN FloorId BIGINT NULL;

    ALTER TABLE dbo.adm_AmenityMaster WITH CHECK
    ADD CONSTRAINT [FK_adm_AmenityMaster_adm_Building] FOREIGN KEY([BuildingId])
    REFERENCES [dbo].[adm_Building] ([Id]);

    ALTER TABLE dbo.adm_AmenityMaster WITH CHECK
    ADD CONSTRAINT [FK_adm_AmenityMaster_adm_Floor] FOREIGN KEY([FloorId])
    REFERENCES [dbo].[adm_Floor] ([Id]);

    IF COL_LENGTH('dbo.adm_AmenityMaster', 'DeviceId') IS NULL
        ALTER TABLE dbo.adm_AmenityMaster ADD DeviceId INT NULL;

    IF COL_LENGTH('dbo.adm_AmenityMaster', 'DeviceUserName') IS NULL
        ALTER TABLE dbo.adm_AmenityMaster ADD DeviceUserName NVARCHAR(100) NULL;

    IF COL_LENGTH('dbo.adm_AmenityMaster', 'DevicePassword') IS NULL
        ALTER TABLE dbo.adm_AmenityMaster ADD DevicePassword NVARCHAR(100) NULL;

    IF OBJECT_ID('FK_adm_AmenityMaster_hik_Devices', 'F') IS NULL
        ALTER TABLE dbo.adm_AmenityMaster WITH CHECK
        ADD CONSTRAINT [FK_adm_AmenityMaster_hik_Devices] FOREIGN KEY([DeviceId])
        REFERENCES [dbo].[hik_Devices] ([Id]);
END
