CREATE TABLE [dbo].[Regions] (
    [Id]                UNIQUEIDENTIFIER NOT NULL,
    [Name]              NVARCHAR (MAX)   NOT NULL,
    [BusinessAccountId] UNIQUEIDENTIFIER NULL,
    [Color]             NVARCHAR (MAX)   NULL,
    [Notes]             NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_Regions] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_BusinessAccountRegion] FOREIGN KEY ([BusinessAccountId]) REFERENCES [dbo].[Parties_BusinessAccount] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);


GO
CREATE NONCLUSTERED INDEX [IX_FK_BusinessAccountRegion]
    ON [dbo].[Regions]([BusinessAccountId] ASC);

