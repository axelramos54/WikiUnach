\# WikiUnach — Database Schema Reference



\*\*Engine:\*\* MySQL 8.0 on AWS RDS (us-east-2)

\*\*Normalization:\*\* 3rd Normal Form (3NF)

\*\*Primary Keys:\*\* All VARCHAR(50) generated with `Guid.NewGuid()`

\*\*Password storage:\*\* SHA-256 hex (no salt — academic scope)



\---



\## Table Relationships Overview

Facultades ──< Licenciaturas ──< Materias ──< PaginasWiki

Usuarios ──< PaginasWiki (via Revisiones, Comentarios)

PaginasWiki ──< Etiquetas (via Pagina\_Etiquetas junction)

Usuarios ──< Bookmarks ──> PaginasWiki

Usuarios ──< Votos ──> PaginasWiki

Usuarios ──< Notificaciones



\## Tables



| Table | Primary Key | Description |

|---|---|---|

| `tbl\\\_WikiUnach\\\_Facultades` | `ID\\\_Facultad` | University faculties |

| `tbl\\\_WikiUnach\\\_Licenciaturas` | `ID\\\_Licenciatura` | Degree programs per faculty |

| `tbl\\\_WikiUnach\\\_Materias` | `ID\\\_Materia` | Subjects per degree + semester |

| `tbl\\\_WikiUnach\\\_Usuarios` | `ID\\\_Usuario` | Users (all roles) |

| `tbl\\\_WikiUnach\\\_PaginasWiki` | `ID\\\_Pagina` | Wiki pages (academic content) |

| `tbl\\\_WikiUnach\\\_RevisionesWiki` | `ID\\\_Revision` | Version history per page |

| `tbl\\\_WikiUnach\\\_Comentarios` | `ID\\\_Comentario` | Comments on wiki pages |

| `tbl\\\_WikiUnach\\\_Archivos` | `ID\\\_Archivo` | S3 file metadata |

| `tbl\\\_WikiUnach\\\_Etiquetas` | `ID\\\_Etiqueta` | Tags for wiki pages |

| `tbl\\\_WikiUnach\\\_Votos` | `(ID\\\_Usuario, ID\\\_Pagina)` | Like/Dislike per user per page |

| `tbl\\\_WikiUnach\\\_Bookmarks` | `(ID\\\_Usuario, ID\\\_Pagina)` | Saved pages per user |

| `tbl\\\_WikiUnach\\\_Notificaciones` | `ID\\\_Notificacion` | User notification feed |



\## User Roles



| Role | Permissions |

|---|---|

| `VISITANTE` | Read-only access, no account required |

| `ALUMNO` | Read + upload within own faculty/degree/semester |

| `MAESTRO` | Read + upload within own faculty (all degrees) |

| `ADMINISTRADOR` | Full control — requires special access sequence |



\---



> Full SQL creation script available in `src/Database/Migracion\\\_AdminFeatures.sql`

