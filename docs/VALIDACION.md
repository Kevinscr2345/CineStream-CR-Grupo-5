# Validación técnica realizada

Se ejecutaron comprobaciones automáticas sobre la entrega antes de generar el ZIP final.

## Datos y base de datos

- 12 películas reales.
- 48 personas reales entre directores y elenco.
- 51 créditos de dirección y actuación.
- 10 géneros.
- 12 reseñas iniciales.
- 2 WatchLists y 8 relaciones con películas.
- 12 rutas de video independientes: `video01.mp4` a `video12.mp4`.
- Verificación de los hashes PBKDF2 de las cuentas `demo` y `ana`.
- Comprobación de identificadores utilizados por relaciones y datos iniciales.

## Recursos visuales

- 12 portadas locales de respaldo, todas diferentes y de 900 × 1350 px.
- 12 fondos locales, todos diferentes y de 1600 × 900 px.
- 48 tarjetas locales de respaldo para personas y una tarjeta genérica.
- Carga opcional de miniaturas reales desde Wikipedia/Wikimedia con caché y respaldo local automático.

## Código

- JavaScript validado con `node --check`.
- Archivos C# analizados sintácticamente con un parser de C#.
- Archivos JSON validados.
- Archivos `.csproj` validados como XML.
- Consistencia entre el inicializador de Entity Framework, el manifiesto del catálogo y el script de SQL Server.

## Limitación del entorno de generación

No se ejecutó `dotnet build` porque el entorno donde se preparó la entrega no dispone del SDK de .NET. La solución usa `.NET 10` y Entity Framework Core `10.0.8`, igual que el proyecto de referencia suministrado. La compilación final debe realizarse en Visual Studio 2022 actualizado y con el SDK de .NET 10 instalado.
