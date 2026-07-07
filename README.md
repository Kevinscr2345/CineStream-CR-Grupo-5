# CineStream CR — ASP.NET Core MVC + SQL Server

Proyecto académico desarrollado con una estructura similar al proyecto de referencia del curso:

- `CineStreamCR`: interfaz ASP.NET Core MVC, controladores, vistas, JavaScript y CSS.
- `CineStreamCR.BLL`: DTO, servicios, validaciones y reglas de negocio.
- `CineStreamCR.DAL`: entidades, Entity Framework Core, repositorio y datos iniciales.
- SQL Server / SQL Server Management Studio.

## Catálogo real incluido

El proyecto carga 12 películas reales y reconocidas:

1. The Lord of the Rings: The Fellowship of the Ring (2001)
2. Star Wars: Episode IV – A New Hope (1977)
3. Harry Potter and the Philosopher's Stone (2001)
4. The Dark Knight (2008)
5. Avengers: Endgame (2019)
6. Jurassic Park (1993)
7. Titanic (1997)
8. The Matrix (1999)
9. Gladiator (2000)
10. Pirates of the Caribbean: The Curse of the Black Pearl (2003)
11. Interstellar (2014)
12. Inception (2010)

Cada registro incluye año, duración, sinopsis, géneros, dirección, elenco principal, personajes y enlace a la fuente de información. Se registran 48 personas reales y 51 créditos de dirección/actuación.

## Imágenes

Cada película intenta cargar su miniatura o póster publicado en Wikipedia/Wikimedia mediante un endpoint con caché. También se incluyen 12 portadas y 12 fondos locales distintos como respaldo sin conexión. Los perfiles utilizan el mismo mecanismo para mostrar fotografías reales y recurren a una tarjeta gráfica local si la fuente externa no responde.

## Videos

Los videos no están incluidos. Copie sus archivos en:

`CineStreamCR/wwwroot/videos/`

con los nombres `video01.mp4` hasta `video12.mp4`. No debe modificar la base ni JavaScript si conserva esos nombres.

## Configurar SQL Server

Abra `CineStreamCR/appsettings.json` y cambie únicamente el nombre de la instancia si no coincide con su equipo:

```json
"DefaultConnection": "Server=NACHO\\SQLEXPRESS;Database=CineStreamCR;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
```

Ejemplos habituales:

- `Server=localhost;...`
- `Server=DESKTOP-XXXX\\SQLEXPRESS;...`
- `Server=(localdb)\\MSSQLLocalDB;...`

## Crear la base con Entity Framework

1. Abra `CineStreamCR.sln` en Visual Studio 2022.
2. Establezca `CineStreamCR` como proyecto de inicio.
3. Revise `appsettings.json`.
4. Ejecute con `Ctrl + F5`.
5. `DbInitializer` ejecuta `EnsureCreatedAsync()` y carga los datos si la tabla `Movies` está vacía.
6. En SSMS, actualice la carpeta **Databases** y abra `CineStreamCR`.

## Crear la base desde SSMS

Abra y ejecute:

`database/CineStreamCR_SQLServer_FullSetup.sql`

Después ejecute `database/Verify_CineStreamCR.sql` para comprobar los registros.

## Credenciales

- Usuario: `demo`
- Contraseña: `Demo123*`

Cuenta secundaria:

- Usuario: `ana`
- Contraseña: `Ana123*`

## Funcionalidades

- Login con correo o usuario y contraseña protegida con PBKDF2.
- Catálogo paginado con búsqueda en tiempo real, filtros y ordenamiento.
- Detalle de películas con fuentes, géneros, varios directores cuando corresponde y elenco real.
- Perfil biográfico y filmografía de actores y directores.
- WatchLists personalizadas.
- Calificación de 1 a 10 y reseña opcional.
- Reproductor persistente, anterior/siguiente, volumen, progreso y mini reproductor.
- Guardado del progreso en SQL Server.
- Diseño oscuro y responsive.

## Nota de compilación

La solución utiliza `.NET 10` y Entity Framework Core `10.0.8`, igual que el proyecto de referencia adjuntado.


## Base de datos: dos formas de uso

El proyecto incluye dos opciones para la base de datos:

1. **Automática:** dejar `Database:AutoCreate` en `true` para que EF Core cree la base si no existe y cargue datos iniciales.
2. **Manual:** ejecutar `database/01_OPCION_MANUAL_SSMS_CrearBaseCompleta.sql` en SSMS y luego poner `Database:AutoCreate` en `false`.

Ver instrucciones completas en `docs/BASE_DE_DATOS_DOBLE_OPCION.md`.
