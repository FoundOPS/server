﻿CREATE TABLE [dbo].[Fields_LocationField] (
    [LocationId]           UNIQUEIDENTIFIER NULL,
    [LocationFieldTypeInt] SMALLINT         NOT NULL,
    [Id]                   UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Fields_LocationField] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_LocationField_inherits_Field] FOREIGN KEY ([Id]) REFERENCES [dbo].[Fields] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_LocationFieldLocation] FOREIGN KEY ([LocationId]) REFERENCES [dbo].[Locations] ([Id]) ON DELETE SET NULL
);






GO
CREATE NONCLUSTERED INDEX [IX_FK_LocationFieldLocation]
    ON [dbo].[Fields_LocationField]([LocationId] ASC);

