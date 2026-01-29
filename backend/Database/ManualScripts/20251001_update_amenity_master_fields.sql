IF COL_LENGTH('dbo.adm_AmenityMaster', 'MaxCapacity') IS NULL
    ALTER TABLE dbo.adm_AmenityMaster ADD MaxCapacity INT NULL;

IF COL_LENGTH('dbo.adm_AmenityMaster', 'MaxBookingsPerDayPerFlat') IS NULL
    ALTER TABLE dbo.adm_AmenityMaster ADD MaxBookingsPerDayPerFlat INT NULL;

IF COL_LENGTH('dbo.adm_AmenityMaster', 'MaxActiveBookingsPerFlat') IS NULL
    ALTER TABLE dbo.adm_AmenityMaster ADD MaxActiveBookingsPerFlat INT NULL;

IF COL_LENGTH('dbo.adm_AmenityMaster', 'MinAdvanceBookingHours') IS NULL
    ALTER TABLE dbo.adm_AmenityMaster ADD MinAdvanceBookingHours INT NULL;

IF COL_LENGTH('dbo.adm_AmenityMaster', 'MinAdvanceBookingDays') IS NULL
    ALTER TABLE dbo.adm_AmenityMaster ADD MinAdvanceBookingDays INT NULL;

IF COL_LENGTH('dbo.adm_AmenityMaster', 'MaxAdvanceBookingDays') IS NULL
    ALTER TABLE dbo.adm_AmenityMaster ADD MaxAdvanceBookingDays INT NULL;

IF COL_LENGTH('dbo.adm_AmenityMaster', 'BookingSlotRequired') IS NULL
    ALTER TABLE dbo.adm_AmenityMaster ADD BookingSlotRequired BIT NOT NULL CONSTRAINT DF_adm_AmenityMaster_BookingSlotRequired DEFAULT(0);

IF COL_LENGTH('dbo.adm_AmenityMaster', 'SlotDurationMinutes') IS NULL
    ALTER TABLE dbo.adm_AmenityMaster ADD SlotDurationMinutes INT NULL;

IF COL_LENGTH('dbo.adm_AmenityMaster', 'BufferTimeMinutes') IS NULL
    ALTER TABLE dbo.adm_AmenityMaster ADD BufferTimeMinutes INT NULL;

IF COL_LENGTH('dbo.adm_AmenityMaster', 'AllowMultipleSlotsPerBooking') IS NULL
    ALTER TABLE dbo.adm_AmenityMaster ADD AllowMultipleSlotsPerBooking BIT NOT NULL CONSTRAINT DF_adm_AmenityMaster_AllowMultipleSlotsPerBooking DEFAULT(0);

IF COL_LENGTH('dbo.adm_AmenityMaster', 'AllowMultipleUnits') IS NULL
    ALTER TABLE dbo.adm_AmenityMaster ADD AllowMultipleUnits BIT NOT NULL CONSTRAINT DF_adm_AmenityMaster_AllowMultipleUnits DEFAULT(0);

IF COL_LENGTH('dbo.adm_AmenityMaster', 'RequiresApproval') IS NULL
    ALTER TABLE dbo.adm_AmenityMaster ADD RequiresApproval BIT NOT NULL CONSTRAINT DF_adm_AmenityMaster_RequiresApproval DEFAULT(0);

IF COL_LENGTH('dbo.adm_AmenityMaster', 'AllowGuests') IS NULL
    ALTER TABLE dbo.adm_AmenityMaster ADD AllowGuests BIT NOT NULL CONSTRAINT DF_adm_AmenityMaster_AllowGuests DEFAULT(0);

IF COL_LENGTH('dbo.adm_AmenityMaster', 'MaxGuestsAllowed') IS NULL
    ALTER TABLE dbo.adm_AmenityMaster ADD MaxGuestsAllowed INT NULL;

IF COL_LENGTH('dbo.adm_AmenityMaster', 'AvailableDays') IS NULL
    ALTER TABLE dbo.adm_AmenityMaster ADD AvailableDays NVARCHAR(50) NULL;

IF COL_LENGTH('dbo.adm_AmenityMaster', 'OpenTime') IS NULL
    ALTER TABLE dbo.adm_AmenityMaster ADD OpenTime TIME(7) NULL;

IF COL_LENGTH('dbo.adm_AmenityMaster', 'CloseTime') IS NULL
    ALTER TABLE dbo.adm_AmenityMaster ADD CloseTime TIME(7) NULL;

IF COL_LENGTH('dbo.adm_AmenityMaster', 'HolidayBlocked') IS NULL
    ALTER TABLE dbo.adm_AmenityMaster ADD HolidayBlocked BIT NOT NULL CONSTRAINT DF_adm_AmenityMaster_HolidayBlocked DEFAULT(0);

IF COL_LENGTH('dbo.adm_AmenityMaster', 'MaintenanceSchedule') IS NULL
    ALTER TABLE dbo.adm_AmenityMaster ADD MaintenanceSchedule NVARCHAR(500) NULL;

IF COL_LENGTH('dbo.adm_AmenityMaster', 'IsChargeable') IS NULL
    ALTER TABLE dbo.adm_AmenityMaster ADD IsChargeable BIT NOT NULL CONSTRAINT DF_adm_AmenityMaster_IsChargeable DEFAULT(0);

IF COL_LENGTH('dbo.adm_AmenityMaster', 'ChargeType') IS NULL
    ALTER TABLE dbo.adm_AmenityMaster ADD ChargeType NVARCHAR(20) NULL;

IF COL_LENGTH('dbo.adm_AmenityMaster', 'BaseRate') IS NULL
    ALTER TABLE dbo.adm_AmenityMaster ADD BaseRate DECIMAL(18, 2) NULL;

IF COL_LENGTH('dbo.adm_AmenityMaster', 'SecurityDeposit') IS NULL
    ALTER TABLE dbo.adm_AmenityMaster ADD SecurityDeposit DECIMAL(18, 2) NULL;

IF COL_LENGTH('dbo.adm_AmenityMaster', 'RefundableDeposit') IS NULL
    ALTER TABLE dbo.adm_AmenityMaster ADD RefundableDeposit BIT NOT NULL CONSTRAINT DF_adm_AmenityMaster_RefundableDeposit DEFAULT(0);

IF COL_LENGTH('dbo.adm_AmenityMaster', 'TaxApplicable') IS NULL
    ALTER TABLE dbo.adm_AmenityMaster ADD TaxApplicable BIT NOT NULL CONSTRAINT DF_adm_AmenityMaster_TaxApplicable DEFAULT(0);

IF COL_LENGTH('dbo.adm_AmenityMaster', 'TaxCodeId') IS NULL
    ALTER TABLE dbo.adm_AmenityMaster ADD TaxCodeId BIGINT NULL;

IF COL_LENGTH('dbo.adm_AmenityMaster', 'TaxPercentage') IS NULL
    ALTER TABLE dbo.adm_AmenityMaster ADD TaxPercentage DECIMAL(5, 2) NULL;

IF COL_LENGTH('dbo.adm_AmenityMaster', 'TermsAndConditions') IS NULL
    ALTER TABLE dbo.adm_AmenityMaster ADD TermsAndConditions NVARCHAR(MAX) NULL;

IF COL_LENGTH('dbo.adm_AmenityMaster', 'DocumentPaths') IS NOT NULL
    ALTER TABLE dbo.adm_AmenityMaster DROP COLUMN DocumentPaths;

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
