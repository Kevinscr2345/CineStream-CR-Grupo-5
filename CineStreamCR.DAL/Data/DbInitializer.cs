using CineStreamCR.DAL.Entidades;
using CineStreamCR.DAL.Seguridad;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CineStreamCR.DAL.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services, bool seedData = true)
    {
        await using var scope = services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            await db.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "No fue posible conectar o crear la base CineStreamCR en SQL Server. Revise DefaultConnection y que SQL Server esté iniciado.", ex);
        }

        if (!seedData || await db.Movies.AnyAsync()) return;

        await using var transaction = await db.Database.BeginTransactionAsync();
        try
        {
            var demoUser = new User
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Email = "demo@cinestream.cr",
                UserName = "demo",
                DisplayName = "Usuario Demo",
                PasswordHash = PasswordHasher.Hash("Demo123*")
            };
            var secondUser = new User
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Email = "ana@cinestream.cr",
                UserName = "ana",
                DisplayName = "Ana Vargas",
                PasswordHash = PasswordHasher.Hash("Ana123*")
            };

            var genres = new[]
            {
                new Genre { Name = "Acción" },
                new Genre { Name = "Aventura" },
                new Genre { Name = "Ciencia ficción" },
                new Genre { Name = "Crimen" },
                new Genre { Name = "Drama" },
                new Genre { Name = "Fantasía" },
                new Genre { Name = "Misterio" },
                new Genre { Name = "Romance" },
                new Genre { Name = "Superhéroes" },
                new Genre { Name = "Suspenso" }
            };

            var people = new[]
            {
                Person(1, "Peter Jackson", "Neozelandés", 1961, 10, 31,
                    "Cineasta neozelandés conocido por dirigir, escribir y producir las trilogías cinematográficas de The Lord of the Rings y The Hobbit.",
                    "https://en.wikipedia.org/wiki/Peter_Jackson"),
                Person(2, "Elijah Wood", "Estadounidense", 1981, 1, 28,
                    "Actor estadounidense reconocido internacionalmente por interpretar a Frodo Baggins en la trilogía cinematográfica de The Lord of the Rings.",
                    "https://en.wikipedia.org/wiki/Elijah_Wood"),
                Person(3, "Ian McKellen", "Británico", 1939, 5, 25,
                    "Actor británico de teatro y cine, ampliamente reconocido por sus interpretaciones de Gandalf y Magneto y por una extensa carrera shakespeariana.",
                    "https://en.wikipedia.org/wiki/Ian_McKellen"),
                Person(4, "Viggo Mortensen", "Estadounidense-danés", 1958, 10, 20,
                    "Actor, escritor y artista estadounidense-danés conocido por interpretar a Aragorn y por su trabajo en cine independiente y dramático.",
                    "https://en.wikipedia.org/wiki/Viggo_Mortensen"),
                Person(5, "George Lucas", "Estadounidense", 1944, 5, 14,
                    "Cineasta estadounidense creador de Star Wars e Indiana Jones y fundador de Lucasfilm e Industrial Light & Magic.",
                    "https://en.wikipedia.org/wiki/George_Lucas"),
                Person(6, "Mark Hamill", "Estadounidense", 1951, 9, 25,
                    "Actor estadounidense conocido por interpretar a Luke Skywalker y por una destacada carrera como actor de voz.",
                    "https://en.wikipedia.org/wiki/Mark_Hamill"),
                Person(7, "Harrison Ford", "Estadounidense", 1942, 7, 13,
                    "Actor estadounidense célebre por sus papeles como Han Solo, Indiana Jones y Rick Deckard.",
                    "https://en.wikipedia.org/wiki/Harrison_Ford"),
                Person(8, "Carrie Fisher", "Estadounidense", 1956, 10, 21,
                    "Actriz y escritora estadounidense recordada por interpretar a la princesa Leia en Star Wars.",
                    "https://en.wikipedia.org/wiki/Carrie_Fisher"),
                Person(9, "Chris Columbus", "Estadounidense", 1958, 9, 10,
                    "Director, productor y guionista estadounidense que dirigió las dos primeras películas de Harry Potter y varias comedias familiares.",
                    "https://en.wikipedia.org/wiki/Chris_Columbus_(filmmaker)"),
                Person(10, "Daniel Radcliffe", "Británico", 1989, 7, 23,
                    "Actor británico conocido por interpretar a Harry Potter en la serie cinematográfica basada en las novelas de J. K. Rowling.",
                    "https://en.wikipedia.org/wiki/Daniel_Radcliffe"),
                Person(11, "Rupert Grint", "Británico", 1988, 8, 24,
                    "Actor británico reconocido por su papel de Ron Weasley en las películas de Harry Potter.",
                    "https://en.wikipedia.org/wiki/Rupert_Grint"),
                Person(12, "Emma Watson", "Británica", 1990, 4, 15,
                    "Actriz británica conocida por interpretar a Hermione Granger y por su trabajo posterior en cine y causas educativas.",
                    "https://en.wikipedia.org/wiki/Emma_Watson"),
                Person(13, "Christopher Nolan", "Británico-estadounidense", 1970, 7, 30,
                    "Cineasta británico-estadounidense conocido por películas de gran escala con estructuras narrativas complejas, entre ellas The Dark Knight, Inception e Interstellar.",
                    "https://en.wikipedia.org/wiki/Christopher_Nolan"),
                Person(14, "Christian Bale", "Británico", 1974, 1, 30,
                    "Actor británico ganador del Óscar, reconocido por sus transformaciones físicas y por interpretar a Bruce Wayne en la trilogía The Dark Knight.",
                    "https://en.wikipedia.org/wiki/Christian_Bale"),
                Person(15, "Heath Ledger", "Australiano", 1979, 4, 4,
                    "Actor australiano ganador póstumo del Óscar por su interpretación del Joker en The Dark Knight.",
                    "https://en.wikipedia.org/wiki/Heath_Ledger"),
                Person(16, "Aaron Eckhart", "Estadounidense", 1968, 3, 12,
                    "Actor estadounidense conocido por sus papeles en Thank You for Smoking y como Harvey Dent en The Dark Knight.",
                    "https://en.wikipedia.org/wiki/Aaron_Eckhart"),
                Person(17, "Anthony Russo", "Estadounidense", 1970, 2, 3,
                    "Director y productor estadounidense que, junto con su hermano Joe Russo, dirigió varias películas del Universo Cinematográfico de Marvel.",
                    "https://en.wikipedia.org/wiki/Russo_brothers"),
                Person(18, "Joe Russo", "Estadounidense", 1971, 7, 18,
                    "Director y productor estadounidense que forma con Anthony Russo el dúo responsable de Avengers: Infinity War y Avengers: Endgame.",
                    "https://en.wikipedia.org/wiki/Russo_brothers"),
                Person(19, "Robert Downey Jr.", "Estadounidense", 1965, 4, 4,
                    "Actor estadounidense ganador del Óscar, conocido por interpretar a Tony Stark en el Universo Cinematográfico de Marvel.",
                    "https://en.wikipedia.org/wiki/Robert_Downey_Jr."),
                Person(20, "Chris Evans", "Estadounidense", 1981, 6, 13,
                    "Actor estadounidense conocido por interpretar a Steve Rogers, Captain America, en el Universo Cinematográfico de Marvel.",
                    "https://en.wikipedia.org/wiki/Chris_Evans_(actor)"),
                Person(21, "Scarlett Johansson", "Estadounidense", 1984, 11, 22,
                    "Actriz estadounidense reconocida por una amplia carrera dramática y por interpretar a Natasha Romanoff en Marvel.",
                    "https://en.wikipedia.org/wiki/Scarlett_Johansson"),
                Person(22, "Steven Spielberg", "Estadounidense", 1946, 12, 18,
                    "Director y productor estadounidense, figura central del cine moderno y responsable de títulos como Jaws, E.T., Jurassic Park y Schindler’s List.",
                    "https://en.wikipedia.org/wiki/Steven_Spielberg"),
                Person(23, "Sam Neill", "Neozelandés", 1947, 9, 14,
                    "Actor neozelandés conocido por interpretar al paleontólogo Alan Grant en la franquicia Jurassic Park.",
                    "https://en.wikipedia.org/wiki/Sam_Neill"),
                Person(24, "Laura Dern", "Estadounidense", 1967, 2, 10,
                    "Actriz estadounidense ganadora del Óscar, conocida por Jurassic Park, Marriage Story y numerosas producciones de cine y televisión.",
                    "https://en.wikipedia.org/wiki/Laura_Dern"),
                Person(25, "Jeff Goldblum", "Estadounidense", 1952, 10, 22,
                    "Actor estadounidense conocido por The Fly, Jurassic Park, Independence Day y su estilo interpretativo distintivo.",
                    "https://en.wikipedia.org/wiki/Jeff_Goldblum"),
                Person(26, "James Cameron", "Canadiense", 1954, 8, 16,
                    "Director, guionista y productor canadiense conocido por Titanic, Avatar, Aliens y Terminator 2.",
                    "https://en.wikipedia.org/wiki/James_Cameron"),
                Person(27, "Leonardo DiCaprio", "Estadounidense", 1974, 11, 11,
                    "Actor y productor estadounidense ganador del Óscar, protagonista de Titanic, Inception y The Revenant.",
                    "https://en.wikipedia.org/wiki/Leonardo_DiCaprio"),
                Person(28, "Kate Winslet", "Británica", 1975, 10, 5,
                    "Actriz británica ganadora del Óscar, reconocida por Titanic, The Reader y una extensa carrera en drama.",
                    "https://en.wikipedia.org/wiki/Kate_Winslet"),
                Person(29, "Billy Zane", "Estadounidense", 1966, 2, 24,
                    "Actor estadounidense conocido por interpretar a Caledon Hockley en Titanic y por papeles en cine y televisión.",
                    "https://en.wikipedia.org/wiki/Billy_Zane"),
                Person(30, "Lana Wachowski", "Estadounidense", 1965, 6, 21,
                    "Cineasta estadounidense que, junto con Lilly Wachowski, creó y dirigió The Matrix y otras obras de ciencia ficción.",
                    "https://en.wikipedia.org/wiki/The_Wachowskis"),
                Person(31, "Lilly Wachowski", "Estadounidense", 1967, 12, 29,
                    "Cineasta estadounidense conocida por su colaboración con Lana Wachowski en The Matrix, Cloud Atlas y Sense8.",
                    "https://en.wikipedia.org/wiki/The_Wachowskis"),
                Person(32, "Keanu Reeves", "Canadiense", 1964, 9, 2,
                    "Actor canadiense conocido por The Matrix, John Wick, Speed y una amplia carrera en cine de acción y ciencia ficción.",
                    "https://en.wikipedia.org/wiki/Keanu_Reeves"),
                Person(33, "Laurence Fishburne", "Estadounidense", 1961, 7, 30,
                    "Actor estadounidense reconocido por interpretar a Morpheus en The Matrix y por una extensa trayectoria en teatro, cine y televisión.",
                    "https://en.wikipedia.org/wiki/Laurence_Fishburne"),
                Person(34, "Carrie-Anne Moss", "Canadiense", 1967, 8, 21,
                    "Actriz canadiense conocida principalmente por interpretar a Trinity en la franquicia The Matrix.",
                    "https://en.wikipedia.org/wiki/Carrie-Anne_Moss"),
                Person(35, "Ridley Scott", "Británico", 1937, 11, 30,
                    "Director y productor británico conocido por Alien, Blade Runner, Gladiator y The Martian.",
                    "https://en.wikipedia.org/wiki/Ridley_Scott"),
                Person(36, "Russell Crowe", "Neozelandés-australiano", 1964, 4, 7,
                    "Actor neozelandés-australiano ganador del Óscar por su interpretación de Máximo en Gladiator.",
                    "https://en.wikipedia.org/wiki/Russell_Crowe"),
                Person(37, "Joaquin Phoenix", "Estadounidense", 1974, 10, 28,
                    "Actor estadounidense ganador del Óscar, reconocido por Joker, Gladiator, Walk the Line y The Master.",
                    "https://en.wikipedia.org/wiki/Joaquin_Phoenix"),
                Person(38, "Connie Nielsen", "Danesa", 1965, 7, 3,
                    "Actriz danesa conocida por interpretar a Lucilla en Gladiator y por sus trabajos en cine europeo y estadounidense.",
                    "https://en.wikipedia.org/wiki/Connie_Nielsen"),
                Person(39, "Gore Verbinski", "Estadounidense", 1964, 3, 16,
                    "Director estadounidense conocido por The Ring, Rango y las primeras películas de Pirates of the Caribbean.",
                    "https://en.wikipedia.org/wiki/Gore_Verbinski"),
                Person(40, "Johnny Depp", "Estadounidense", 1963, 6, 9,
                    "Actor estadounidense conocido por interpretar al capitán Jack Sparrow y por sus colaboraciones con diversos directores.",
                    "https://en.wikipedia.org/wiki/Johnny_Depp"),
                Person(41, "Geoffrey Rush", "Australiano", 1951, 7, 6,
                    "Actor australiano ganador del Óscar, conocido por Shine, The King’s Speech y su papel de Barbossa.",
                    "https://en.wikipedia.org/wiki/Geoffrey_Rush"),
                Person(42, "Orlando Bloom", "Británico", 1977, 1, 13,
                    "Actor británico conocido por interpretar a Legolas en The Lord of the Rings y a Will Turner en Pirates of the Caribbean.",
                    "https://en.wikipedia.org/wiki/Orlando_Bloom"),
                Person(43, "Keira Knightley", "Británica", 1985, 3, 26,
                    "Actriz británica conocida por Pride & Prejudice, Atonement y su papel de Elizabeth Swann.",
                    "https://en.wikipedia.org/wiki/Keira_Knightley"),
                Person(44, "Matthew McConaughey", "Estadounidense", 1969, 11, 4,
                    "Actor estadounidense ganador del Óscar por Dallas Buyers Club y protagonista de Interstellar y True Detective.",
                    "https://en.wikipedia.org/wiki/Matthew_McConaughey"),
                Person(45, "Anne Hathaway", "Estadounidense", 1982, 11, 12,
                    "Actriz estadounidense ganadora del Óscar, conocida por Les Misérables, The Devil Wears Prada e Interstellar.",
                    "https://en.wikipedia.org/wiki/Anne_Hathaway"),
                Person(46, "Jessica Chastain", "Estadounidense", 1977, 3, 24,
                    "Actriz y productora estadounidense ganadora del Óscar, conocida por Zero Dark Thirty, Interstellar y The Eyes of Tammy Faye.",
                    "https://en.wikipedia.org/wiki/Jessica_Chastain"),
                Person(47, "Joseph Gordon-Levitt", "Estadounidense", 1981, 2, 17,
                    "Actor y cineasta estadounidense conocido por Inception, 500 Days of Summer y Looper.",
                    "https://en.wikipedia.org/wiki/Joseph_Gordon-Levitt"),
                Person(48, "Elliot Page", "Canadiense", 1987, 2, 21,
                    "Actor canadiense conocido por Juno, Inception, Hard Candy y la serie The Umbrella Academy.",
                    "https://en.wikipedia.org/wiki/Elliot_Page"),
            };

            var movies = new[]
            {
                Movie(1, "The Lord of the Rings: The Fellowship of the Ring", "En la Tierra Media, el joven hobbit Frodo Bolsón hereda un anillo de poder que debe ser destruido antes de que el señor oscuro Sauron lo recupere. Acompañado por una comunidad de héroes, inicia un viaje peligroso hacia Mordor.", 2001, 178, true, "https://en.wikipedia.org/wiki/The_Lord_of_the_Rings:_The_Fellowship_of_the_Ring"),
                Movie(2, "Star Wars: Episode IV – A New Hope", "Luke Skywalker abandona su vida en Tatooine para unirse a la Alianza Rebelde. Junto a Han Solo y la princesa Leia, intenta destruir la Estrella de la Muerte y enfrentarse al Imperio Galáctico.", 1977, 121, false, "https://en.wikipedia.org/wiki/Star_Wars_(film)"),
                Movie(3, "Harry Potter and the Philosopher's Stone", "Harry Potter descubre que es un mago y comienza sus estudios en Hogwarts. Allí forma amistad con Ron y Hermione, conoce el mundo mágico y se enfrenta a un misterio relacionado con la Piedra Filosofal.", 2001, 152, false, "https://en.wikipedia.org/wiki/Harry_Potter_and_the_Philosopher%27s_Stone_(film)"),
                Movie(4, "The Dark Knight", "Batman, el fiscal Harvey Dent y el comisionado Gordon intentan detener al crimen organizado de Gotham. Su alianza es puesta a prueba cuando el Joker inicia una campaña de caos que obliga al héroe a enfrentar límites morales.", 2008, 152, false, "https://en.wikipedia.org/wiki/The_Dark_Knight"),
                Movie(5, "Avengers: Endgame", "Después de la devastación causada por Thanos, los Vengadores supervivientes buscan una última oportunidad para revertir la pérdida y restaurar el universo, aunque la misión exige sacrificios decisivos.", 2019, 181, false, "https://en.wikipedia.org/wiki/Avengers:_Endgame"),
                Movie(6, "Jurassic Park", "Un grupo de especialistas visita un parque temático en Isla Nublar, cerca de Costa Rica, donde dinosaurios clonados han vuelto a la vida. Una falla de seguridad convierte la visita en una lucha por sobrevivir.", 1993, 127, false, "https://en.wikipedia.org/wiki/Jurassic_Park_(film)"),
                Movie(7, "Titanic", "A bordo del RMS Titanic, Jack Dawson y Rose DeWitt Bukater, jóvenes de clases sociales distintas, se enamoran durante el viaje inaugural del barco, cuya colisión con un iceberg cambia sus vidas para siempre.", 1997, 195, false, "https://en.wikipedia.org/wiki/Titanic_(1997_film)"),
                Movie(8, "The Matrix", "El programador Thomas Anderson, conocido como Neo, descubre que la realidad que conoce es una simulación creada por máquinas. Morfeo y Trinity lo introducen en una rebelión que cuestiona la libertad y la identidad.", 1999, 136, false, "https://en.wikipedia.org/wiki/The_Matrix"),
                Movie(9, "Gladiator", "El general romano Máximo es traicionado por Cómodo y reducido a la esclavitud. Convertido en gladiador, busca justicia mientras asciende en la arena y desafía al nuevo emperador de Roma.", 2000, 155, false, "https://en.wikipedia.org/wiki/Gladiator_(2000_film)"),
                Movie(10, "Pirates of the Caribbean: The Curse of the Black Pearl", "El herrero Will Turner se une al impredecible capitán Jack Sparrow para rescatar a Elizabeth Swann de una tripulación pirata condenada por una antigua maldición azteca.", 2003, 143, false, "https://en.wikipedia.org/wiki/Pirates_of_the_Caribbean:_The_Curse_of_the_Black_Pearl"),
                Movie(11, "Interstellar", "Con la Tierra al borde del colapso, el expiloto Cooper se une a una misión espacial que atraviesa un agujero de gusano en busca de un nuevo hogar para la humanidad, mientras lucha contra el tiempo y la distancia.", 2014, 169, false, "https://en.wikipedia.org/wiki/Interstellar_(film)"),
                Movie(12, "Inception", "Dom Cobb dirige un equipo capaz de entrar en los sueños para robar secretos. Para recuperar su vida, acepta la misión inversa: implantar una idea en la mente de un heredero empresarial.", 2010, 148, false, "https://en.wikipedia.org/wiki/Inception"),
            };

            db.Users.AddRange(demoUser, secondUser);
            db.Genres.AddRange(genres);
            db.People.AddRange(people);
            db.Movies.AddRange(movies);
            await db.SaveChangesAsync();

            var movieByTitle = movies.ToDictionary(x => x.Title, StringComparer.OrdinalIgnoreCase);
            var genreByName = genres.ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
            var personByName = people.ToDictionary(x => x.FullName, StringComparer.OrdinalIgnoreCase);

            void AddGenres(string movieTitle, params string[] genreNames)
            {
                foreach (var genreName in genreNames)
                    db.MovieGenres.Add(new MovieGenre { MovieId = movieByTitle[movieTitle].Id, GenreId = genreByName[genreName].Id });
            }

            AddGenres("The Lord of the Rings: The Fellowship of the Ring", "Aventura", "Fantasía", "Drama");
            AddGenres("Star Wars: Episode IV – A New Hope", "Ciencia ficción", "Aventura", "Acción");
            AddGenres("Harry Potter and the Philosopher's Stone", "Fantasía", "Aventura");
            AddGenres("The Dark Knight", "Acción", "Drama", "Crimen", "Suspenso");
            AddGenres("Avengers: Endgame", "Acción", "Aventura", "Ciencia ficción", "Superhéroes");
            AddGenres("Jurassic Park", "Aventura", "Ciencia ficción", "Suspenso");
            AddGenres("Titanic", "Drama", "Romance");
            AddGenres("The Matrix", "Acción", "Ciencia ficción");
            AddGenres("Gladiator", "Acción", "Aventura", "Drama");
            AddGenres("Pirates of the Caribbean: The Curse of the Black Pearl", "Aventura", "Fantasía", "Acción");
            AddGenres("Interstellar", "Ciencia ficción", "Drama", "Aventura");
            AddGenres("Inception", "Ciencia ficción", "Acción", "Suspenso", "Misterio");

            void Director(string movieTitle, string personName, int order = 0) => db.MovieCredits.Add(new MovieCredit
            { MovieId = movieByTitle[movieTitle].Id, PersonId = personByName[personName].Id, CreditType = CreditType.Director, BillingOrder = order });
            void Actor(string movieTitle, string personName, string character, int order) => db.MovieCredits.Add(new MovieCredit
            { MovieId = movieByTitle[movieTitle].Id, PersonId = personByName[personName].Id, CreditType = CreditType.Actor, CharacterName = character, BillingOrder = order });

            Director("The Lord of the Rings: The Fellowship of the Ring", "Peter Jackson", 0);
            Actor("The Lord of the Rings: The Fellowship of the Ring", "Elijah Wood", "Frodo Baggins", 1);
            Actor("The Lord of the Rings: The Fellowship of the Ring", "Ian McKellen", "Gandalf", 2);
            Actor("The Lord of the Rings: The Fellowship of the Ring", "Viggo Mortensen", "Aragorn", 3);
            Director("Star Wars: Episode IV – A New Hope", "George Lucas", 0);
            Actor("Star Wars: Episode IV – A New Hope", "Mark Hamill", "Luke Skywalker", 1);
            Actor("Star Wars: Episode IV – A New Hope", "Harrison Ford", "Han Solo", 2);
            Actor("Star Wars: Episode IV – A New Hope", "Carrie Fisher", "Princess Leia Organa", 3);
            Director("Harry Potter and the Philosopher's Stone", "Chris Columbus", 0);
            Actor("Harry Potter and the Philosopher's Stone", "Daniel Radcliffe", "Harry Potter", 1);
            Actor("Harry Potter and the Philosopher's Stone", "Rupert Grint", "Ron Weasley", 2);
            Actor("Harry Potter and the Philosopher's Stone", "Emma Watson", "Hermione Granger", 3);
            Director("The Dark Knight", "Christopher Nolan", 0);
            Actor("The Dark Knight", "Christian Bale", "Bruce Wayne / Batman", 1);
            Actor("The Dark Knight", "Heath Ledger", "The Joker", 2);
            Actor("The Dark Knight", "Aaron Eckhart", "Harvey Dent / Two-Face", 3);
            Director("Avengers: Endgame", "Anthony Russo", 0);
            Director("Avengers: Endgame", "Joe Russo", 1);
            Actor("Avengers: Endgame", "Robert Downey Jr.", "Tony Stark / Iron Man", 1);
            Actor("Avengers: Endgame", "Chris Evans", "Steve Rogers / Captain America", 2);
            Actor("Avengers: Endgame", "Scarlett Johansson", "Natasha Romanoff / Black Widow", 3);
            Director("Jurassic Park", "Steven Spielberg", 0);
            Actor("Jurassic Park", "Sam Neill", "Dr. Alan Grant", 1);
            Actor("Jurassic Park", "Laura Dern", "Dr. Ellie Sattler", 2);
            Actor("Jurassic Park", "Jeff Goldblum", "Dr. Ian Malcolm", 3);
            Director("Titanic", "James Cameron", 0);
            Actor("Titanic", "Leonardo DiCaprio", "Jack Dawson", 1);
            Actor("Titanic", "Kate Winslet", "Rose DeWitt Bukater", 2);
            Actor("Titanic", "Billy Zane", "Caledon Hockley", 3);
            Director("The Matrix", "Lana Wachowski", 0);
            Director("The Matrix", "Lilly Wachowski", 1);
            Actor("The Matrix", "Keanu Reeves", "Neo", 1);
            Actor("The Matrix", "Laurence Fishburne", "Morpheus", 2);
            Actor("The Matrix", "Carrie-Anne Moss", "Trinity", 3);
            Director("Gladiator", "Ridley Scott", 0);
            Actor("Gladiator", "Russell Crowe", "Maximus Decimus Meridius", 1);
            Actor("Gladiator", "Joaquin Phoenix", "Commodus", 2);
            Actor("Gladiator", "Connie Nielsen", "Lucilla", 3);
            Director("Pirates of the Caribbean: The Curse of the Black Pearl", "Gore Verbinski", 0);
            Actor("Pirates of the Caribbean: The Curse of the Black Pearl", "Johnny Depp", "Captain Jack Sparrow", 1);
            Actor("Pirates of the Caribbean: The Curse of the Black Pearl", "Geoffrey Rush", "Captain Hector Barbossa", 2);
            Actor("Pirates of the Caribbean: The Curse of the Black Pearl", "Orlando Bloom", "Will Turner", 3);
            Actor("Pirates of the Caribbean: The Curse of the Black Pearl", "Keira Knightley", "Elizabeth Swann", 4);
            Director("Interstellar", "Christopher Nolan", 0);
            Actor("Interstellar", "Matthew McConaughey", "Joseph Cooper", 1);
            Actor("Interstellar", "Anne Hathaway", "Dr. Amelia Brand", 2);
            Actor("Interstellar", "Jessica Chastain", "Murphy Cooper", 3);
            Director("Inception", "Christopher Nolan", 0);
            Actor("Inception", "Leonardo DiCaprio", "Dom Cobb", 1);
            Actor("Inception", "Joseph Gordon-Levitt", "Arthur", 2);
            Actor("Inception", "Elliot Page", "Ariadne", 3);

            db.Reviews.AddRange(
                Review(demoUser, movieByTitle["The Lord of the Rings: The Fellowship of the Ring"], 10, "Una aventura épica con un mundo inolvidable."),
                Review(secondUser, movieByTitle["Star Wars: Episode IV – A New Hope"], 10, "Un clásico de ciencia ficción que cambió el cine popular."),
                Review(demoUser, movieByTitle["Harry Potter and the Philosopher's Stone"], 9, "Una introducción mágica y muy entretenida."),
                Review(secondUser, movieByTitle["The Dark Knight"], 10, "Un drama de superhéroes intenso y memorable."),
                Review(demoUser, movieByTitle["Avengers: Endgame"], 9, "Un cierre emocional para una gran etapa del universo Marvel."),
                Review(secondUser, movieByTitle["Jurassic Park"], 10, "Aventura, tensión y efectos visuales que siguen impresionando."),
                Review(demoUser, movieByTitle["Titanic"], 9, "Una historia romántica y trágica de enorme escala."),
                Review(secondUser, movieByTitle["The Matrix"], 10, "Una propuesta de ciencia ficción que invita a cuestionar la realidad."),
                Review(demoUser, movieByTitle["Gladiator"], 9, "Acción histórica con una interpretación central poderosa."),
                Review(secondUser, movieByTitle["Pirates of the Caribbean: The Curse of the Black Pearl"], 9, "Una aventura de piratas divertida y llena de personalidad."),
                Review(demoUser, movieByTitle["Interstellar"], 10, "Ciencia ficción emotiva con grandes ideas visuales."),
                Review(secondUser, movieByTitle["Inception"], 10, "Un thriller complejo que recompensa la atención.")
            );

            var favorites = new WatchList { UserId = demoUser.Id, Name = "Épicos favoritos", Description = "Grandes aventuras y dramas que volvería a ver." };
            var weekend = new WatchList { UserId = demoUser.Id, Name = "Maratón de ciencia ficción", Description = "Películas seleccionadas para una maratón de fin de semana." };
            db.WatchLists.AddRange(favorites, weekend);
            await db.SaveChangesAsync();

            db.WatchListMovies.AddRange(
                WatchListMovie(favorites, movieByTitle["The Lord of the Rings: The Fellowship of the Ring"]),
                WatchListMovie(favorites, movieByTitle["The Dark Knight"]),
                WatchListMovie(favorites, movieByTitle["Titanic"]),
                WatchListMovie(favorites, movieByTitle["Interstellar"]),
                WatchListMovie(weekend, movieByTitle["Star Wars: Episode IV – A New Hope"]),
                WatchListMovie(weekend, movieByTitle["Avengers: Endgame"]),
                WatchListMovie(weekend, movieByTitle["The Matrix"]),
                WatchListMovie(weekend, movieByTitle["Inception"])
            );

            await db.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private static Person Person(int imageIndex, string name, string nationality, int year, int month, int day,
        string biography, string informationSourceUrl)
    {
        var wikiTitle = new Uri(informationSourceUrl).AbsolutePath.Split("/wiki/", StringSplitOptions.RemoveEmptyEntries).Last();
        var fallback = Uri.EscapeDataString($"/images/people/person-{imageIndex}.jpg");
        return new Person
        {
            FullName = name,
            Nationality = nationality,
            BirthDate = new DateOnly(year, month, day),
            Biography = biography,
            InformationSourceUrl = informationSourceUrl,
            ImageSourceUrl = informationSourceUrl,
            PhotoUrl = $"/api/media/wiki-thumbnail?title={Uri.EscapeDataString(Uri.UnescapeDataString(wikiTitle))}&fallback={fallback}"
        };
    }

    private static Movie Movie(int imageIndex, string title, string synopsis, int year, int duration, bool featured, string sourceUrl)
    {
        var wikiTitle = new Uri(sourceUrl).AbsolutePath.Split("/wiki/", StringSplitOptions.RemoveEmptyEntries).Last();
        var fallback = Uri.EscapeDataString($"/images/posters/movie-{imageIndex}.jpg");
        return new Movie
        {
            Title = title,
            Synopsis = synopsis,
            ReleaseYear = year,
            DurationMinutes = duration,
            IsFeatured = featured,
            PosterUrl = $"/api/media/wiki-thumbnail?title={Uri.EscapeDataString(Uri.UnescapeDataString(wikiTitle))}&fallback={fallback}",
            BackdropUrl = $"/images/backdrops/movie-{imageIndex}.jpg",
            VideoUrl = $"/videos/video{imageIndex:00}.mp4",
            InformationSourceUrl = sourceUrl,
            ImageSourceUrl = sourceUrl
        };
    }

    private static Review Review(User user, Movie movie, int score, string comment) => new()
    { UserId = user.Id, MovieId = movie.Id, Score = score, Comment = comment };

    private static WatchListMovie WatchListMovie(WatchList list, Movie movie) => new()
    { WatchListId = list.Id, MovieId = movie.Id };
}
