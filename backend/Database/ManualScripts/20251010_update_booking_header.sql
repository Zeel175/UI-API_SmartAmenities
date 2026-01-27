IF COL_LENGTH('dbo.adm_BookingHeader', 'ResidentUserId') IS NULL
    ALTER TABLE dbo.adm_BookingHeader ADD ResidentUserId BIGINT NULL;

IF COL_LENGTH('dbo.adm_BookingHeader', 'FlatId') IS NULL
    ALTER TABLE dbo.adm_BookingHeader ADD FlatId BIGINT NULL;

IF COL_LENGTH('dbo.adm_BookingHeader', 'ResidentNameSnapshot') IS NULL
    ALTER TABLE dbo.adm_BookingHeader ADD ResidentNameSnapshot NVARCHAR(200) NULL;

IF COL_LENGTH('dbo.adm_BookingHeader', 'ContactNumberSnapshot') IS NULL
    ALTER TABLE dbo.adm_BookingHeader ADD ContactNumberSnapshot NVARCHAR(30) NULL;

IF COL_LENGTH('dbo.adm_BookingHeader', 'IsChargeableSnapshot') IS NULL
    ALTER TABLE dbo.adm_BookingHeader ADD IsChargeableSnapshot BIT NULL;

IF COL_LENGTH('dbo.adm_BookingHeader', 'AmountBeforeTax') IS NULL
    ALTER TABLE dbo.adm_BookingHeader ADD AmountBeforeTax DECIMAL(18,2) NULL;

IF COL_LENGTH('dbo.adm_BookingHeader', 'TaxAmount') IS NULL
    ALTER TABLE dbo.adm_BookingHeader ADD TaxAmount DECIMAL(18,2) NULL;

IF COL_LENGTH('dbo.adm_BookingHeader', 'DepositAmount') IS NULL
    ALTER TABLE dbo.adm_BookingHeader ADD DepositAmount DECIMAL(18,2) NULL;

IF COL_LENGTH('dbo.adm_BookingHeader', 'DiscountAmount') IS NULL
    ALTER TABLE dbo.adm_BookingHeader ADD DiscountAmount DECIMAL(18,2) NULL;

IF COL_LENGTH('dbo.adm_BookingHeader', 'ConvenienceFee') IS NULL
    ALTER TABLE dbo.adm_BookingHeader ADD ConvenienceFee DECIMAL(18,2) NULL;

IF COL_LENGTH('dbo.adm_BookingHeader', 'TotalPayable') IS NULL
    ALTER TABLE dbo.adm_BookingHeader ADD TotalPayable DECIMAL(18,2) NULL;

IF COL_LENGTH('dbo.adm_BookingHeader', 'RequiresApprovalSnapshot') IS NULL
    ALTER TABLE dbo.adm_BookingHeader ADD RequiresApprovalSnapshot BIT NULL;

IF COL_LENGTH('dbo.adm_BookingHeader', 'ApprovedBy') IS NULL
    ALTER TABLE dbo.adm_BookingHeader ADD ApprovedBy BIGINT NULL;

IF COL_LENGTH('dbo.adm_BookingHeader', 'ApprovedOn') IS NULL
    ALTER TABLE dbo.adm_BookingHeader ADD ApprovedOn DATETIME2(7) NULL;

IF COL_LENGTH('dbo.adm_BookingHeader', 'RejectionReason') IS NULL
    ALTER TABLE dbo.adm_BookingHeader ADD RejectionReason NVARCHAR(500) NULL;

IF COL_LENGTH('dbo.adm_BookingHeader', 'CancelledBy') IS NULL
    ALTER TABLE dbo.adm_BookingHeader ADD CancelledBy BIGINT NULL;

IF COL_LENGTH('dbo.adm_BookingHeader', 'CancelledOn') IS NULL
    ALTER TABLE dbo.adm_BookingHeader ADD CancelledOn DATETIME2(7) NULL;

IF COL_LENGTH('dbo.adm_BookingHeader', 'CancellationReason') IS NULL
    ALTER TABLE dbo.adm_BookingHeader ADD CancellationReason NVARCHAR(500) NULL;

IF COL_LENGTH('dbo.adm_BookingHeader', 'RefundStatus') IS NULL
    ALTER TABLE dbo.adm_BookingHeader ADD RefundStatus NVARCHAR(50) NULL;
