# CineStream CR — ASP.NET Core MVC + SQL Server
## Integrantes finales del grupo

- Esteban Valverde Fallas
- Ignacio Cascante Navarro
- Kevin Castro Lara
- Kevin Vargas Morales

## Repositorio
https://github.com/Kevinscr2345/CineStream-CR-Grupo-5

## Arquitectura del proyecto
El proyecto utiliza una arquitectura por capas, separando la presentación, la lógica de negocio y el acceso a datos.

### CineStreamCR
**Tipo:** ASP.NET Core MVC
Capa de presentación de la aplicación. Contiene:
- Controllers: controladores MVC que reciben las solicitudes del usuario.
- Views: interfaz HTML/Razor.
- Models: modelos utilizados por la capa MVC.
- wwwroot: archivos estáticos, incluyendo CSS, JavaScript...
- Program.cs: configuración y registro de dependencias de la aplicación.
- appsettings.json: configuración de la aplicación y cadena de conexión.
- 
### CineStreamCR.BLL
**Tipo:** Class Library
Lógica de negocio de la aplicación:

- DTOs para transportar información entre capas.
- Services separados por dominio:
  - Media
  - Movie
  - Person
  - Playback
  - Review
  - User
  - WatchList
- Validaciones y reglas de negocio.
- Mapeo entre entidades y DTOs mediante `MapeoClases.cs`.

La BLL no accede directamente a la base de datos. Utiliza las abstracciones de acceso a datos proporcionadas por la DAL.

### CineStreamCR.DAL
**Tipo:** Class Library

Es la capa de acceso a datos y persistencia. Contiene:

- `Data`: contexto de Entity Framework Core y configuración de persistencia.
- `Entidades`: entidades que representan las tablas y relaciones de la base de datos.
- `Repositorios`: implementación de los repositorios utilizados para acceder a los datos.
- `Seguridad`: componentes de seguridad de usuarios usando hash.

### Flujo general

La comunicación entre las capas sigue el siguiente flujo:

CineStreamCR (MVC) → CineStreamCR.BLL → CineStreamCR.DAL → SQL Server

La capa MVC no accede directamente al contexto de Entity Framework ni a la base de datos. Las solicitudes pasan por los servicios de la BLL, que aplican las reglas de negocio y utilizan los repositorios DAL
  
## Librerías y paquetes NuGet utilizados

La solución utiliza .NET 10 y los siguientes paquetes NuGet:

### CineStreamCR

- `Microsoft.EntityFrameworkCore.Design` — versión `10.0.8`
  - Utilizado para las herramientas de diseño de Entity Framework Core y las operaciones relacionadas con la generación y administración del modelo de datos.

### CineStreamCR.BLL

- `AutoMapper` — versión `16.1.1`
  - Utilizado para realizar el mapeo entre entidades y DTOs.
- `Microsoft.Extensions.Http` — versión `10.0.0-preview.7.25380.108`
  - Utilizado para las funcionalidades relacionadas con `HttpClient` y la integración de servicios HTTP.

### CineStreamCR.DAL

- `Microsoft.EntityFrameworkCore` — versión `10.0.8`
  - ORM utilizado para el acceso y persistencia de datos.
- `Microsoft.EntityFrameworkCore.SqlServer` — versión `10.0.8`
  - Proveedor de Entity Framework Core para SQL Server.
- `Microsoft.EntityFrameworkCore.Design` — versión `10.0.8`
  - Herramientas necesarias para operaciones de diseño de Entity Framework Core.
- `Microsoft.EntityFrameworkCore.Tools` — versión `10.0.8`
  - Herramientas de Entity Framework Core utilizadas durante el desarrollo y administración del modelo de datos.
  - 
## Principios SOLID aplicados
### S — Single Responsibility Principle
Cada clase tiene una responsabilidad específica. Por ejemplo, los servicios de la BLL se encuentran separados por dominio (`Movie`, `Person`, `Review`, `User`, `WatchList`, etc.), evitando concentrar toda la lógica de negocio en una única clase.
### O — Open/Closed Principle
La aplicación utiliza interfaces y servicios separados por responsabilidad, permitiendo extender el comportamiento de determinadas funcionalidades sin modificar innecesariamente las implementaciones existentes.
### L — Liskov Substitution Principle
Las implementaciones concretas de los repositorios y servicios se utilizan mediante sus respectivas abstracciones, permitiendo que una implementación pueda ser sustituida por otra que cumpla el mismo contrato sin modificar el código cliente.
### I — Interface Segregation Principle
Las interfaces se mantienen enfocadas en las operaciones que necesita cada componente. Ejemplo, `ICineRepositorio` define las operaciones relacionadas con el acceso a los datos del dominio de cine.
### D — Dependency Inversion Principle
Las capas superiores dependen de abstracciones en lugar de depender directamente de implementaciones concretas. La aplicación utiliza inyección de dependencias para registrar y proporcionar los servicios y repositorios necesarios.

## Patrones de diseño utilizados
### Repository Pattern
Se utiliza el patrón Repository para encapsular el acceso a los datos.
La interfaz `ICineRepositorio` define las operaciones disponibles y `CineRepositorio` proporciona su implementación. Esto permite que la lógica de negocio no dependa directamente de los detalles de Entity Framework Core.
Ubicación:
`CineStreamCR.DAL/Repositorios/Cine/`
- `ICineRepositorio.cs`
- `CineRepositorio.cs`
### Service Layer Pattern
La lógica de negocio se organiza mediante servicios dentro de `CineStreamCR.BLL`.
Los servicios se encuentran separados por dominio, por ejemplo:
- Movie
- Person
- Review
- User
- WatchList
- Playback
- Media
Esto permite centralizar las reglas de negocio y mantener los controladores MVC enfocados principalmente en atender las solicitudes y coordinar las respuestas.
### Dependency Injection
Se utiliza inyección de dependencias de ASP.NET Core para proporcionar los servicios y repositorios a las clases que los necesitan.
Esto reduce el acoplamiento entre componentes y facilita las pruebas y el mantenimiento del sistema.
### Data Transfer Object (DTO)
Se utilizan DTOs en `CineStreamCR.BLL/Dtos` para transportar información entre las diferentes capas sin exponer directamente las entidades de persistencia a la capa de presentación.

## Decisiones de diseño de base de datos
La base de datos fue diseñada separando las principales entidades del sistema.
Entre las principales decisiones se encuentran:
- Separación de películas, personas, géneros, usuarios, reseñas y listas de reproducción en entidades independientes.
- Uso de relaciones entre películas y personas para representar los créditos de actuación y dirección.
- Uso de tablas intermedias para representar relaciones de muchos a muchos, como películas y géneros.
- Los créditos de las personas se diferencian mediante el tipo de crédito correspondiente, permitiendo representar si una persona participa como actor o director.
- Las WatchLists se relacionan con los usuarios y las películas mediante sus respectivas entidades de relación.
- Las reseñas y calificaciones se relacionan con el usuario y la película correspondiente.
- El progreso de reproducción se almacena para permitir conservar el punto en el que el usuario dejó una película.
- Se utilizan claves primarias y foráneas para mantener la integridad referencial.
- Se utilizan restricciones de unicidad cuando corresponde para evitar registros duplicados.
- Entity Framework Core se utiliza como ORM para realizar el acceso y persistencia de los datos.
- Se contempla una opción de inicialización automática mediante `DbInitializer` y una alternativa de creación manual mediante scripts SQL.
  
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
Los videos no se incluyen dentro del repositorio. El reproductor utiliza URLs para acceder al contenido.

Esta implementación corresponde a un cambio realizado de acuerdo con lo establecido con el profesor.

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
