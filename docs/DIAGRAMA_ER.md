# Diagrama entidad-relación

```mermaid
erDiagram
  Users ||--o{ WatchLists : crea
  Users ||--o{ Reviews : publica
  Users ||--o{ PlaybackProgresses : guarda
  Movies ||--o{ Reviews : recibe
  Movies ||--o{ PlaybackProgresses : registra
  Movies ||--o{ MovieGenres : clasifica
  Genres ||--o{ MovieGenres : agrupa
  Movies ||--o{ MovieCredits : acredita
  People ||--o{ MovieCredits : participa
  WatchLists ||--o{ WatchListMovies : contiene
  Movies ||--o{ WatchListMovies : aparece
```
