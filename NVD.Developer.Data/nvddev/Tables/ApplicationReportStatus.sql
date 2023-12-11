CREATE TABLE [nvddev].[ApplicationReportStatus]
(
	[Id] INT IDENTITY (1, 1) NOT NULL,
	[StatusName] VARCHAR(150) NOT NULL, 
	CONSTRAINT [PK_ApplicationReportStatus] PRIMARY KEY CLUSTERED ([Id] ASC),
)
GO
CREATE NONCLUSTERED INDEX [IX_ApplicationReportStatus_Id]
    ON [nvddev].[ApplicationReportStatus]([Id] ASC);