IF OBJECT_ID('dbo.adm_BookingHeader', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[adm_BookingHeader](
        [Id] [bigint] IDENTITY(1,1) NOT NULL,
        [AmenityId] [bigint] NOT NULL,
        [SocietyId] [bigint] NOT NULL,
        [BookingNo] [nvarchar](50) NOT NULL,
        [BookingDate] [date] NOT NULL,
        [Remarks] [nvarchar](500) NULL,
        [Status] [nvarchar](50) NOT NULL,
        [CreatedDate] [datetime2](7) NOT NULL,
        [CreatedBy] [bigint] NULL,
        [ModifiedDate] [datetime2](7) NULL,
        [ModifiedBy] [bigint] NULL,
        CONSTRAINT [PK_adm_BookingHeader] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    ALTER TABLE [dbo].[adm_BookingHeader] WITH CHECK
    ADD CONSTRAINT [FK_adm_BookingHeader_adm_AmenityMaster] FOREIGN KEY([AmenityId])
    REFERENCES [dbo].[adm_AmenityMaster] ([Id]);

    ALTER TABLE [dbo].[adm_BookingHeader] WITH CHECK
    ADD CONSTRAINT [FK_adm_BookingHeader_adm_Property] FOREIGN KEY([SocietyId])
    REFERENCES [dbo].[adm_Property] ([Id]);
END

IF NOT EXISTS (SELECT 1 FROM dbo.adm_Permission WHERE Code = 'PER_BOOKING_HEADER')
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
        ('PER_BOOKING_HEADER', 'Booking Header', 1, 72, 1, NULL, NULL, 1, GETUTCDATE(), 1, GETUTCDATE(), 1),
        ('PER_BOOKING_HEADER', 'Booking Header', 1, 73, 1, NULL, NULL, 1, GETUTCDATE(), 1, GETUTCDATE(), 1),
        ('PER_BOOKING_HEADER', 'Booking Header', 1, 74, 1, NULL, NULL, 1, GETUTCDATE(), 1, GETUTCDATE(), 1),
        ('PER_BOOKING_HEADER', 'Booking Header', 1, 75, 1, NULL, NULL, 1, GETUTCDATE(), 1, GETUTCDATE(), 1),
        ('PER_BOOKING_HEADER', 'Booking Header', 1, 76, 1, NULL, NULL, 1, GETUTCDATE(), 1, GETUTCDATE(), 1);
END
