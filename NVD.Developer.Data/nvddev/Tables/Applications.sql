CREATE TABLE [nvddev].[Applications]
(
	[Id] INT IDENTITY (1, 1) NOT NULL,
    [Name] VARCHAR(150) NOT NULL, 
    [DisplayName] VARCHAR(250) NOT NULL, 
    [Description] VARCHAR(MAX) NULL, 
    [LicenseRequired] BIT NOT NULL,
    [ImageData] VARBINARY(MAX) null,
    [DateCreated] DATETIME NOT NULL, 
    [DateUpdated] DATETIME NOT NULL, 
    
    CONSTRAINT [PK_Applications] PRIMARY KEY CLUSTERED ([Id] ASC),
)

GO
CREATE NONCLUSTERED INDEX [IX_Applications_Id]
    ON [nvddev].[Applications]([Id] ASC);
