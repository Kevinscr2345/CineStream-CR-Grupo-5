/* CineStream CR - SQL Server 2019/2022
   Script completo: crea la base, tablas, restricciones, índices y datos de demostración.
   Ejecute este archivo en SQL Server Management Studio con una cuenta que pueda crear bases. */
SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

USE [master];
GO
IF DB_ID(N'CineStreamCR') IS NULL
BEGIN
    CREATE DATABASE [CineStreamCR];
END
GO
USE [CineStreamCR];
GO

-- Reinicio controlado del esquema de demostración.

IF OBJECT_ID(N'[dbo].[PlaybackProgresses]', N'U') IS NOT NULL DROP TABLE [dbo].[PlaybackProgresses];
IF OBJECT_ID(N'[dbo].[Reviews]', N'U') IS NOT NULL DROP TABLE [dbo].[Reviews];
IF OBJECT_ID(N'[dbo].[WatchListMovies]', N'U') IS NOT NULL DROP TABLE [dbo].[WatchListMovies];
IF OBJECT_ID(N'[dbo].[WatchLists]', N'U') IS NOT NULL DROP TABLE [dbo].[WatchLists];
IF OBJECT_ID(N'[dbo].[MovieCredits]', N'U') IS NOT NULL DROP TABLE [dbo].[MovieCredits];
IF OBJECT_ID(N'[dbo].[MovieGenres]', N'U') IS NOT NULL DROP TABLE [dbo].[MovieGenres];
IF OBJECT_ID(N'[dbo].[People]', N'U') IS NOT NULL DROP TABLE [dbo].[People];
IF OBJECT_ID(N'[dbo].[Genres]', N'U') IS NOT NULL DROP TABLE [dbo].[Genres];
IF OBJECT_ID(N'[dbo].[Movies]', N'U') IS NOT NULL DROP TABLE [dbo].[Movies];
IF OBJECT_ID(N'[dbo].[Users]', N'U') IS NOT NULL DROP TABLE [dbo].[Users];
GO

CREATE TABLE [dbo].[Users] (
    [Id] uniqueidentifier NOT NULL,
    [Email] nvarchar(160) NOT NULL,
    [UserName] nvarchar(80) NOT NULL,
    [DisplayName] nvarchar(120) NOT NULL,
    [PasswordHash] nvarchar(500) NOT NULL,
    [CreatedAt] datetime2(7) NOT NULL CONSTRAINT [DF_Users_CreatedAt] DEFAULT SYSUTCDATETIME(),
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);

CREATE TABLE [dbo].[Genres] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(80) NOT NULL,
    CONSTRAINT [PK_Genres] PRIMARY KEY ([Id])
);

CREATE TABLE [dbo].[Movies] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Title] nvarchar(180) NOT NULL,
    [Synopsis] nvarchar(2000) NOT NULL,
    [ReleaseYear] int NOT NULL,
    [DurationMinutes] int NOT NULL,
    [PosterUrl] nvarchar(400) NOT NULL,
    [BackdropUrl] nvarchar(400) NOT NULL,
    [VideoUrl] nvarchar(400) NOT NULL,
    [InformationSourceUrl] nvarchar(600) NOT NULL CONSTRAINT [DF_Movies_InformationSourceUrl] DEFAULT N'',
    [ImageSourceUrl] nvarchar(600) NOT NULL CONSTRAINT [DF_Movies_ImageSourceUrl] DEFAULT N'',
    [IsFeatured] bit NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2(7) NOT NULL CONSTRAINT [DF_Movies_CreatedAt] DEFAULT SYSUTCDATETIME(),
    CONSTRAINT [PK_Movies] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_Movies_ReleaseYear] CHECK ([ReleaseYear] BETWEEN 1888 AND 2100),
    CONSTRAINT [CK_Movies_DurationMinutes] CHECK ([DurationMinutes] > 0)
);

CREATE TABLE [dbo].[People] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FullName] nvarchar(160) NOT NULL,
    [Nationality] nvarchar(100) NOT NULL,
    [Biography] nvarchar(2500) NOT NULL,
    [BirthDate] date NOT NULL,
    [PhotoUrl] nvarchar(600) NOT NULL,
    [InformationSourceUrl] nvarchar(600) NOT NULL CONSTRAINT [DF_People_InformationSourceUrl] DEFAULT N'',
    [ImageSourceUrl] nvarchar(600) NOT NULL CONSTRAINT [DF_People_ImageSourceUrl] DEFAULT N'',
    CONSTRAINT [PK_People] PRIMARY KEY ([Id])
);

CREATE TABLE [dbo].[MovieGenres] (
    [MovieId] int NOT NULL,
    [GenreId] int NOT NULL,
    CONSTRAINT [PK_MovieGenres] PRIMARY KEY ([MovieId], [GenreId]),
    CONSTRAINT [FK_MovieGenres_Movies_MovieId] FOREIGN KEY ([MovieId]) REFERENCES [dbo].[Movies]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_MovieGenres_Genres_GenreId] FOREIGN KEY ([GenreId]) REFERENCES [dbo].[Genres]([Id]) ON DELETE CASCADE
);

CREATE TABLE [dbo].[MovieCredits] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [MovieId] int NOT NULL,
    [PersonId] int NOT NULL,
    [CreditType] int NOT NULL,
    [CharacterName] nvarchar(180) NULL,
    [BillingOrder] int NOT NULL,
    CONSTRAINT [PK_MovieCredits] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MovieCredits_Movies_MovieId] FOREIGN KEY ([MovieId]) REFERENCES [dbo].[Movies]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_MovieCredits_People_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[People]([Id]),
    CONSTRAINT [CK_MovieCredits_CreditType] CHECK ([CreditType] IN (1,2))
);

CREATE TABLE [dbo].[WatchLists] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(500) NOT NULL,
    [CreatedAt] datetime2(7) NOT NULL CONSTRAINT [DF_WatchLists_CreatedAt] DEFAULT SYSUTCDATETIME(),
    [UpdatedAt] datetime2(7) NOT NULL CONSTRAINT [DF_WatchLists_UpdatedAt] DEFAULT SYSUTCDATETIME(),
    CONSTRAINT [PK_WatchLists] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_WatchLists_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]) ON DELETE CASCADE
);

CREATE TABLE [dbo].[WatchListMovies] (
    [WatchListId] int NOT NULL,
    [MovieId] int NOT NULL,
    [AddedAt] datetime2(7) NOT NULL CONSTRAINT [DF_WatchListMovies_AddedAt] DEFAULT SYSUTCDATETIME(),
    CONSTRAINT [PK_WatchListMovies] PRIMARY KEY ([WatchListId], [MovieId]),
    CONSTRAINT [FK_WatchListMovies_WatchLists_WatchListId] FOREIGN KEY ([WatchListId]) REFERENCES [dbo].[WatchLists]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_WatchListMovies_Movies_MovieId] FOREIGN KEY ([MovieId]) REFERENCES [dbo].[Movies]([Id]) ON DELETE CASCADE
);

CREATE TABLE [dbo].[Reviews] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    [MovieId] int NOT NULL,
    [Score] int NOT NULL,
    [Comment] nvarchar(1000) NULL,
    [CreatedAt] datetime2(7) NOT NULL CONSTRAINT [DF_Reviews_CreatedAt] DEFAULT SYSUTCDATETIME(),
    [UpdatedAt] datetime2(7) NOT NULL CONSTRAINT [DF_Reviews_UpdatedAt] DEFAULT SYSUTCDATETIME(),
    CONSTRAINT [PK_Reviews] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Reviews_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Reviews_Movies_MovieId] FOREIGN KEY ([MovieId]) REFERENCES [dbo].[Movies]([Id]) ON DELETE CASCADE,
    CONSTRAINT [CK_Reviews_Score] CHECK ([Score] BETWEEN 1 AND 10)
);

CREATE TABLE [dbo].[PlaybackProgresses] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    [MovieId] int NOT NULL,
    [CurrentSecond] float NOT NULL,
    [TotalSeconds] float NOT NULL,
    [IsCompleted] bit NOT NULL,
    [LastPlayedAt] datetime2(7) NOT NULL CONSTRAINT [DF_PlaybackProgresses_LastPlayedAt] DEFAULT SYSUTCDATETIME(),
    CONSTRAINT [PK_PlaybackProgresses] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PlaybackProgresses_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_PlaybackProgresses_Movies_MovieId] FOREIGN KEY ([MovieId]) REFERENCES [dbo].[Movies]([Id]) ON DELETE CASCADE,
    CONSTRAINT [CK_PlaybackProgress_CurrentSecond] CHECK ([CurrentSecond] >= 0),
    CONSTRAINT [CK_PlaybackProgress_TotalSeconds] CHECK ([TotalSeconds] >= 0)
);
GO

CREATE UNIQUE INDEX [IX_Users_Email] ON [dbo].[Users]([Email]);
CREATE UNIQUE INDEX [IX_Users_UserName] ON [dbo].[Users]([UserName]);
CREATE UNIQUE INDEX [IX_Genres_Name] ON [dbo].[Genres]([Name]);
CREATE INDEX [IX_Movies_Title] ON [dbo].[Movies]([Title]);
CREATE INDEX [IX_Movies_ReleaseYear] ON [dbo].[Movies]([ReleaseYear]);
CREATE INDEX [IX_People_FullName] ON [dbo].[People]([FullName]);
CREATE INDEX [IX_MovieGenres_GenreId] ON [dbo].[MovieGenres]([GenreId]);
CREATE UNIQUE INDEX [IX_MovieCredits_MovieId_PersonId_CreditType] ON [dbo].[MovieCredits]([MovieId], [PersonId], [CreditType]);
CREATE INDEX [IX_MovieCredits_PersonId] ON [dbo].[MovieCredits]([PersonId]);
CREATE UNIQUE INDEX [IX_WatchLists_UserId_Name] ON [dbo].[WatchLists]([UserId], [Name]);
CREATE INDEX [IX_WatchListMovies_MovieId] ON [dbo].[WatchListMovies]([MovieId]);
CREATE UNIQUE INDEX [IX_Reviews_UserId_MovieId] ON [dbo].[Reviews]([UserId], [MovieId]);
CREATE INDEX [IX_Reviews_MovieId] ON [dbo].[Reviews]([MovieId]);
CREATE UNIQUE INDEX [IX_PlaybackProgresses_UserId_MovieId] ON [dbo].[PlaybackProgresses]([UserId], [MovieId]);
CREATE INDEX [IX_PlaybackProgresses_MovieId] ON [dbo].[PlaybackProgresses]([MovieId]);
GO

BEGIN TRANSACTION;

INSERT INTO [dbo].[Users] ([Id],[Email],[UserName],[DisplayName],[PasswordHash],[CreatedAt]) VALUES ('11111111-1111-1111-1111-111111111111',N'demo@cinestream.cr',N'demo',N'Usuario Demo',N'120000.ABEiM0RVZneImaq7zN3u/w==.CN2+OSskixJ/3obvkvizFVia25ONXrwoImMinTSWxGw=',SYSUTCDATETIME());
INSERT INTO [dbo].[Users] ([Id],[Email],[UserName],[DisplayName],[PasswordHash],[CreatedAt]) VALUES ('22222222-2222-2222-2222-222222222222',N'ana@cinestream.cr',N'ana',N'Ana Vargas',N'120000.ECEyQ1RldoeYqbrL3O3+Dw==.WxiJhw1ZwAYnyGRwQG/jpBmxRAAzBcO9jZuzlAf1T6A=',SYSUTCDATETIME());

SET IDENTITY_INSERT [dbo].[Genres] ON;
INSERT INTO [dbo].[Genres] ([Id],[Name]) VALUES (1,N'Acción');
INSERT INTO [dbo].[Genres] ([Id],[Name]) VALUES (2,N'Aventura');
INSERT INTO [dbo].[Genres] ([Id],[Name]) VALUES (3,N'Ciencia ficción');
INSERT INTO [dbo].[Genres] ([Id],[Name]) VALUES (4,N'Crimen');
INSERT INTO [dbo].[Genres] ([Id],[Name]) VALUES (5,N'Drama');
INSERT INTO [dbo].[Genres] ([Id],[Name]) VALUES (6,N'Fantasía');
INSERT INTO [dbo].[Genres] ([Id],[Name]) VALUES (7,N'Misterio');
INSERT INTO [dbo].[Genres] ([Id],[Name]) VALUES (8,N'Romance');
INSERT INTO [dbo].[Genres] ([Id],[Name]) VALUES (9,N'Superhéroes');
INSERT INTO [dbo].[Genres] ([Id],[Name]) VALUES (10,N'Suspenso');
SET IDENTITY_INSERT [dbo].[Genres] OFF;

SET IDENTITY_INSERT [dbo].[Movies] ON;
INSERT INTO [dbo].[Movies] ([Id],[Title],[Synopsis],[ReleaseYear],[DurationMinutes],[PosterUrl],[BackdropUrl],[VideoUrl],[InformationSourceUrl],[ImageSourceUrl],[IsFeatured],[IsActive],[CreatedAt]) VALUES (1,N'The Lord of the Rings: The Fellowship of the Ring',N'En la Tierra Media, el joven hobbit Frodo Bolsón hereda un anillo de poder que debe ser destruido antes de que el señor oscuro Sauron lo recupere. Acompañado por una comunidad de héroes, inicia un viaje peligroso hacia Mordor.',2001,178,N'/api/media/wiki-thumbnail?title=The_Lord_of_the_Rings:_The_Fellowship_of_the_Ring&fallback=%2Fimages%2Fposters%2Fmovie-1.jpg',N'/images/backdrops/movie-1.jpg',N'/videos/video01.mp4',N'https://en.wikipedia.org/wiki/The_Lord_of_the_Rings:_The_Fellowship_of_the_Ring',N'https://en.wikipedia.org/wiki/The_Lord_of_the_Rings:_The_Fellowship_of_the_Ring',1,1,SYSUTCDATETIME());
INSERT INTO [dbo].[Movies] ([Id],[Title],[Synopsis],[ReleaseYear],[DurationMinutes],[PosterUrl],[BackdropUrl],[VideoUrl],[InformationSourceUrl],[ImageSourceUrl],[IsFeatured],[IsActive],[CreatedAt]) VALUES (2,N'Star Wars: Episode IV – A New Hope',N'Luke Skywalker abandona su vida en Tatooine para unirse a la Alianza Rebelde. Junto a Han Solo y la princesa Leia, intenta destruir la Estrella de la Muerte y enfrentarse al Imperio Galáctico.',1977,121,N'/api/media/wiki-thumbnail?title=Star_Wars_(film)&fallback=%2Fimages%2Fposters%2Fmovie-2.jpg',N'/images/backdrops/movie-2.jpg',N'/videos/video02.mp4',N'https://en.wikipedia.org/wiki/Star_Wars_(film)',N'https://en.wikipedia.org/wiki/Star_Wars_(film)',0,1,SYSUTCDATETIME());
INSERT INTO [dbo].[Movies] ([Id],[Title],[Synopsis],[ReleaseYear],[DurationMinutes],[PosterUrl],[BackdropUrl],[VideoUrl],[InformationSourceUrl],[ImageSourceUrl],[IsFeatured],[IsActive],[CreatedAt]) VALUES (3,N'Harry Potter and the Philosopher''s Stone',N'Harry Potter descubre que es un mago y comienza sus estudios en Hogwarts. Allí forma amistad con Ron y Hermione, conoce el mundo mágico y se enfrenta a un misterio relacionado con la Piedra Filosofal.',2001,152,N'/api/media/wiki-thumbnail?title=Harry_Potter_and_the_Philosopher%27s_Stone_(film)&fallback=%2Fimages%2Fposters%2Fmovie-3.jpg',N'/images/backdrops/movie-3.jpg',N'/videos/video03.mp4',N'https://en.wikipedia.org/wiki/Harry_Potter_and_the_Philosopher%27s_Stone_(film)',N'https://en.wikipedia.org/wiki/Harry_Potter_and_the_Philosopher%27s_Stone_(film)',0,1,SYSUTCDATETIME());
INSERT INTO [dbo].[Movies] ([Id],[Title],[Synopsis],[ReleaseYear],[DurationMinutes],[PosterUrl],[BackdropUrl],[VideoUrl],[InformationSourceUrl],[ImageSourceUrl],[IsFeatured],[IsActive],[CreatedAt]) VALUES (4,N'The Dark Knight',N'Batman, el fiscal Harvey Dent y el comisionado Gordon intentan detener al crimen organizado de Gotham. Su alianza es puesta a prueba cuando el Joker inicia una campaña de caos que obliga al héroe a enfrentar límites morales.',2008,152,N'/api/media/wiki-thumbnail?title=The_Dark_Knight&fallback=%2Fimages%2Fposters%2Fmovie-4.jpg',N'/images/backdrops/movie-4.jpg',N'/videos/video04.mp4',N'https://en.wikipedia.org/wiki/The_Dark_Knight',N'https://en.wikipedia.org/wiki/The_Dark_Knight',0,1,SYSUTCDATETIME());
INSERT INTO [dbo].[Movies] ([Id],[Title],[Synopsis],[ReleaseYear],[DurationMinutes],[PosterUrl],[BackdropUrl],[VideoUrl],[InformationSourceUrl],[ImageSourceUrl],[IsFeatured],[IsActive],[CreatedAt]) VALUES (5,N'Avengers: Endgame',N'Después de la devastación causada por Thanos, los Vengadores supervivientes buscan una última oportunidad para revertir la pérdida y restaurar el universo, aunque la misión exige sacrificios decisivos.',2019,181,N'/api/media/wiki-thumbnail?title=Avengers:_Endgame&fallback=%2Fimages%2Fposters%2Fmovie-5.jpg',N'/images/backdrops/movie-5.jpg',N'/videos/video05.mp4',N'https://en.wikipedia.org/wiki/Avengers:_Endgame',N'https://en.wikipedia.org/wiki/Avengers:_Endgame',0,1,SYSUTCDATETIME());
INSERT INTO [dbo].[Movies] ([Id],[Title],[Synopsis],[ReleaseYear],[DurationMinutes],[PosterUrl],[BackdropUrl],[VideoUrl],[InformationSourceUrl],[ImageSourceUrl],[IsFeatured],[IsActive],[CreatedAt]) VALUES (6,N'Jurassic Park',N'Un grupo de especialistas visita un parque temático en Isla Nublar, cerca de Costa Rica, donde dinosaurios clonados han vuelto a la vida. Una falla de seguridad convierte la visita en una lucha por sobrevivir.',1993,127,N'/api/media/wiki-thumbnail?title=Jurassic_Park_(film)&fallback=%2Fimages%2Fposters%2Fmovie-6.jpg',N'/images/backdrops/movie-6.jpg',N'/videos/video06.mp4',N'https://en.wikipedia.org/wiki/Jurassic_Park_(film)',N'https://en.wikipedia.org/wiki/Jurassic_Park_(film)',0,1,SYSUTCDATETIME());
INSERT INTO [dbo].[Movies] ([Id],[Title],[Synopsis],[ReleaseYear],[DurationMinutes],[PosterUrl],[BackdropUrl],[VideoUrl],[InformationSourceUrl],[ImageSourceUrl],[IsFeatured],[IsActive],[CreatedAt]) VALUES (7,N'Titanic',N'A bordo del RMS Titanic, Jack Dawson y Rose DeWitt Bukater, jóvenes de clases sociales distintas, se enamoran durante el viaje inaugural del barco, cuya colisión con un iceberg cambia sus vidas para siempre.',1997,195,N'/api/media/wiki-thumbnail?title=Titanic_(1997_film)&fallback=%2Fimages%2Fposters%2Fmovie-7.jpg',N'/images/backdrops/movie-7.jpg',N'/videos/video07.mp4',N'https://en.wikipedia.org/wiki/Titanic_(1997_film)',N'https://en.wikipedia.org/wiki/Titanic_(1997_film)',0,1,SYSUTCDATETIME());
INSERT INTO [dbo].[Movies] ([Id],[Title],[Synopsis],[ReleaseYear],[DurationMinutes],[PosterUrl],[BackdropUrl],[VideoUrl],[InformationSourceUrl],[ImageSourceUrl],[IsFeatured],[IsActive],[CreatedAt]) VALUES (8,N'The Matrix',N'El programador Thomas Anderson, conocido como Neo, descubre que la realidad que conoce es una simulación creada por máquinas. Morfeo y Trinity lo introducen en una rebelión que cuestiona la libertad y la identidad.',1999,136,N'/api/media/wiki-thumbnail?title=The_Matrix&fallback=%2Fimages%2Fposters%2Fmovie-8.jpg',N'/images/backdrops/movie-8.jpg',N'/videos/video08.mp4',N'https://en.wikipedia.org/wiki/The_Matrix',N'https://en.wikipedia.org/wiki/The_Matrix',0,1,SYSUTCDATETIME());
INSERT INTO [dbo].[Movies] ([Id],[Title],[Synopsis],[ReleaseYear],[DurationMinutes],[PosterUrl],[BackdropUrl],[VideoUrl],[InformationSourceUrl],[ImageSourceUrl],[IsFeatured],[IsActive],[CreatedAt]) VALUES (9,N'Gladiator',N'El general romano Máximo es traicionado por Cómodo y reducido a la esclavitud. Convertido en gladiador, busca justicia mientras asciende en la arena y desafía al nuevo emperador de Roma.',2000,155,N'/api/media/wiki-thumbnail?title=Gladiator_(2000_film)&fallback=%2Fimages%2Fposters%2Fmovie-9.jpg',N'/images/backdrops/movie-9.jpg',N'/videos/video09.mp4',N'https://en.wikipedia.org/wiki/Gladiator_(2000_film)',N'https://en.wikipedia.org/wiki/Gladiator_(2000_film)',0,1,SYSUTCDATETIME());
INSERT INTO [dbo].[Movies] ([Id],[Title],[Synopsis],[ReleaseYear],[DurationMinutes],[PosterUrl],[BackdropUrl],[VideoUrl],[InformationSourceUrl],[ImageSourceUrl],[IsFeatured],[IsActive],[CreatedAt]) VALUES (10,N'Pirates of the Caribbean: The Curse of the Black Pearl',N'El herrero Will Turner se une al impredecible capitán Jack Sparrow para rescatar a Elizabeth Swann de una tripulación pirata condenada por una antigua maldición azteca.',2003,143,N'/api/media/wiki-thumbnail?title=Pirates_of_the_Caribbean:_The_Curse_of_the_Black_Pearl&fallback=%2Fimages%2Fposters%2Fmovie-10.jpg',N'/images/backdrops/movie-10.jpg',N'/videos/video10.mp4',N'https://en.wikipedia.org/wiki/Pirates_of_the_Caribbean:_The_Curse_of_the_Black_Pearl',N'https://en.wikipedia.org/wiki/Pirates_of_the_Caribbean:_The_Curse_of_the_Black_Pearl',0,1,SYSUTCDATETIME());
INSERT INTO [dbo].[Movies] ([Id],[Title],[Synopsis],[ReleaseYear],[DurationMinutes],[PosterUrl],[BackdropUrl],[VideoUrl],[InformationSourceUrl],[ImageSourceUrl],[IsFeatured],[IsActive],[CreatedAt]) VALUES (11,N'Interstellar',N'Con la Tierra al borde del colapso, el expiloto Cooper se une a una misión espacial que atraviesa un agujero de gusano en busca de un nuevo hogar para la humanidad, mientras lucha contra el tiempo y la distancia.',2014,169,N'/api/media/wiki-thumbnail?title=Interstellar_(film)&fallback=%2Fimages%2Fposters%2Fmovie-11.jpg',N'/images/backdrops/movie-11.jpg',N'/videos/video11.mp4',N'https://en.wikipedia.org/wiki/Interstellar_(film)',N'https://en.wikipedia.org/wiki/Interstellar_(film)',0,1,SYSUTCDATETIME());
INSERT INTO [dbo].[Movies] ([Id],[Title],[Synopsis],[ReleaseYear],[DurationMinutes],[PosterUrl],[BackdropUrl],[VideoUrl],[InformationSourceUrl],[ImageSourceUrl],[IsFeatured],[IsActive],[CreatedAt]) VALUES (12,N'Inception',N'Dom Cobb dirige un equipo capaz de entrar en los sueños para robar secretos. Para recuperar su vida, acepta la misión inversa: implantar una idea en la mente de un heredero empresarial.',2010,148,N'/api/media/wiki-thumbnail?title=Inception&fallback=%2Fimages%2Fposters%2Fmovie-12.jpg',N'/images/backdrops/movie-12.jpg',N'/videos/video12.mp4',N'https://en.wikipedia.org/wiki/Inception',N'https://en.wikipedia.org/wiki/Inception',0,1,SYSUTCDATETIME());
SET IDENTITY_INSERT [dbo].[Movies] OFF;

SET IDENTITY_INSERT [dbo].[People] ON;
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (1,N'Peter Jackson',N'Neozelandés',N'Cineasta neozelandés conocido por dirigir, escribir y producir las trilogías cinematográficas de The Lord of the Rings y The Hobbit.',CONVERT(date,'1961-10-31',23),N'/api/media/wiki-thumbnail?title=Peter_Jackson&fallback=%2Fimages%2Fpeople%2Fperson-1.jpg',N'https://en.wikipedia.org/wiki/Peter_Jackson',N'https://en.wikipedia.org/wiki/Peter_Jackson');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (2,N'Elijah Wood',N'Estadounidense',N'Actor estadounidense reconocido internacionalmente por interpretar a Frodo Baggins en la trilogía cinematográfica de The Lord of the Rings.',CONVERT(date,'1981-01-28',23),N'/api/media/wiki-thumbnail?title=Elijah_Wood&fallback=%2Fimages%2Fpeople%2Fperson-2.jpg',N'https://en.wikipedia.org/wiki/Elijah_Wood',N'https://en.wikipedia.org/wiki/Elijah_Wood');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (3,N'Ian McKellen',N'Británico',N'Actor británico de teatro y cine, ampliamente reconocido por sus interpretaciones de Gandalf y Magneto y por una extensa carrera shakespeariana.',CONVERT(date,'1939-05-25',23),N'/api/media/wiki-thumbnail?title=Ian_McKellen&fallback=%2Fimages%2Fpeople%2Fperson-3.jpg',N'https://en.wikipedia.org/wiki/Ian_McKellen',N'https://en.wikipedia.org/wiki/Ian_McKellen');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (4,N'Viggo Mortensen',N'Estadounidense-danés',N'Actor, escritor y artista estadounidense-danés conocido por interpretar a Aragorn y por su trabajo en cine independiente y dramático.',CONVERT(date,'1958-10-20',23),N'/api/media/wiki-thumbnail?title=Viggo_Mortensen&fallback=%2Fimages%2Fpeople%2Fperson-4.jpg',N'https://en.wikipedia.org/wiki/Viggo_Mortensen',N'https://en.wikipedia.org/wiki/Viggo_Mortensen');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (5,N'George Lucas',N'Estadounidense',N'Cineasta estadounidense creador de Star Wars e Indiana Jones y fundador de Lucasfilm e Industrial Light & Magic.',CONVERT(date,'1944-05-14',23),N'/api/media/wiki-thumbnail?title=George_Lucas&fallback=%2Fimages%2Fpeople%2Fperson-5.jpg',N'https://en.wikipedia.org/wiki/George_Lucas',N'https://en.wikipedia.org/wiki/George_Lucas');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (6,N'Mark Hamill',N'Estadounidense',N'Actor estadounidense conocido por interpretar a Luke Skywalker y por una destacada carrera como actor de voz.',CONVERT(date,'1951-09-25',23),N'/api/media/wiki-thumbnail?title=Mark_Hamill&fallback=%2Fimages%2Fpeople%2Fperson-6.jpg',N'https://en.wikipedia.org/wiki/Mark_Hamill',N'https://en.wikipedia.org/wiki/Mark_Hamill');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (7,N'Harrison Ford',N'Estadounidense',N'Actor estadounidense célebre por sus papeles como Han Solo, Indiana Jones y Rick Deckard.',CONVERT(date,'1942-07-13',23),N'/api/media/wiki-thumbnail?title=Harrison_Ford&fallback=%2Fimages%2Fpeople%2Fperson-7.jpg',N'https://en.wikipedia.org/wiki/Harrison_Ford',N'https://en.wikipedia.org/wiki/Harrison_Ford');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (8,N'Carrie Fisher',N'Estadounidense',N'Actriz y escritora estadounidense recordada por interpretar a la princesa Leia en Star Wars.',CONVERT(date,'1956-10-21',23),N'/api/media/wiki-thumbnail?title=Carrie_Fisher&fallback=%2Fimages%2Fpeople%2Fperson-8.jpg',N'https://en.wikipedia.org/wiki/Carrie_Fisher',N'https://en.wikipedia.org/wiki/Carrie_Fisher');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (9,N'Chris Columbus',N'Estadounidense',N'Director, productor y guionista estadounidense que dirigió las dos primeras películas de Harry Potter y varias comedias familiares.',CONVERT(date,'1958-09-10',23),N'/api/media/wiki-thumbnail?title=Chris_Columbus_%28filmmaker%29&fallback=%2Fimages%2Fpeople%2Fperson-9.jpg',N'https://en.wikipedia.org/wiki/Chris_Columbus_(filmmaker)',N'https://en.wikipedia.org/wiki/Chris_Columbus_(filmmaker)');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (10,N'Daniel Radcliffe',N'Británico',N'Actor británico conocido por interpretar a Harry Potter en la serie cinematográfica basada en las novelas de J. K. Rowling.',CONVERT(date,'1989-07-23',23),N'/api/media/wiki-thumbnail?title=Daniel_Radcliffe&fallback=%2Fimages%2Fpeople%2Fperson-10.jpg',N'https://en.wikipedia.org/wiki/Daniel_Radcliffe',N'https://en.wikipedia.org/wiki/Daniel_Radcliffe');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (11,N'Rupert Grint',N'Británico',N'Actor británico reconocido por su papel de Ron Weasley en las películas de Harry Potter.',CONVERT(date,'1988-08-24',23),N'/api/media/wiki-thumbnail?title=Rupert_Grint&fallback=%2Fimages%2Fpeople%2Fperson-11.jpg',N'https://en.wikipedia.org/wiki/Rupert_Grint',N'https://en.wikipedia.org/wiki/Rupert_Grint');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (12,N'Emma Watson',N'Británica',N'Actriz británica conocida por interpretar a Hermione Granger y por su trabajo posterior en cine y causas educativas.',CONVERT(date,'1990-04-15',23),N'/api/media/wiki-thumbnail?title=Emma_Watson&fallback=%2Fimages%2Fpeople%2Fperson-12.jpg',N'https://en.wikipedia.org/wiki/Emma_Watson',N'https://en.wikipedia.org/wiki/Emma_Watson');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (13,N'Christopher Nolan',N'Británico-estadounidense',N'Cineasta británico-estadounidense conocido por películas de gran escala con estructuras narrativas complejas, entre ellas The Dark Knight, Inception e Interstellar.',CONVERT(date,'1970-07-30',23),N'/api/media/wiki-thumbnail?title=Christopher_Nolan&fallback=%2Fimages%2Fpeople%2Fperson-13.jpg',N'https://en.wikipedia.org/wiki/Christopher_Nolan',N'https://en.wikipedia.org/wiki/Christopher_Nolan');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (14,N'Christian Bale',N'Británico',N'Actor británico ganador del Óscar, reconocido por sus transformaciones físicas y por interpretar a Bruce Wayne en la trilogía The Dark Knight.',CONVERT(date,'1974-01-30',23),N'/api/media/wiki-thumbnail?title=Christian_Bale&fallback=%2Fimages%2Fpeople%2Fperson-14.jpg',N'https://en.wikipedia.org/wiki/Christian_Bale',N'https://en.wikipedia.org/wiki/Christian_Bale');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (15,N'Heath Ledger',N'Australiano',N'Actor australiano ganador póstumo del Óscar por su interpretación del Joker en The Dark Knight.',CONVERT(date,'1979-04-04',23),N'/api/media/wiki-thumbnail?title=Heath_Ledger&fallback=%2Fimages%2Fpeople%2Fperson-15.jpg',N'https://en.wikipedia.org/wiki/Heath_Ledger',N'https://en.wikipedia.org/wiki/Heath_Ledger');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (16,N'Aaron Eckhart',N'Estadounidense',N'Actor estadounidense conocido por sus papeles en Thank You for Smoking y como Harvey Dent en The Dark Knight.',CONVERT(date,'1968-03-12',23),N'/api/media/wiki-thumbnail?title=Aaron_Eckhart&fallback=%2Fimages%2Fpeople%2Fperson-16.jpg',N'https://en.wikipedia.org/wiki/Aaron_Eckhart',N'https://en.wikipedia.org/wiki/Aaron_Eckhart');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (17,N'Anthony Russo',N'Estadounidense',N'Director y productor estadounidense que, junto con su hermano Joe Russo, dirigió varias películas del Universo Cinematográfico de Marvel.',CONVERT(date,'1970-02-03',23),N'/api/media/wiki-thumbnail?title=Russo_brothers&fallback=%2Fimages%2Fpeople%2Fperson-17.jpg',N'https://en.wikipedia.org/wiki/Russo_brothers',N'https://en.wikipedia.org/wiki/Russo_brothers');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (18,N'Joe Russo',N'Estadounidense',N'Director y productor estadounidense que forma con Anthony Russo el dúo responsable de Avengers: Infinity War y Avengers: Endgame.',CONVERT(date,'1971-07-18',23),N'/api/media/wiki-thumbnail?title=Russo_brothers&fallback=%2Fimages%2Fpeople%2Fperson-18.jpg',N'https://en.wikipedia.org/wiki/Russo_brothers',N'https://en.wikipedia.org/wiki/Russo_brothers');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (19,N'Robert Downey Jr.',N'Estadounidense',N'Actor estadounidense ganador del Óscar, conocido por interpretar a Tony Stark en el Universo Cinematográfico de Marvel.',CONVERT(date,'1965-04-04',23),N'/api/media/wiki-thumbnail?title=Robert_Downey_Jr.&fallback=%2Fimages%2Fpeople%2Fperson-19.jpg',N'https://en.wikipedia.org/wiki/Robert_Downey_Jr.',N'https://en.wikipedia.org/wiki/Robert_Downey_Jr.');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (20,N'Chris Evans',N'Estadounidense',N'Actor estadounidense conocido por interpretar a Steve Rogers, Captain America, en el Universo Cinematográfico de Marvel.',CONVERT(date,'1981-06-13',23),N'/api/media/wiki-thumbnail?title=Chris_Evans_%28actor%29&fallback=%2Fimages%2Fpeople%2Fperson-20.jpg',N'https://en.wikipedia.org/wiki/Chris_Evans_(actor)',N'https://en.wikipedia.org/wiki/Chris_Evans_(actor)');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (21,N'Scarlett Johansson',N'Estadounidense',N'Actriz estadounidense reconocida por una amplia carrera dramática y por interpretar a Natasha Romanoff en Marvel.',CONVERT(date,'1984-11-22',23),N'/api/media/wiki-thumbnail?title=Scarlett_Johansson&fallback=%2Fimages%2Fpeople%2Fperson-21.jpg',N'https://en.wikipedia.org/wiki/Scarlett_Johansson',N'https://en.wikipedia.org/wiki/Scarlett_Johansson');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (22,N'Steven Spielberg',N'Estadounidense',N'Director y productor estadounidense, figura central del cine moderno y responsable de títulos como Jaws, E.T., Jurassic Park y Schindler’s List.',CONVERT(date,'1946-12-18',23),N'/api/media/wiki-thumbnail?title=Steven_Spielberg&fallback=%2Fimages%2Fpeople%2Fperson-22.jpg',N'https://en.wikipedia.org/wiki/Steven_Spielberg',N'https://en.wikipedia.org/wiki/Steven_Spielberg');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (23,N'Sam Neill',N'Neozelandés',N'Actor neozelandés conocido por interpretar al paleontólogo Alan Grant en la franquicia Jurassic Park.',CONVERT(date,'1947-09-14',23),N'/api/media/wiki-thumbnail?title=Sam_Neill&fallback=%2Fimages%2Fpeople%2Fperson-23.jpg',N'https://en.wikipedia.org/wiki/Sam_Neill',N'https://en.wikipedia.org/wiki/Sam_Neill');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (24,N'Laura Dern',N'Estadounidense',N'Actriz estadounidense ganadora del Óscar, conocida por Jurassic Park, Marriage Story y numerosas producciones de cine y televisión.',CONVERT(date,'1967-02-10',23),N'/api/media/wiki-thumbnail?title=Laura_Dern&fallback=%2Fimages%2Fpeople%2Fperson-24.jpg',N'https://en.wikipedia.org/wiki/Laura_Dern',N'https://en.wikipedia.org/wiki/Laura_Dern');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (25,N'Jeff Goldblum',N'Estadounidense',N'Actor estadounidense conocido por The Fly, Jurassic Park, Independence Day y su estilo interpretativo distintivo.',CONVERT(date,'1952-10-22',23),N'/api/media/wiki-thumbnail?title=Jeff_Goldblum&fallback=%2Fimages%2Fpeople%2Fperson-25.jpg',N'https://en.wikipedia.org/wiki/Jeff_Goldblum',N'https://en.wikipedia.org/wiki/Jeff_Goldblum');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (26,N'James Cameron',N'Canadiense',N'Director, guionista y productor canadiense conocido por Titanic, Avatar, Aliens y Terminator 2.',CONVERT(date,'1954-08-16',23),N'/api/media/wiki-thumbnail?title=James_Cameron&fallback=%2Fimages%2Fpeople%2Fperson-26.jpg',N'https://en.wikipedia.org/wiki/James_Cameron',N'https://en.wikipedia.org/wiki/James_Cameron');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (27,N'Leonardo DiCaprio',N'Estadounidense',N'Actor y productor estadounidense ganador del Óscar, protagonista de Titanic, Inception y The Revenant.',CONVERT(date,'1974-11-11',23),N'/api/media/wiki-thumbnail?title=Leonardo_DiCaprio&fallback=%2Fimages%2Fpeople%2Fperson-27.jpg',N'https://en.wikipedia.org/wiki/Leonardo_DiCaprio',N'https://en.wikipedia.org/wiki/Leonardo_DiCaprio');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (28,N'Kate Winslet',N'Británica',N'Actriz británica ganadora del Óscar, reconocida por Titanic, The Reader y una extensa carrera en drama.',CONVERT(date,'1975-10-05',23),N'/api/media/wiki-thumbnail?title=Kate_Winslet&fallback=%2Fimages%2Fpeople%2Fperson-28.jpg',N'https://en.wikipedia.org/wiki/Kate_Winslet',N'https://en.wikipedia.org/wiki/Kate_Winslet');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (29,N'Billy Zane',N'Estadounidense',N'Actor estadounidense conocido por interpretar a Caledon Hockley en Titanic y por papeles en cine y televisión.',CONVERT(date,'1966-02-24',23),N'/api/media/wiki-thumbnail?title=Billy_Zane&fallback=%2Fimages%2Fpeople%2Fperson-29.jpg',N'https://en.wikipedia.org/wiki/Billy_Zane',N'https://en.wikipedia.org/wiki/Billy_Zane');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (30,N'Lana Wachowski',N'Estadounidense',N'Cineasta estadounidense que, junto con Lilly Wachowski, creó y dirigió The Matrix y otras obras de ciencia ficción.',CONVERT(date,'1965-06-21',23),N'/api/media/wiki-thumbnail?title=The_Wachowskis&fallback=%2Fimages%2Fpeople%2Fperson-30.jpg',N'https://en.wikipedia.org/wiki/The_Wachowskis',N'https://en.wikipedia.org/wiki/The_Wachowskis');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (31,N'Lilly Wachowski',N'Estadounidense',N'Cineasta estadounidense conocida por su colaboración con Lana Wachowski en The Matrix, Cloud Atlas y Sense8.',CONVERT(date,'1967-12-29',23),N'/api/media/wiki-thumbnail?title=The_Wachowskis&fallback=%2Fimages%2Fpeople%2Fperson-31.jpg',N'https://en.wikipedia.org/wiki/The_Wachowskis',N'https://en.wikipedia.org/wiki/The_Wachowskis');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (32,N'Keanu Reeves',N'Canadiense',N'Actor canadiense conocido por The Matrix, John Wick, Speed y una amplia carrera en cine de acción y ciencia ficción.',CONVERT(date,'1964-09-02',23),N'/api/media/wiki-thumbnail?title=Keanu_Reeves&fallback=%2Fimages%2Fpeople%2Fperson-32.jpg',N'https://en.wikipedia.org/wiki/Keanu_Reeves',N'https://en.wikipedia.org/wiki/Keanu_Reeves');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (33,N'Laurence Fishburne',N'Estadounidense',N'Actor estadounidense reconocido por interpretar a Morpheus en The Matrix y por una extensa trayectoria en teatro, cine y televisión.',CONVERT(date,'1961-07-30',23),N'/api/media/wiki-thumbnail?title=Laurence_Fishburne&fallback=%2Fimages%2Fpeople%2Fperson-33.jpg',N'https://en.wikipedia.org/wiki/Laurence_Fishburne',N'https://en.wikipedia.org/wiki/Laurence_Fishburne');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (34,N'Carrie-Anne Moss',N'Canadiense',N'Actriz canadiense conocida principalmente por interpretar a Trinity en la franquicia The Matrix.',CONVERT(date,'1967-08-21',23),N'/api/media/wiki-thumbnail?title=Carrie-Anne_Moss&fallback=%2Fimages%2Fpeople%2Fperson-34.jpg',N'https://en.wikipedia.org/wiki/Carrie-Anne_Moss',N'https://en.wikipedia.org/wiki/Carrie-Anne_Moss');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (35,N'Ridley Scott',N'Británico',N'Director y productor británico conocido por Alien, Blade Runner, Gladiator y The Martian.',CONVERT(date,'1937-11-30',23),N'/api/media/wiki-thumbnail?title=Ridley_Scott&fallback=%2Fimages%2Fpeople%2Fperson-35.jpg',N'https://en.wikipedia.org/wiki/Ridley_Scott',N'https://en.wikipedia.org/wiki/Ridley_Scott');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (36,N'Russell Crowe',N'Neozelandés-australiano',N'Actor neozelandés-australiano ganador del Óscar por su interpretación de Máximo en Gladiator.',CONVERT(date,'1964-04-07',23),N'/api/media/wiki-thumbnail?title=Russell_Crowe&fallback=%2Fimages%2Fpeople%2Fperson-36.jpg',N'https://en.wikipedia.org/wiki/Russell_Crowe',N'https://en.wikipedia.org/wiki/Russell_Crowe');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (37,N'Joaquin Phoenix',N'Estadounidense',N'Actor estadounidense ganador del Óscar, reconocido por Joker, Gladiator, Walk the Line y The Master.',CONVERT(date,'1974-10-28',23),N'/api/media/wiki-thumbnail?title=Joaquin_Phoenix&fallback=%2Fimages%2Fpeople%2Fperson-37.jpg',N'https://en.wikipedia.org/wiki/Joaquin_Phoenix',N'https://en.wikipedia.org/wiki/Joaquin_Phoenix');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (38,N'Connie Nielsen',N'Danesa',N'Actriz danesa conocida por interpretar a Lucilla en Gladiator y por sus trabajos en cine europeo y estadounidense.',CONVERT(date,'1965-07-03',23),N'/api/media/wiki-thumbnail?title=Connie_Nielsen&fallback=%2Fimages%2Fpeople%2Fperson-38.jpg',N'https://en.wikipedia.org/wiki/Connie_Nielsen',N'https://en.wikipedia.org/wiki/Connie_Nielsen');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (39,N'Gore Verbinski',N'Estadounidense',N'Director estadounidense conocido por The Ring, Rango y las primeras películas de Pirates of the Caribbean.',CONVERT(date,'1964-03-16',23),N'/api/media/wiki-thumbnail?title=Gore_Verbinski&fallback=%2Fimages%2Fpeople%2Fperson-39.jpg',N'https://en.wikipedia.org/wiki/Gore_Verbinski',N'https://en.wikipedia.org/wiki/Gore_Verbinski');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (40,N'Johnny Depp',N'Estadounidense',N'Actor estadounidense conocido por interpretar al capitán Jack Sparrow y por sus colaboraciones con diversos directores.',CONVERT(date,'1963-06-09',23),N'/api/media/wiki-thumbnail?title=Johnny_Depp&fallback=%2Fimages%2Fpeople%2Fperson-40.jpg',N'https://en.wikipedia.org/wiki/Johnny_Depp',N'https://en.wikipedia.org/wiki/Johnny_Depp');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (41,N'Geoffrey Rush',N'Australiano',N'Actor australiano ganador del Óscar, conocido por Shine, The King’s Speech y su papel de Barbossa.',CONVERT(date,'1951-07-06',23),N'/api/media/wiki-thumbnail?title=Geoffrey_Rush&fallback=%2Fimages%2Fpeople%2Fperson-41.jpg',N'https://en.wikipedia.org/wiki/Geoffrey_Rush',N'https://en.wikipedia.org/wiki/Geoffrey_Rush');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (42,N'Orlando Bloom',N'Británico',N'Actor británico conocido por interpretar a Legolas en The Lord of the Rings y a Will Turner en Pirates of the Caribbean.',CONVERT(date,'1977-01-13',23),N'/api/media/wiki-thumbnail?title=Orlando_Bloom&fallback=%2Fimages%2Fpeople%2Fperson-42.jpg',N'https://en.wikipedia.org/wiki/Orlando_Bloom',N'https://en.wikipedia.org/wiki/Orlando_Bloom');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (43,N'Keira Knightley',N'Británica',N'Actriz británica conocida por Pride & Prejudice, Atonement y su papel de Elizabeth Swann.',CONVERT(date,'1985-03-26',23),N'/api/media/wiki-thumbnail?title=Keira_Knightley&fallback=%2Fimages%2Fpeople%2Fperson-43.jpg',N'https://en.wikipedia.org/wiki/Keira_Knightley',N'https://en.wikipedia.org/wiki/Keira_Knightley');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (44,N'Matthew McConaughey',N'Estadounidense',N'Actor estadounidense ganador del Óscar por Dallas Buyers Club y protagonista de Interstellar y True Detective.',CONVERT(date,'1969-11-04',23),N'/api/media/wiki-thumbnail?title=Matthew_McConaughey&fallback=%2Fimages%2Fpeople%2Fperson-44.jpg',N'https://en.wikipedia.org/wiki/Matthew_McConaughey',N'https://en.wikipedia.org/wiki/Matthew_McConaughey');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (45,N'Anne Hathaway',N'Estadounidense',N'Actriz estadounidense ganadora del Óscar, conocida por Les Misérables, The Devil Wears Prada e Interstellar.',CONVERT(date,'1982-11-12',23),N'/api/media/wiki-thumbnail?title=Anne_Hathaway&fallback=%2Fimages%2Fpeople%2Fperson-45.jpg',N'https://en.wikipedia.org/wiki/Anne_Hathaway',N'https://en.wikipedia.org/wiki/Anne_Hathaway');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (46,N'Jessica Chastain',N'Estadounidense',N'Actriz y productora estadounidense ganadora del Óscar, conocida por Zero Dark Thirty, Interstellar y The Eyes of Tammy Faye.',CONVERT(date,'1977-03-24',23),N'/api/media/wiki-thumbnail?title=Jessica_Chastain&fallback=%2Fimages%2Fpeople%2Fperson-46.jpg',N'https://en.wikipedia.org/wiki/Jessica_Chastain',N'https://en.wikipedia.org/wiki/Jessica_Chastain');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (47,N'Joseph Gordon-Levitt',N'Estadounidense',N'Actor y cineasta estadounidense conocido por Inception, 500 Days of Summer y Looper.',CONVERT(date,'1981-02-17',23),N'/api/media/wiki-thumbnail?title=Joseph_Gordon-Levitt&fallback=%2Fimages%2Fpeople%2Fperson-47.jpg',N'https://en.wikipedia.org/wiki/Joseph_Gordon-Levitt',N'https://en.wikipedia.org/wiki/Joseph_Gordon-Levitt');
INSERT INTO [dbo].[People] ([Id],[FullName],[Nationality],[Biography],[BirthDate],[PhotoUrl],[InformationSourceUrl],[ImageSourceUrl]) VALUES (48,N'Elliot Page',N'Canadiense',N'Actor canadiense conocido por Juno, Inception, Hard Candy y la serie The Umbrella Academy.',CONVERT(date,'1987-02-21',23),N'/api/media/wiki-thumbnail?title=Elliot_Page&fallback=%2Fimages%2Fpeople%2Fperson-48.jpg',N'https://en.wikipedia.org/wiki/Elliot_Page',N'https://en.wikipedia.org/wiki/Elliot_Page');
SET IDENTITY_INSERT [dbo].[People] OFF;

INSERT INTO [dbo].[MovieGenres] ([MovieId],[GenreId]) VALUES (1,2);
INSERT INTO [dbo].[MovieGenres] ([MovieId],[GenreId]) VALUES (1,6);
INSERT INTO [dbo].[MovieGenres] ([MovieId],[GenreId]) VALUES (1,5);
INSERT INTO [dbo].[MovieGenres] ([MovieId],[GenreId]) VALUES (2,3);
INSERT INTO [dbo].[MovieGenres] ([MovieId],[GenreId]) VALUES (2,2);
INSERT INTO [dbo].[MovieGenres] ([MovieId],[GenreId]) VALUES (2,1);
INSERT INTO [dbo].[MovieGenres] ([MovieId],[GenreId]) VALUES (3,6);
INSERT INTO [dbo].[MovieGenres] ([MovieId],[GenreId]) VALUES (3,2);
INSERT INTO [dbo].[MovieGenres] ([MovieId],[GenreId]) VALUES (4,1);
INSERT INTO [dbo].[MovieGenres] ([MovieId],[GenreId]) VALUES (4,5);
INSERT INTO [dbo].[MovieGenres] ([MovieId],[GenreId]) VALUES (4,4);
INSERT INTO [dbo].[MovieGenres] ([MovieId],[GenreId]) VALUES (4,10);
INSERT INTO [dbo].[MovieGenres] ([MovieId],[GenreId]) VALUES (5,1);
INSERT INTO [dbo].[MovieGenres] ([MovieId],[GenreId]) VALUES (5,2);
INSERT INTO [dbo].[MovieGenres] ([MovieId],[GenreId]) VALUES (5,3);
INSERT INTO [dbo].[MovieGenres] ([MovieId],[GenreId]) VALUES (5,9);
INSERT INTO [dbo].[MovieGenres] ([MovieId],[GenreId]) VALUES (6,2);
INSERT INTO [dbo].[MovieGenres] ([MovieId],[GenreId]) VALUES (6,3);
INSERT INTO [dbo].[MovieGenres] ([MovieId],[GenreId]) VALUES (6,10);
INSERT INTO [dbo].[MovieGenres] ([MovieId],[GenreId]) VALUES (7,5);
INSERT INTO [dbo].[MovieGenres] ([MovieId],[GenreId]) VALUES (7,8);
INSERT INTO [dbo].[MovieGenres] ([MovieId],[GenreId]) VALUES (8,1);
INSERT INTO [dbo].[MovieGenres] ([MovieId],[GenreId]) VALUES (8,3);
INSERT INTO [dbo].[MovieGenres] ([MovieId],[GenreId]) VALUES (9,1);
INSERT INTO [dbo].[MovieGenres] ([MovieId],[GenreId]) VALUES (9,2);
INSERT INTO [dbo].[MovieGenres] ([MovieId],[GenreId]) VALUES (9,5);
INSERT INTO [dbo].[MovieGenres] ([MovieId],[GenreId]) VALUES (10,2);
INSERT INTO [dbo].[MovieGenres] ([MovieId],[GenreId]) VALUES (10,6);
INSERT INTO [dbo].[MovieGenres] ([MovieId],[GenreId]) VALUES (10,1);
INSERT INTO [dbo].[MovieGenres] ([MovieId],[GenreId]) VALUES (11,3);
INSERT INTO [dbo].[MovieGenres] ([MovieId],[GenreId]) VALUES (11,5);
INSERT INTO [dbo].[MovieGenres] ([MovieId],[GenreId]) VALUES (11,2);
INSERT INTO [dbo].[MovieGenres] ([MovieId],[GenreId]) VALUES (12,3);
INSERT INTO [dbo].[MovieGenres] ([MovieId],[GenreId]) VALUES (12,1);
INSERT INTO [dbo].[MovieGenres] ([MovieId],[GenreId]) VALUES (12,10);
INSERT INTO [dbo].[MovieGenres] ([MovieId],[GenreId]) VALUES (12,7);

SET IDENTITY_INSERT [dbo].[MovieCredits] ON;
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (1,1,1,1,NULL,0);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (2,1,2,2,N'Frodo Baggins',1);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (3,1,3,2,N'Gandalf',2);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (4,1,4,2,N'Aragorn',3);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (5,2,5,1,NULL,0);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (6,2,6,2,N'Luke Skywalker',1);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (7,2,7,2,N'Han Solo',2);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (8,2,8,2,N'Princess Leia Organa',3);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (9,3,9,1,NULL,0);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (10,3,10,2,N'Harry Potter',1);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (11,3,11,2,N'Ron Weasley',2);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (12,3,12,2,N'Hermione Granger',3);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (13,4,13,1,NULL,0);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (14,4,14,2,N'Bruce Wayne / Batman',1);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (15,4,15,2,N'The Joker',2);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (16,4,16,2,N'Harvey Dent / Two-Face',3);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (17,5,17,1,NULL,0);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (18,5,18,1,NULL,1);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (19,5,19,2,N'Tony Stark / Iron Man',1);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (20,5,20,2,N'Steve Rogers / Captain America',2);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (21,5,21,2,N'Natasha Romanoff / Black Widow',3);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (22,6,22,1,NULL,0);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (23,6,23,2,N'Dr. Alan Grant',1);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (24,6,24,2,N'Dr. Ellie Sattler',2);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (25,6,25,2,N'Dr. Ian Malcolm',3);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (26,7,26,1,NULL,0);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (27,7,27,2,N'Jack Dawson',1);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (28,7,28,2,N'Rose DeWitt Bukater',2);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (29,7,29,2,N'Caledon Hockley',3);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (30,8,30,1,NULL,0);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (31,8,31,1,NULL,1);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (32,8,32,2,N'Neo',1);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (33,8,33,2,N'Morpheus',2);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (34,8,34,2,N'Trinity',3);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (35,9,35,1,NULL,0);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (36,9,36,2,N'Maximus Decimus Meridius',1);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (37,9,37,2,N'Commodus',2);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (38,9,38,2,N'Lucilla',3);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (39,10,39,1,NULL,0);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (40,10,40,2,N'Captain Jack Sparrow',1);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (41,10,41,2,N'Captain Hector Barbossa',2);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (42,10,42,2,N'Will Turner',3);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (43,10,43,2,N'Elizabeth Swann',4);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (44,11,13,1,NULL,0);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (45,11,44,2,N'Joseph Cooper',1);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (46,11,45,2,N'Dr. Amelia Brand',2);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (47,11,46,2,N'Murphy Cooper',3);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (48,12,13,1,NULL,0);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (49,12,27,2,N'Dom Cobb',1);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (50,12,47,2,N'Arthur',2);
INSERT INTO [dbo].[MovieCredits] ([Id],[MovieId],[PersonId],[CreditType],[CharacterName],[BillingOrder]) VALUES (51,12,48,2,N'Ariadne',3);
SET IDENTITY_INSERT [dbo].[MovieCredits] OFF;

SET IDENTITY_INSERT [dbo].[Reviews] ON;
INSERT INTO [dbo].[Reviews] ([Id],[UserId],[MovieId],[Score],[Comment],[CreatedAt],[UpdatedAt]) VALUES (1,'11111111-1111-1111-1111-111111111111',1,10,N'Una aventura épica con un mundo inolvidable.',SYSUTCDATETIME(),SYSUTCDATETIME());
INSERT INTO [dbo].[Reviews] ([Id],[UserId],[MovieId],[Score],[Comment],[CreatedAt],[UpdatedAt]) VALUES (2,'22222222-2222-2222-2222-222222222222',2,10,N'Un clásico de ciencia ficción que cambió el cine popular.',SYSUTCDATETIME(),SYSUTCDATETIME());
INSERT INTO [dbo].[Reviews] ([Id],[UserId],[MovieId],[Score],[Comment],[CreatedAt],[UpdatedAt]) VALUES (3,'11111111-1111-1111-1111-111111111111',3,9,N'Una introducción mágica y muy entretenida.',SYSUTCDATETIME(),SYSUTCDATETIME());
INSERT INTO [dbo].[Reviews] ([Id],[UserId],[MovieId],[Score],[Comment],[CreatedAt],[UpdatedAt]) VALUES (4,'22222222-2222-2222-2222-222222222222',4,10,N'Un drama de superhéroes intenso y memorable.',SYSUTCDATETIME(),SYSUTCDATETIME());
INSERT INTO [dbo].[Reviews] ([Id],[UserId],[MovieId],[Score],[Comment],[CreatedAt],[UpdatedAt]) VALUES (5,'11111111-1111-1111-1111-111111111111',5,9,N'Un cierre emocional para una gran etapa del universo Marvel.',SYSUTCDATETIME(),SYSUTCDATETIME());
INSERT INTO [dbo].[Reviews] ([Id],[UserId],[MovieId],[Score],[Comment],[CreatedAt],[UpdatedAt]) VALUES (6,'22222222-2222-2222-2222-222222222222',6,10,N'Aventura, tensión y efectos visuales que siguen impresionando.',SYSUTCDATETIME(),SYSUTCDATETIME());
INSERT INTO [dbo].[Reviews] ([Id],[UserId],[MovieId],[Score],[Comment],[CreatedAt],[UpdatedAt]) VALUES (7,'11111111-1111-1111-1111-111111111111',7,9,N'Una historia romántica y trágica de enorme escala.',SYSUTCDATETIME(),SYSUTCDATETIME());
INSERT INTO [dbo].[Reviews] ([Id],[UserId],[MovieId],[Score],[Comment],[CreatedAt],[UpdatedAt]) VALUES (8,'22222222-2222-2222-2222-222222222222',8,10,N'Una propuesta de ciencia ficción que invita a cuestionar la realidad.',SYSUTCDATETIME(),SYSUTCDATETIME());
INSERT INTO [dbo].[Reviews] ([Id],[UserId],[MovieId],[Score],[Comment],[CreatedAt],[UpdatedAt]) VALUES (9,'11111111-1111-1111-1111-111111111111',9,9,N'Acción histórica con una interpretación central poderosa.',SYSUTCDATETIME(),SYSUTCDATETIME());
INSERT INTO [dbo].[Reviews] ([Id],[UserId],[MovieId],[Score],[Comment],[CreatedAt],[UpdatedAt]) VALUES (10,'22222222-2222-2222-2222-222222222222',10,9,N'Una aventura de piratas divertida y llena de personalidad.',SYSUTCDATETIME(),SYSUTCDATETIME());
INSERT INTO [dbo].[Reviews] ([Id],[UserId],[MovieId],[Score],[Comment],[CreatedAt],[UpdatedAt]) VALUES (11,'11111111-1111-1111-1111-111111111111',11,10,N'Ciencia ficción emotiva con grandes ideas visuales.',SYSUTCDATETIME(),SYSUTCDATETIME());
INSERT INTO [dbo].[Reviews] ([Id],[UserId],[MovieId],[Score],[Comment],[CreatedAt],[UpdatedAt]) VALUES (12,'22222222-2222-2222-2222-222222222222',12,10,N'Un thriller complejo que recompensa la atención.',SYSUTCDATETIME(),SYSUTCDATETIME());
SET IDENTITY_INSERT [dbo].[Reviews] OFF;

SET IDENTITY_INSERT [dbo].[WatchLists] ON;
INSERT INTO [dbo].[WatchLists] ([Id],[UserId],[Name],[Description],[CreatedAt],[UpdatedAt]) VALUES (1,'11111111-1111-1111-1111-111111111111',N'Épicos favoritos',N'Grandes aventuras y dramas que volvería a ver.',SYSUTCDATETIME(),SYSUTCDATETIME());
INSERT INTO [dbo].[WatchLists] ([Id],[UserId],[Name],[Description],[CreatedAt],[UpdatedAt]) VALUES (2,'11111111-1111-1111-1111-111111111111',N'Maratón de ciencia ficción',N'Películas seleccionadas para una maratón de fin de semana.',SYSUTCDATETIME(),SYSUTCDATETIME());
SET IDENTITY_INSERT [dbo].[WatchLists] OFF;
INSERT INTO [dbo].[WatchListMovies] ([WatchListId],[MovieId],[AddedAt]) VALUES (1,1,SYSUTCDATETIME());
INSERT INTO [dbo].[WatchListMovies] ([WatchListId],[MovieId],[AddedAt]) VALUES (1,4,SYSUTCDATETIME());
INSERT INTO [dbo].[WatchListMovies] ([WatchListId],[MovieId],[AddedAt]) VALUES (1,7,SYSUTCDATETIME());
INSERT INTO [dbo].[WatchListMovies] ([WatchListId],[MovieId],[AddedAt]) VALUES (1,11,SYSUTCDATETIME());
INSERT INTO [dbo].[WatchListMovies] ([WatchListId],[MovieId],[AddedAt]) VALUES (2,2,SYSUTCDATETIME());
INSERT INTO [dbo].[WatchListMovies] ([WatchListId],[MovieId],[AddedAt]) VALUES (2,5,SYSUTCDATETIME());
INSERT INTO [dbo].[WatchListMovies] ([WatchListId],[MovieId],[AddedAt]) VALUES (2,8,SYSUTCDATETIME());
INSERT INTO [dbo].[WatchListMovies] ([WatchListId],[MovieId],[AddedAt]) VALUES (2,12,SYSUTCDATETIME());

COMMIT TRANSACTION;
GO

PRINT N'CineStreamCR creada correctamente con 12 películas reales.';
SELECT N'Users' AS Tabla, COUNT(*) AS Registros FROM dbo.Users
UNION ALL SELECT N'Movies', COUNT(*) FROM dbo.Movies
UNION ALL SELECT N'Genres', COUNT(*) FROM dbo.Genres
UNION ALL SELECT N'People', COUNT(*) FROM dbo.People
UNION ALL SELECT N'MovieCredits', COUNT(*) FROM dbo.MovieCredits
UNION ALL SELECT N'Reviews', COUNT(*) FROM dbo.Reviews
UNION ALL SELECT N'WatchLists', COUNT(*) FROM dbo.WatchLists;
GO
