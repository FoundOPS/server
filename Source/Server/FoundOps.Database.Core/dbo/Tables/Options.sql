CREATE TABLE [dbo].[Options] (
    [Id]             UNIQUEIDENTIFIER NOT NULL,
    [Name]           NVARCHAR (MAX)   NOT NULL,
    [IsChecked]      BIT              NOT NULL,
    [OptionsFieldId] UNIQUEIDENTIFIER NOT NULL,
    [Index]          INT              NOT NULL,
    [Tooltip]        NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_Options] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_OptionsFieldOption] FOREIGN KEY ([OptionsFieldId]) REFERENCES [dbo].[Fields_OptionsField] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);


GO
CREATE NONCLUSTERED INDEX [IX_FK_OptionsFieldOption]
    ON [dbo].[Options]([OptionsFieldId] ASC);

