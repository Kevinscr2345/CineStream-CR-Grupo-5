# CineStreamCR - Base de datos con dos opciones

Este proyecto queda preparado para trabajar de dos maneras:

## Opción 1: Automática desde la aplicación

Esta opción sirve para quien sí quiere que Entity Framework cree la base automáticamente.

1. Abrir `CineStreamCR/appsettings.json`.
2. Dejar esta configuración:

```json
"Database": {
  "AutoCreate": true,
  "SeedData": true
}
```

3. Revisar la cadena de conexión:

```json
"DefaultConnection": "Server=localhost\SQLEXPRESS;Database=CineStreamCR;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
```

4. Ejecutar el proyecto desde Visual Studio.

Con `AutoCreate: true`, el proyecto usa `DbInitializer` y `EnsureCreated()` para crear la base si no existe. También inserta datos iniciales si `SeedData` está en `true` y la tabla `Movies` está vacía.

## Opción 2: Manual desde SQL Server Management Studio

Esta opción sirve para quien no quiere usar creación automática, migraciones ni comandos de Entity Framework.

1. Abrir SQL Server Management Studio.
2. Ejecutar el archivo:

```txt
database/01_OPCION_MANUAL_SSMS_CrearBaseCompleta.sql
```

3. Abrir `CineStreamCR/appsettings.json`.
4. Cambiar la configuración a:

```json
"Database": {
  "AutoCreate": false,
  "SeedData": false
}
```

5. Ejecutar el proyecto desde Visual Studio.

Con `AutoCreate: false`, el sistema no intenta crear la base. Solo se conecta a la base existente creada manualmente en SSMS.

## Usuarios de prueba

Después de usar cualquiera de las dos opciones, se pueden usar estos usuarios:

- Usuario: `demo` / Correo: `demo@cinestream.cr` / Contraseña: `Demo123*`
- Usuario: `ana` / Correo: `ana@cinestream.cr` / Contraseña: `Ana123*`

## Archivos agregados

- `database/01_OPCION_MANUAL_SSMS_CrearBaseCompleta.sql`
- `database/02_Verificar_BaseDatos.sql`
- `database/03_Resetear_BaseDatos.sql`
- `CineStreamCR/appsettings.Automatico.example.json`
- `CineStreamCR/appsettings.Manual.example.json`

## Nota importante

El proyecto no cambia la lógica de negocio, controladores, vistas ni JavaScript. Solo se agregó la opción configurable para decidir si la base se crea automáticamente o si se usará una base creada manualmente.
