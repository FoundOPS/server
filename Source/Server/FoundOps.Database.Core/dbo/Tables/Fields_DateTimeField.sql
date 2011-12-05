CREATE TABLE [dbo].[Fields_DateTimeField] (
    [Earliest] DATETIME         NOT NULL,
    [Latest]   DATETIME         NOT NULL,
    [TypeInt]  SMALLINT         NOT NULL,
    [Value]    DATETIME         NULL,
    [Id]       UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Fields_DateTimeField] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_DateTimeField_inherits_Field] FOREIGN KEY ([Id]) REFERENCES [dbo].[Fields] ([Id]) ON DELETE NO ACTION ON UPDATE NO ACTION
);

