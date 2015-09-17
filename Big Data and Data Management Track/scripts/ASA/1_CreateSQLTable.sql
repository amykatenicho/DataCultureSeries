CREATE TABLE [dbo].[readingsByMinute] 
(
    [ID] INT IDENTITY(1,1),
    [DeviceType] VarChar(50),
    [DateOfReading] VarChar(50),
    [RoomNumber] VarChar(5),
    [Reading] FLOAT
)
GO

CREATE CLUSTERED INDEX [readingsByMinute]
    ON [dbo].[readingsByMinute]([ID] ASC);


CREATE TABLE [dbo].[AvgReadings] (
    [WinStartTime]   DATETIME2 (6) NULL,
    [WinEndTime]     DATETIME2 (6) NULL,
    [Type]     VarChar(50) NULL,
    [RoomNumber]     VarChar(10) NULL,
    [Avgreading] FLOAT (53)    NULL,
    [EventCount] BIGINT null
);

GO
CREATE CLUSTERED INDEX [AvgReadings]
    ON [dbo].[AvgReadings]([RoomNumber] ASC);
