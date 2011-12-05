CREATE TABLE [dbo].[Options_LocationOption] (
    [Id] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Options_LocationOption] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_LocationOption_inherits_Option] FOREIGN KEY ([Id]) REFERENCES [dbo].[Options] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);

