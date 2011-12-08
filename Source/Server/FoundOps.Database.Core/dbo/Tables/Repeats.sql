CREATE TABLE [dbo].[Repeats] (
    [Id]                 UNIQUEIDENTIFIER NOT NULL,
    [EndDate]            DATETIME         NULL,
    [EndAfterTimes]      INT              NULL,
    [RepeatEveryTimes]   INT              NOT NULL,
    [FrequencyInt]       INT              NOT NULL,
    [FrequencyDetailInt] INT              NULL,
    [StartDate]          DATETIME         NOT NULL,
    CONSTRAINT [PK_Repeats] PRIMARY KEY CLUSTERED ([Id] ASC)
);

