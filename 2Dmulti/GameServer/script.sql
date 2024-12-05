IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Account] (
    [AccountDbId] int NOT NULL IDENTITY,
    [AccountName] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_Account] PRIMARY KEY ([AccountDbId])
);
GO

CREATE TABLE [Player] (
    [PlayerDbId] int NOT NULL IDENTITY,
    [PlayerName] nvarchar(450) NOT NULL,
    [AccountDbId] int NOT NULL,
    [Level] int NOT NULL,
    [Hp] int NOT NULL,
    [MaxHp] int NOT NULL,
    [Attack] int NOT NULL,
    [Speed] real NOT NULL,
    [TotalExp] int NOT NULL,
    CONSTRAINT [PK_Player] PRIMARY KEY ([PlayerDbId]),
    CONSTRAINT [FK_Player_Account_AccountDbId] FOREIGN KEY ([AccountDbId]) REFERENCES [Account] ([AccountDbId]) ON DELETE CASCADE
);
GO

CREATE TABLE [Item] (
    [ItemDbId] int NOT NULL IDENTITY,
    [TemplateId] int NOT NULL,
    [Count] int NOT NULL,
    [Slot] int NOT NULL,
    [Equipped] bit NOT NULL,
    [OwnerDbId] int NULL,
    CONSTRAINT [PK_Item] PRIMARY KEY ([ItemDbId]),
    CONSTRAINT [FK_Item_Player_OwnerDbId] FOREIGN KEY ([OwnerDbId]) REFERENCES [Player] ([PlayerDbId])
);
GO

CREATE UNIQUE INDEX [IX_Account_AccountName] ON [Account] ([AccountName]);
GO

CREATE INDEX [IX_Item_OwnerDbId] ON [Item] ([OwnerDbId]);
GO

CREATE INDEX [IX_Player_AccountDbId] ON [Player] ([AccountDbId]);
GO

CREATE UNIQUE INDEX [IX_Player_PlayerName] ON [Player] ([PlayerName]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240618081246_Initial', N'8.0.6');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Account] ADD [JwtToken] nvarchar(max) NOT NULL DEFAULT N'';
GO

CREATE TABLE [ServerInfo] (
    [ServerDbId] int NOT NULL IDENTITY,
    [Name] nvarchar(450) NOT NULL,
    [IpAddress] nvarchar(max) NOT NULL,
    [Port] int NOT NULL,
    [BusyScore] int NOT NULL,
    CONSTRAINT [PK_ServerInfo] PRIMARY KEY ([ServerDbId])
);
GO

CREATE UNIQUE INDEX [IX_ServerInfo_Name] ON [ServerInfo] ([Name]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240620150016_AddServer', N'8.0.6');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Player] ADD [CurMap] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [Player] ADD [PosX] real NOT NULL DEFAULT CAST(0 AS real);
GO

ALTER TABLE [Player] ADD [PosY] real NOT NULL DEFAULT CAST(0 AS real);
GO

ALTER TABLE [Player] ADD [PosZ] real NOT NULL DEFAULT CAST(0 AS real);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240709164739_AddPosInfo', N'8.0.6');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [MapInfo] (
    [MapDbId] int NOT NULL IDENTITY,
    [MaxX] int NOT NULL,
    [MaxY] int NOT NULL,
    [MinX] int NOT NULL,
    [MinY] int NOT NULL,
    [TileInfo] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_MapInfo] PRIMARY KEY ([MapDbId])
);
GO

CREATE UNIQUE INDEX [IX_MapInfo_MapDbId] ON [MapInfo] ([MapDbId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240811152332_AddMapDb', N'8.0.6');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240814140220_map', N'8.0.6');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240817154028_first', N'8.0.6');
GO

COMMIT;
GO

