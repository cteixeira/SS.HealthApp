CREATE TABLE [dbo].[UsageMetric] (
    [UsageMetricID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [Class]         NVARCHAR (200) NOT NULL,
    [Method]        NVARCHAR (200) NOT NULL,
    [Info]          NVARCHAR (MAX) NULL,
    [Moment]        SMALLDATETIME  NOT NULL,
    [CompanyID]     NVARCHAR (50)  NOT NULL,
    [UserID]        NVARCHAR (50)  NOT NULL,
    CONSTRAINT [PK_UsageMetric] PRIMARY KEY CLUSTERED ([UsageMetricID] ASC)
);



