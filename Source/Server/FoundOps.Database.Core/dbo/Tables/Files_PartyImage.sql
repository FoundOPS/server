﻿CREATE TABLE [dbo].[Files_PartyImage] (
    [Id] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK_Files_PartyImage] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_PartyImage_inherits_File] FOREIGN KEY ([Id]) REFERENCES [dbo].[Files] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PartyPartyImage] FOREIGN KEY ([Id]) REFERENCES [dbo].[Parties] ([Id])
);



