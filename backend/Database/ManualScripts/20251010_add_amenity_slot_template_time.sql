IF OBJECT_ID('dbo.adm_AmenitySlotTemplateTime', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[adm_AmenitySlotTemplateTime](
        [Id] [bigint] IDENTITY(1,1) NOT NULL,
        [SlotTemplateId] [bigint] NOT NULL,
        [StartTime] [time](7) NOT NULL,
        [EndTime] [time](7) NOT NULL,
        [CapacityPerSlot] [int] NULL,
        [IsActive] [bit] NOT NULL CONSTRAINT [DF_adm_AmenitySlotTemplateTime_IsActive] DEFAULT(1),
        [CreatedDate] [datetime2](7) NOT NULL,
        [CreatedBy] [bigint] NULL,
        [ModifiedDate] [datetime2](7) NULL,
        [ModifiedBy] [bigint] NULL,
        CONSTRAINT [PK_adm_AmenitySlotTemplateTime] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    ALTER TABLE [dbo].[adm_AmenitySlotTemplateTime] WITH CHECK
    ADD CONSTRAINT [FK_adm_AmenitySlotTemplateTime_adm_AmenitySlotTemplate] FOREIGN KEY([SlotTemplateId])
    REFERENCES [dbo].[adm_AmenitySlotTemplate] ([Id])
    ON DELETE CASCADE;
END

IF OBJECT_ID('dbo.adm_AmenitySlotTemplate', 'U') IS NOT NULL
BEGIN
    INSERT INTO dbo.adm_AmenitySlotTemplateTime
    (
        SlotTemplateId,
        StartTime,
        EndTime,
        CapacityPerSlot,
        IsActive,
        CreatedDate,
        CreatedBy,
        ModifiedDate,
        ModifiedBy
    )
    SELECT
        Id,
        StartTime,
        EndTime,
        CapacityPerSlot,
        ISNULL(IsActive, 1),
        ISNULL(CreatedDate, GETUTCDATE()),
        CreatedBy,
        ISNULL(ModifiedDate, GETUTCDATE()),
        ModifiedBy
    FROM dbo.adm_AmenitySlotTemplate
    WHERE StartTime IS NOT NULL
      AND EndTime IS NOT NULL
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.adm_AmenitySlotTemplateTime t
          WHERE t.SlotTemplateId = dbo.adm_AmenitySlotTemplate.Id
            AND t.StartTime = dbo.adm_AmenitySlotTemplate.StartTime
            AND t.EndTime = dbo.adm_AmenitySlotTemplate.EndTime
      );
END

-- Optional cleanup once the application reads from adm_AmenitySlotTemplateTime:
-- ALTER TABLE dbo.adm_AmenitySlotTemplate DROP COLUMN StartTime, EndTime, CapacityPerSlot;
