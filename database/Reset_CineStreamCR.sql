USE [master];
GO
IF DB_ID(N'CineStreamCR') IS NOT NULL
BEGIN
    ALTER DATABASE [CineStreamCR] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [CineStreamCR];
END
GO
PRINT N'Base CineStreamCR eliminada. Al iniciar la aplicación, Entity Framework Core la creará nuevamente.';
GO
