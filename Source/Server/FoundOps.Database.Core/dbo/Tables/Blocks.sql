CREATE TABLE [dbo].[Blocks] (
    [Id]                 UNIQUEIDENTIFIER NOT NULL,
    [Name]               NVARCHAR (MAX)   NOT NULL,
    [Color]              NVARCHAR (MAX)   NOT NULL,
    [HideFromNavigation] BIT              NOT NULL,
    [IconUrl]            NVARCHAR (MAX)   NULL,
    [HoverIconUrl]       NVARCHAR (MAX)   NULL,
    [Url]                NVARCHAR (MAX)   NULL,
    [IsSilverlight]      BIT              NULL,
    CONSTRAINT [PK_Blocks] PRIMARY KEY CLUSTERED ([Id] ASC)
);





