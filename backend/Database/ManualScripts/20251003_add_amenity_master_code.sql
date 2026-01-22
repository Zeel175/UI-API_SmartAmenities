IF COL_LENGTH('dbo.adm_AmenityMaster', 'Code') IS NULL
BEGIN
    ALTER TABLE dbo.adm_AmenityMaster
    ADD Code NVARCHAR(50) NULL;
END;

UPDATE dbo.adm_AmenityMaster
SET Code = 'AMN' + RIGHT('0000000' + CAST(Id AS NVARCHAR(10)), 7)
WHERE (Code IS NULL OR LTRIM(RTRIM(Code)) = '');
