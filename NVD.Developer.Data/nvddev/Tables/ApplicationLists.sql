CREATE TABLE [nvddev].[ApplicationLists]
(
	[Id] INT IDENTITY (1, 1) NOT NULL,
    [UserId] VARCHAR(255) NOT NULL, 
    [ApplicationId] INT NOT NULL, 
    [VersionId] INT NULL, 
    CONSTRAINT [PK_ApplicationLists] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ApplicationLists_Applications] FOREIGN KEY ([ApplicationId]) REFERENCES [nvddev].[Applications]([Id]),
    CONSTRAINT [FK_ApplicationLists_ApplicationVersions] FOREIGN KEY ([VersionId]) REFERENCES [nvddev].[ApplicationVersions]([Id])
)
GO
CREATE NONCLUSTERED INDEX [IX_ApplicationLists_Id]
    ON [nvddev].[ApplicationLists]([Id] ASC);