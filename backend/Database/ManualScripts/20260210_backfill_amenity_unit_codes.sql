WITH ExistingCodes AS (
    SELECT
        AmenityId,
        COUNT(*) AS ExistingCount
    FROM dbo.adm_AmenityUnitMaster
    WHERE UnitCode IS NOT NULL
        AND LTRIM(RTRIM(UnitCode)) <> ''
    GROUP BY AmenityId
),
UnitsToUpdate AS (
    SELECT
        u.Id,
        u.AmenityId,
        ROW_NUMBER() OVER (PARTITION BY u.AmenityId ORDER BY u.Id) AS RowNumber
    FROM dbo.adm_AmenityUnitMaster u
    WHERE u.UnitCode IS NULL
        OR LTRIM(RTRIM(u.UnitCode)) = ''
)
UPDATE u
SET UnitCode = CONCAT(
    'AMU',
    RIGHT('0000000' + CAST(ISNULL(e.ExistingCount, 0) + t.RowNumber AS varchar(7)), 7)
)
FROM dbo.adm_AmenityUnitMaster u
INNER JOIN UnitsToUpdate t
    ON u.Id = t.Id
LEFT JOIN ExistingCodes e
    ON e.AmenityId = t.AmenityId;
