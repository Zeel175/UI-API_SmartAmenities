-- Add FaceUrl columns for resident and family member (manual DB update)
-- Adjust table names if your schema differs.

IF COL_LENGTH('dbo.adm_ResidentMaster', 'FaceUrl') IS NULL
BEGIN
    ALTER TABLE dbo.adm_ResidentMaster
        ADD FaceUrl NVARCHAR(500) NULL;
END;

IF COL_LENGTH('dbo.adm_ResidentFamilyMember', 'FaceUrl') IS NULL
BEGIN
    ALTER TABLE dbo.adm_ResidentFamilyMember
        ADD FaceUrl NVARCHAR(500) NULL;
END;
