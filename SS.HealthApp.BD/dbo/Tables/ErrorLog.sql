CREATE TABLE [dbo].[ErrorLog] (
    [ErrorLogID]  BIGINT         IDENTITY (1, 1) NOT NULL,
    [Priority]    SMALLINT       NOT NULL,
    [Source]      NVARCHAR (200) NOT NULL,
    [Description] NVARCHAR (MAX) NOT NULL,
    [Moment]      SMALLDATETIME  NOT NULL,
    [CompanyID]   NVARCHAR (50)  NOT NULL,
    [UserID]      NVARCHAR (50)  NOT NULL,
    CONSTRAINT [PK_ErrorLog] PRIMARY KEY CLUSTERED ([ErrorLogID] ASC)
);

