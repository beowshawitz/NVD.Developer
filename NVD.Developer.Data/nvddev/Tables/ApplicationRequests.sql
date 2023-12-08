CREATE TABLE [nvddev].[ApplicationRequests]
(
	[Id] INT IDENTITY (1, 1) NOT NULL,
    [StatusId] INT NOT NULL,
    [UserId] VARCHAR(255) NOT NULL, 
    [DisplayName] VARCHAR(250) NOT NULL, 
    [ContactNumber] VARCHAR(250) NOT NULL, 
    [ContactEmail] VARCHAR(250) NOT NULL, 
    [ApplicationName] VARCHAR(250) NOT NULL, 
    [ApplicationVersion] VARCHAR(150) NULL, 
    [RequestingReason] VARCHAR(MAX) NOT NULL, 
    [DateCreated] DATETIME NOT NULL, 
    [DateUpdated] DATETIME NOT NULL,     
    CONSTRAINT [PK_ApplicationRequests] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ApplicationRequests_ApplicationRequestStatus] FOREIGN KEY ([StatusId]) REFERENCES [nvddev].[ApplicationRequestStatus]([Id]),
)

GO
CREATE NONCLUSTERED INDEX [IX_ApplicationRequests_Id]
    ON [nvddev].[ApplicationRequests]([Id] ASC);
