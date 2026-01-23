IF COL_LENGTH('dbo.adm_AmenitySlotTemplate', 'AmenityUnitId') IS NULL
BEGIN
    ALTER TABLE dbo.adm_AmenitySlotTemplate
        ADD AmenityUnitId BIGINT NULL;
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.foreign_keys
    WHERE name = 'FK_adm_AmenitySlotTemplate_adm_AmenityUnitMaster'
)
BEGIN
    ALTER TABLE dbo.adm_AmenitySlotTemplate WITH CHECK
        ADD CONSTRAINT FK_adm_AmenitySlotTemplate_adm_AmenityUnitMaster
        FOREIGN KEY (AmenityUnitId)
        REFERENCES dbo.adm_AmenityUnitMaster (Id);
END;
