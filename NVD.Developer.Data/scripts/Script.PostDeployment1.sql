/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

SET IDENTITY_INSERT [nvddev].[ApplicationRequestStatus] ON
MERGE INTO [nvddev].[ApplicationRequestStatus] AS Target 
USING (Values
	(1, 'Submitted'),
	(2, 'Acknowledged'),
	(3, 'Processing'),
	(4, 'Accepted'),
	(5, 'Denied')
) AS Source([Id],[StatusName]) ON (Target.[Id] = Source.[Id])
	WHEN MATCHED THEN 
		UPDATE SET [StatusName] = Source.[StatusName]
	WHEN NOT MATCHED BY TARGET THEN
		INSERT([Id],[StatusName]) VALUES(Source.[Id], Source.[StatusName]);
SET IDENTITY_INSERT [nvddev].[ApplicationRequestStatus] OFF

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'db_nvddev_reader' AND type = 'R')
BEGIN
    CREATE ROLE db_nvddev_reader;
	GRANT SELECT ON SCHEMA::nvddev TO db_nvddev_reader;
END;

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'db_nvddev_contributor' AND type = 'R')
BEGIN
    CREATE ROLE db_nvddev_contributor;
	GRANT SELECT, INSERT, UPDATE, DELETE, EXECUTE ON SCHEMA::nvddev TO db_nvddev_contributor;
END;