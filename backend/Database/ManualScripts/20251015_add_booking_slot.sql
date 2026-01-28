IF OBJECT_ID('dbo.adm_BookingSlot', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[adm_BookingSlot](
        [Id] [bigint] IDENTITY(1,1) NOT NULL,
        [BookingId] [bigint] NOT NULL,
        [BookingUnitId] [bigint] NOT NULL,
        [AmenityId] [bigint] NOT NULL,
        [AmenityUnitId] [bigint] NOT NULL,
        [SlotStartDateTime] [datetime2](7) NOT NULL,
        [SlotEndDateTime] [datetime2](7) NOT NULL,
        [SlotStatus] [nvarchar](20) NOT NULL CONSTRAINT [DF_adm_BookingSlot_SlotStatus] DEFAULT('Reserved'),
        [CheckInRequired] [bit] NOT NULL CONSTRAINT [DF_adm_BookingSlot_CheckInRequired] DEFAULT(0),
        [CheckInTime] [datetime2](7) NULL,
        [CheckOutTime] [datetime2](7) NULL,
        [CreatedDate] [datetime2](7) NOT NULL,
        [CreatedBy] [bigint] NULL,
        [ModifiedDate] [datetime2](7) NULL,
        [ModifiedBy] [bigint] NULL,
        CONSTRAINT [PK_adm_BookingSlot] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    ALTER TABLE [dbo].[adm_BookingSlot] WITH CHECK
    ADD CONSTRAINT [FK_adm_BookingSlot_adm_BookingHeader] FOREIGN KEY([BookingId])
    REFERENCES [dbo].[adm_BookingHeader] ([Id]);

    ALTER TABLE [dbo].[adm_BookingSlot] WITH CHECK
    ADD CONSTRAINT [FK_adm_BookingSlot_adm_BookingUnit] FOREIGN KEY([BookingUnitId])
    REFERENCES [dbo].[adm_BookingUnit] ([Id]);

    ALTER TABLE [dbo].[adm_BookingSlot] WITH CHECK
    ADD CONSTRAINT [FK_adm_BookingSlot_adm_AmenityMaster] FOREIGN KEY([AmenityId])
    REFERENCES [dbo].[adm_AmenityMaster] ([Id]);

    ALTER TABLE [dbo].[adm_BookingSlot] WITH CHECK
    ADD CONSTRAINT [FK_adm_BookingSlot_adm_AmenityUnit] FOREIGN KEY([AmenityUnitId])
    REFERENCES [dbo].[adm_AmenityUnit] ([Id]);
END
