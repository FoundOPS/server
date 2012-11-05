CREATE TABLE [dbo].[Fields_SignatureField] (
    [Value] NVARCHAR (MAX)   NULL,
    [Id]    UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Fields_SignatureField] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SignatureField_inherits_Field] FOREIGN KEY ([Id]) REFERENCES [dbo].[Fields] ([Id]) ON DELETE CASCADE
);

