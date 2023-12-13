CREATE TABLE [nvddev].[ApplicationReportComments]
(
	[Id] INT IDENTITY (1, 1) NOT NULL, 
    [ReportId] INT NOT NULL, 
    [UserId] VARCHAR(255) NOT NULL,
    [Comment] VARCHAR(max) NOT NULL, 
    [DateCreated] DATETIME NOT NULL, 
    [DateUpdated] DATETIME NOT NULL, 
    CONSTRAINT [PK_ApplicationReportComments] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ApplicationReportComments_ApplicationReports] FOREIGN KEY ([ReportId]) REFERENCES [nvddev].[ApplicationReports]([Id])
)
GO
CREATE NONCLUSTERED INDEX [IX_ApplicationReportComments_Id]
    ON [nvddev].[ApplicationReportComments]([Id] ASC);