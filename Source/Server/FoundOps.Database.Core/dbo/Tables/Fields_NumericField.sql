CREATE TABLE [dbo].[Fields_NumericField] (
    [Mask]          NVARCHAR (MAX)   NOT NULL,
    [DecimalPlaces] INT              NOT NULL,
    [Minimum]       DECIMAL (16, 6)  NOT NULL,
    [Maximum]       DECIMAL (16, 6)  NOT NULL,
    [Value]         DECIMAL (16, 6)  NULL,
    [Id]            UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Fields_NumericField] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_NumericField_inherits_Field] FOREIGN KEY ([Id]) REFERENCES [dbo].[Fields] ([Id]) ON DELETE CASCADE
);



