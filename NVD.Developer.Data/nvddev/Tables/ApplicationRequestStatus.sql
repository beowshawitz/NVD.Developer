CREATE TABLE [nvddev].[ApplicationRequestStatus]
(
	[Id] INT IDENTITY (1, 1) NOT NULL,
	[StatusName] VARCHAR(150) NOT NULL, 
	CONSTRAINT [PK_ApplicationRequestStatus] PRIMARY KEY CLUSTERED ([Id] ASC),
)
GO
CREATE NONCLUSTERED INDEX [IX_ApplicationRequestStatus_Id]
    ON [nvddev].[ApplicationRequestStatus]([Id] ASC);