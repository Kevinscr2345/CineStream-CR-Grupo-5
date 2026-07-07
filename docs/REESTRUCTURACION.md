# Reestructuración de carpetas

Este proyecto conserva la lógica, namespaces y comportamiento original. La reorganización se hizo para que la solución se parezca más a la estructura vista en clase:

- `CineStreamCR`: capa Web / presentación.
- `CineStreamCR.BLL`: DTOs, respuestas, AutoMapper y servicios por módulo.
- `CineStreamCR.DAL`: DbContext, entidades separadas y repositorio en subcarpeta.

Archivos grandes separados por estructura:

- `CineStreamCR.BLL/Services/Interfaces.cs` se separó en carpetas por módulo.
- `CineStreamCR.BLL/Services/Servicios.cs` se separó en servicios individuales por módulo.
- `CineStreamCR.BLL/Dtos/Contratos.cs` se separó en DTOs individuales por módulo.
- `CineStreamCR.DAL/Entidades/Entidades.cs` se separó en una clase por archivo.
- `CineStreamCR.DAL/Repositorios` se ordenó en `Repositorios/Cine`.

Los archivos originales quedaron como referencia en `docs/estructura-original` con extensión `.txt`, para que no se compilen dos veces.
