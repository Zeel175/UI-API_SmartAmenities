IF OBJECT_ID('dbo.adm_AmenityUnitMaster', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[adm_AmenityUnitMaster](
        [Id] [bigint] IDENTITY(1,1) NOT NULL,
        [AmenityId] [bigint] NOT NULL,
        [UnitName] [nvarchar](200) NOT NULL,
        [UnitCode] [nvarchar](50) NULL,
        [Status] [nvarchar](50) NOT NULL,
        [ShortDescription] [nvarchar](200) NULL,
        [LongDescription] [nvarchar](max) NULL,
        [IsChargeable] [bit] NOT NULL CONSTRAINT [DF_adm_AmenityUnitMaster_IsChargeable] DEFAULT(0),
        [ChargeType] [nvarchar](20) NULL,
        [BaseRate] [decimal](18, 2) NULL,
        [SecurityDeposit] [decimal](18, 2) NULL,
        [RefundableDeposit] [bit] NOT NULL CONSTRAINT [DF_adm_AmenityUnitMaster_RefundableDeposit] DEFAULT(0),
        [TaxApplicable] [bit] NOT NULL CONSTRAINT [DF_adm_AmenityUnitMaster_TaxApplicable] DEFAULT(0),
        [TaxCodeId] [bigint] NULL,
        [TaxPercentage] [decimal](5, 2) NULL,
        [CreatedDate] [datetime2](7) NOT NULL,
        [CreatedBy] [bigint] NULL,
        [ModifiedDate] [datetime2](7) NULL,
        [ModifiedBy] [bigint] NULL,
        CONSTRAINT [PK_adm_AmenityUnitMaster] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    CREATE UNIQUE INDEX [IX_adm_AmenityUnitMaster_AmenityId_UnitCode]
        ON [dbo].[adm_AmenityUnitMaster]([AmenityId], [UnitCode]);

    ALTER TABLE [dbo].[adm_AmenityUnitMaster] WITH CHECK
    ADD CONSTRAINT [FK_adm_AmenityUnitMaster_adm_AmenityMaster] FOREIGN KEY([AmenityId])
    REFERENCES [dbo].[adm_AmenityMaster] ([Id]);
END

IF OBJECT_ID('dbo.adm_AmenityUnitFeature', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[adm_AmenityUnitFeature](
        [Id] [bigint] IDENTITY(1,1) NOT NULL,
        [AmenityUnitId] [bigint] NOT NULL,
        [FeatureId] [bigint] NULL,
        [FeatureName] [nvarchar](200) NOT NULL,
        [IsActive] [bit] NOT NULL CONSTRAINT [DF_adm_AmenityUnitFeature_IsActive] DEFAULT(1),
        [CreatedDate] [datetime2](7) NOT NULL,
        [CreatedBy] [bigint] NULL,
        [ModifiedDate] [datetime2](7) NULL,
        [ModifiedBy] [bigint] NULL,
        CONSTRAINT [PK_adm_AmenityUnitFeature] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    ALTER TABLE [dbo].[adm_AmenityUnitFeature] WITH CHECK
    ADD CONSTRAINT [FK_adm_AmenityUnitFeature_adm_AmenityUnitMaster] FOREIGN KEY([AmenityUnitId])
    REFERENCES [dbo].[adm_AmenityUnitMaster] ([Id]);
END

IF NOT EXISTS (SELECT 1 FROM dbo.adm_Permission WHERE Code = 'PER_AMENITY_UNIT')
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
        ('PER_AMENITY_UNIT', 'Amenity Unit', 1, 72, 1, NULL, NULL, 1, GETUTCDATE(), 1, GETUTCDATE(), 1),
        ('PER_AMENITY_UNIT', 'Amenity Unit', 1, 73, 1, NULL, NULL, 1, GETUTCDATE(), 1, GETUTCDATE(), 1),
        ('PER_AMENITY_UNIT', 'Amenity Unit', 1, 74, 1, NULL, NULL, 1, GETUTCDATE(), 1, GETUTCDATE(), 1),
        ('PER_AMENITY_UNIT', 'Amenity Unit', 1, 75, 1, NULL, NULL, 1, GETUTCDATE(), 1, GETUTCDATE(), 1),
        ('PER_AMENITY_UNIT', 'Amenity Unit', 1, 76, 1, NULL, NULL, 1, GETUTCDATE(), 1, GETUTCDATE(), 1);
END
