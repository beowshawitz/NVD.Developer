CREATE TABLE [nvddev].[ApplicationVersions]
(
	[Id] INT IDENTITY (1, 1) NOT NULL, 
    [ApplicationId] INT NOT NULL, 
    [Name] VARCHAR(50) NOT NULL, 
    CONSTRAINT [PK_ApplicationVersions] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ApplicationVersions_Applications] FOREIGN KEY ([ApplicationId]) REFERENCES [nvddev].[Applications]([Id])
)
GO
CREATE NONCLUSTERED INDEX [IX_ApplicationVersions_Id]
    ON [nvddev].[ApplicationVersions]([Id] ASC);