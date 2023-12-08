CREATE TABLE [nvddev].[AppsToPackageManagers]
(
	[Id] INT IDENTITY (1, 1) NOT NULL,
    [ApplicationId] INT NOT NULL, 
    [PackageManagerId] INT NOT NULL, 
    CONSTRAINT [PK_AppsToPackageManagers] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AppsToPackageManagers_Applications] FOREIGN KEY ([ApplicationId]) REFERENCES [nvddev].[Applications]([Id]),
    CONSTRAINT [FK_AppsToPackageManagers_PackageManagers] FOREIGN KEY ([PackageManagerId]) REFERENCES [nvddev].[PackageManagers]([Id])
)

GO
CREATE NONCLUSTERED INDEX [IX_AppsToPackageManagers_Id]
    ON [nvddev].[AppsToPackageManagers]([Id] ASC);
