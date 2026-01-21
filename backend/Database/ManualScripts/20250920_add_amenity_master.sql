IF OBJECT_ID('dbo.adm_AmenityMaster', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[adm_AmenityMaster](
        [Id] [bigint] IDENTITY(1,1) NOT NULL,
        [Name] [nvarchar](200) NOT NULL,
        [Type] [nvarchar](50) NOT NULL,
        [Description] [nvarchar](500) NULL,
        [BuildingId] [bigint] NOT NULL,
        [FloorId] [bigint] NOT NULL,
        [Location] [nvarchar](500) NULL,
        [Status] [nvarchar](50) NOT NULL,
        [CreatedDate] [datetime2](7) NOT NULL,
        [CreatedBy] [bigint] NULL,
        [ModifiedDate] [datetime2](7) NULL,
        [ModifiedBy] [bigint] NULL,
        CONSTRAINT [PK_adm_AmenityMaster] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    ALTER TABLE [dbo].[adm_AmenityMaster] WITH CHECK
    ADD CONSTRAINT [FK_adm_AmenityMaster_adm_Building] FOREIGN KEY([BuildingId])
    REFERENCES [dbo].[adm_Building] ([Id]);

    ALTER TABLE [dbo].[adm_AmenityMaster] WITH CHECK
    ADD CONSTRAINT [FK_adm_AmenityMaster_adm_Floor] FOREIGN KEY([FloorId])
    REFERENCES [dbo].[adm_Floor] ([Id]);
END

IF NOT EXISTS (SELECT 1 FROM dbo.adm_Permission WHERE Code = 'PER_AMENITY')
BEGIN
    INSERT INTO dbo.adm_Permission (
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
        ('PER_AMENITY', 'Amenity', 1, 72, 1, NULL, NULL, 1, GETUTCDATE(), 1, GETUTCDATE(), 1),
        ('PER_AMENITY', 'Amenity', 1, 73, 1, NULL, NULL, 1, GETUTCDATE(), 1, GETUTCDATE(), 1),
        ('PER_AMENITY', 'Amenity', 1, 74, 1, NULL, NULL, 1, GETUTCDATE(), 1, GETUTCDATE(), 1),
        ('PER_AMENITY', 'Amenity', 1, 75, 1, NULL, NULL, 1, GETUTCDATE(), 1, GETUTCDATE(), 1),
        ('PER_AMENITY', 'Amenity', 1, 76, 1, NULL, NULL, 1, GETUTCDATE(), 1, GETUTCDATE(), 1);
END
