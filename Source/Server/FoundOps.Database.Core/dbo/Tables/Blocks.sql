CREATE TABLE [dbo].[Blocks] (
    [Id]                 UNIQUEIDENTIFIER NOT NULL,
    [Name]               NVARCHAR (MAX)   NOT NULL,
    [NavigateUri]        NVARCHAR (MAX)   NOT NULL,
    [Icon]               VARBINARY (MAX)  NULL,
    [Link]               NVARCHAR (MAX)   NOT NULL,
    [LoginNotRequired]   BIT              NOT NULL,
    [HideFromNavigation] BIT              NOT NULL,
    CONSTRAINT [PK_Blocks] PRIMARY KEY CLUSTERED ([Id] ASC)
);



