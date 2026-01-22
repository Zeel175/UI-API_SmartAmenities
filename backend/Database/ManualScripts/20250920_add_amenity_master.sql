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
        [MaxCapacity] [int] NULL,
        [MaxBookingsPerDayPerFlat] [int] NULL,
        [MaxActiveBookingsPerFlat] [int] NULL,
        [MinAdvanceBookingHours] [int] NULL,
        [MinAdvanceBookingDays] [int] NULL,
        [MaxAdvanceBookingDays] [int] NULL,
        [BookingSlotRequired] [bit] NOT NULL CONSTRAINT [DF_adm_AmenityMaster_BookingSlotRequired] DEFAULT(0),
        [SlotDurationMinutes] [int] NULL,
        [BufferTimeMinutes] [int] NULL,
        [AllowMultipleSlotsPerBooking] [bit] NOT NULL CONSTRAINT [DF_adm_AmenityMaster_AllowMultipleSlotsPerBooking] DEFAULT(0),
        [RequiresApproval] [bit] NOT NULL CONSTRAINT [DF_adm_AmenityMaster_RequiresApproval] DEFAULT(0),
        [AllowGuests] [bit] NOT NULL CONSTRAINT [DF_adm_AmenityMaster_AllowGuests] DEFAULT(0),
        [MaxGuestsAllowed] [int] NULL,
        [AvailableDays] [nvarchar](50) NULL,
        [OpenTime] [time](7) NULL,
        [CloseTime] [time](7) NULL,
        [HolidayBlocked] [bit] NOT NULL CONSTRAINT [DF_adm_AmenityMaster_HolidayBlocked] DEFAULT(0),
        [MaintenanceSchedule] [nvarchar](500) NULL,
        [IsChargeable] [bit] NOT NULL CONSTRAINT [DF_adm_AmenityMaster_IsChargeable] DEFAULT(0),
        [ChargeType] [nvarchar](20) NULL,
        [BaseRate] [decimal](18, 2) NULL,
        [SecurityDeposit] [decimal](18, 2) NULL,
        [RefundableDeposit] [bit] NOT NULL CONSTRAINT [DF_adm_AmenityMaster_RefundableDeposit] DEFAULT(0),
        [TaxApplicable] [bit] NOT NULL CONSTRAINT [DF_adm_AmenityMaster_TaxApplicable] DEFAULT(0),
        [TaxCodeId] [bigint] NULL,
        [TaxPercentage] [decimal](5, 2) NULL,
        [TermsAndConditions] [nvarchar](max) NULL,
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

IF OBJECT_ID('dbo.adm_AmenityDocument', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[adm_AmenityDocument](
        [Id] [bigint] IDENTITY(1,1) NOT NULL,
        [AmenityMasterId] [bigint] NULL,
        [FileName] [nvarchar](255) NOT NULL,
        [FilePath] [nvarchar](500) NOT NULL,
        [ContentType] [nvarchar](100) NULL,
        [IsActive] [bit] NOT NULL CONSTRAINT [DF_adm_AmenityDocument_IsActive] DEFAULT(1),
        [CreatedDate] [datetime2](7) NOT NULL,
        [CreatedBy] [bigint] NULL,
        [ModifiedDate] [datetime2](7) NULL,
        [ModifiedBy] [bigint] NULL,
        CONSTRAINT [PK_adm_AmenityDocument] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    ALTER TABLE [dbo].[adm_AmenityDocument] WITH CHECK
    ADD CONSTRAINT [FK_adm_AmenityDocument_adm_AmenityMaster] FOREIGN KEY([AmenityMasterId])
    REFERENCES [dbo].[adm_AmenityMaster] ([Id]);
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
