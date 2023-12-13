CREATE TABLE [nvddev].[ApplicationReports]
(
	[Id] INT IDENTITY (1, 1) NOT NULL,
    [StatusId] INT NOT NULL,
    [UserId] VARCHAR(255) NOT NULL,
    [ApplicationName] VARCHAR(250) NOT NULL, 
    [ApplicationVersion] VARCHAR(150) NULL, 
    [Description] VARCHAR(MAX) NOT NULL, 
    [DateCreated] DATETIME NOT NULL, 
    [DateUpdated] DATETIME NOT NULL, 
    CONSTRAINT [PK_ApplicationReports] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ApplicationReports_ApplicationReportStatus] FOREIGN KEY ([StatusId]) REFERENCES [nvddev].[ApplicationReportStatus]([Id]),
)

GO
CREATE NONCLUSTERED INDEX [IX_ApplicationReports_Id]
    ON [nvddev].[ApplicationReports]([Id] ASC);
