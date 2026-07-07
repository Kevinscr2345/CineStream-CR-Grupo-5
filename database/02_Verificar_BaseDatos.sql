USE [CineStreamCR];
GO
SELECT 'Users' AS Tabla, COUNT(*) AS Registros FROM dbo.Users
UNION ALL SELECT 'Movies', COUNT(*) FROM dbo.Movies
UNION ALL SELECT 'Genres', COUNT(*) FROM dbo.Genres
UNION ALL SELECT 'People', COUNT(*) FROM dbo.People
UNION ALL SELECT 'MovieGenres', COUNT(*) FROM dbo.MovieGenres
UNION ALL SELECT 'MovieCredits', COUNT(*) FROM dbo.MovieCredits
UNION ALL SELECT 'WatchLists', COUNT(*) FROM dbo.WatchLists
UNION ALL SELECT 'WatchListMovies', COUNT(*) FROM dbo.WatchListMovies
UNION ALL SELECT 'Reviews', COUNT(*) FROM dbo.Reviews
UNION ALL SELECT 'PlaybackProgresses', COUNT(*) FROM dbo.PlaybackProgresses;
GO

SELECT m.Id, m.Title, m.ReleaseYear, m.DurationMinutes, m.VideoUrl,
       STRING_AGG(g.Name, ', ') WITHIN GROUP (ORDER BY g.Name) AS Genres
FROM dbo.Movies m
JOIN dbo.MovieGenres mg ON mg.MovieId = m.Id
JOIN dbo.Genres g ON g.Id = mg.GenreId
GROUP BY m.Id, m.Title, m.ReleaseYear, m.DurationMinutes, m.VideoUrl
ORDER BY m.Id;
GO

SELECT m.Title, p.FullName, CASE mc.CreditType WHEN 1 THEN 'Dirección' ELSE 'Actuación' END AS Tipo,
       mc.CharacterName
FROM dbo.MovieCredits mc
JOIN dbo.Movies m ON m.Id = mc.MovieId
JOIN dbo.People p ON p.Id = mc.PersonId
ORDER BY m.Id, mc.CreditType, mc.BillingOrder;
GO
