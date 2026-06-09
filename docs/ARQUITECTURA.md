# Arquitectura

```text
Navegador
  ↓ AJAX / JSON
CineStreamCR (MVC)
  ↓ Interfaces
CineStreamCR.BLL (reglas, DTO, validación)
  ↓ ICineRepositorio
CineStreamCR.DAL (EF Core y SQL Server)
  ↓
SQL Server / SSMS
```

Los controladores son delgados: reciben solicitudes, llaman servicios y devuelven `Respuesta<T>`. La BLL contiene validaciones y transformación a DTO. La DAL administra consultas, relaciones, persistencia y datos iniciales.
