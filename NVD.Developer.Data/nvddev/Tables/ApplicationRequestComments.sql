CREATE TABLE [nvddev].[ApplicationRequestComments]
(
	[Id] INT IDENTITY (1, 1) NOT NULL, 
    [RequestId] INT NOT NULL, 
    [UserId] VARCHAR(255) NOT NULL,
    [Comment] VARCHAR(max) NOT NULL, 
    [DateCreated] DATETIME NOT NULL, 
    [DateUpdated] DATETIME NOT NULL, 
    CONSTRAINT [PK_ApplicationRequestComments] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ApplicationRequestComments_ApplicationRequests] FOREIGN KEY ([RequestId]) REFERENCES [nvddev].[ApplicationRequests]([Id])
)
GO
CREATE NONCLUSTERED INDEX [IX_ApplicationRequestComments_Id]
    ON [nvddev].[ApplicationRequestComments]([Id] ASC);