CREATE TABLE [dbo].[Fields_TextBoxField] (
    [IsMultiline] BIT              NOT NULL,
    [Value]       NVARCHAR (MAX)   NULL,
    [Id]          UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Fields_TextBoxField] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_TextBoxField_inherits_Field] FOREIGN KEY ([Id]) REFERENCES [dbo].[Fields] ([Id]) ON DELETE CASCADE
);



