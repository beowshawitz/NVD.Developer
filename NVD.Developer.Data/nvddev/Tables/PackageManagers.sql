CREATE TABLE [nvddev].[PackageManagers]
(
	[Id] INT IDENTITY (1, 1) NOT NULL,
    [Name] VARCHAR(150) NOT NULL,
	CONSTRAINT [PK_PackageManagers] PRIMARY KEY CLUSTERED ([Id] ASC),
)
GO
CREATE NONCLUSTERED INDEX [IX_PackageManagers_Id]
    ON [nvddev].[PackageManagers]([Id] ASC);
