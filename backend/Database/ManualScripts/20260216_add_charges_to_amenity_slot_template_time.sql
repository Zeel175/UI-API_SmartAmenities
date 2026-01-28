IF COL_LENGTH('dbo.adm_AmenitySlotTemplateTime', 'SlotCharge') IS NULL
BEGIN
    ALTER TABLE dbo.adm_AmenitySlotTemplateTime
        ADD SlotCharge decimal(18,2) NULL;
END

IF COL_LENGTH('dbo.adm_AmenitySlotTemplateTime', 'IsChargeable') IS NULL
BEGIN
    ALTER TABLE dbo.adm_AmenitySlotTemplateTime
        ADD IsChargeable bit NOT NULL CONSTRAINT DF_adm_AmenitySlotTemplateTime_IsChargeable DEFAULT(0);
END

IF COL_LENGTH('dbo.adm_AmenitySlotTemplateTime', 'ChargeType') IS NULL
BEGIN
    ALTER TABLE dbo.adm_AmenitySlotTemplateTime
        ADD ChargeType nvarchar(20) NULL;
END

IF COL_LENGTH('dbo.adm_AmenitySlotTemplateTime', 'BaseRate') IS NULL
BEGIN
    ALTER TABLE dbo.adm_AmenitySlotTemplateTime
        ADD BaseRate decimal(18,2) NULL;
END

IF COL_LENGTH('dbo.adm_AmenitySlotTemplateTime', 'SecurityDeposit') IS NULL
BEGIN
    ALTER TABLE dbo.adm_AmenitySlotTemplateTime
        ADD SecurityDeposit decimal(18,2) NULL;
END

IF COL_LENGTH('dbo.adm_AmenitySlotTemplateTime', 'RefundableDeposit') IS NULL
BEGIN
    ALTER TABLE dbo.adm_AmenitySlotTemplateTime
        ADD RefundableDeposit bit NOT NULL CONSTRAINT DF_adm_AmenitySlotTemplateTime_RefundableDeposit DEFAULT(0);
END

IF COL_LENGTH('dbo.adm_AmenitySlotTemplateTime', 'TaxApplicable') IS NULL
BEGIN
    ALTER TABLE dbo.adm_AmenitySlotTemplateTime
        ADD TaxApplicable bit NOT NULL CONSTRAINT DF_adm_AmenitySlotTemplateTime_TaxApplicable DEFAULT(0);
END

IF COL_LENGTH('dbo.adm_AmenitySlotTemplateTime', 'TaxCodeId') IS NULL
BEGIN
    ALTER TABLE dbo.adm_AmenitySlotTemplateTime
        ADD TaxCodeId bigint NULL;
END

IF COL_LENGTH('dbo.adm_AmenitySlotTemplateTime', 'TaxPercentage') IS NULL
BEGIN
    ALTER TABLE dbo.adm_AmenitySlotTemplateTime
        ADD TaxPercentage decimal(5,2) NULL;
END
