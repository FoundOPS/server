CREATE TABLE [dbo].[Fields_OptionsField] (
    [AllowMultipleSelection] BIT              NOT NULL,
    [TypeInt]                SMALLINT         NOT NULL,
    [Id]                     UNIQUEIDENTIFIER NOT NULL,
    [OptionsString]          NVARCHAR (MAX)   NULL,
    [Value]                  NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_Fields_OptionsField] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_OptionsField_inherits_Field] FOREIGN KEY ([Id]) REFERENCES [dbo].[Fields] ([Id]) ON DELETE CASCADE
);





