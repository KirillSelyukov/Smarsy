﻿CREATE TABLE [dbo].[Ad]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[AdDate] DATETIME2(7),
	[AdText] NVARCHAR(MAX), 
    [CreateDtime] DATETIME2 NULL DEFAULT SYSDATETIME()
)
