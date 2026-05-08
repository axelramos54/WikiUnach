-- ============================================================
--  Migración SQL — Funciones de Administrador y Usuario
--  Ejecutar en MySQL Workbench contra la BD WikiUnach (AWS RDS)
-- ============================================================

USE WikiUnach;

-- ── 1. Tabla de bookmarks (tareas guardadas) ────────────────
CREATE TABLE IF NOT EXISTS tbl_WikiUnach_Bookmarks (
    ID_Usuario      VARCHAR(50) NOT NULL,
    ID_Pagina       VARCHAR(50) NOT NULL,
    Fecha_Marcado   DATETIME DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (ID_Usuario, ID_Pagina),
    CONSTRAINT fk_Bookmark_Usuario FOREIGN KEY (ID_Usuario)
        REFERENCES tbl_WikiUnach_Usuarios(ID_Usuario) ON DELETE CASCADE,
    CONSTRAINT fk_Bookmark_Pagina  FOREIGN KEY (ID_Pagina)
        REFERENCES tbl_WikiUnach_PaginasWiki(ID_Pagina) ON DELETE CASCADE
);

-- ── 2. Votos por usuario (Likes / Dislikes individuales) ────
CREATE TABLE IF NOT EXISTS tbl_WikiUnach_VotosUsuario (
    ID_Usuario  VARCHAR(50) NOT NULL,
    ID_Pagina   VARCHAR(50) NOT NULL,
    Tipo_Voto   ENUM('LIKE','DISLIKE') NOT NULL,
    Fecha_Voto  DATETIME DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (ID_Usuario, ID_Pagina),
    CONSTRAINT fk_Voto_Usuario FOREIGN KEY (ID_Usuario)
        REFERENCES tbl_WikiUnach_Usuarios(ID_Usuario) ON DELETE CASCADE,
    CONSTRAINT fk_Voto_Pagina  FOREIGN KEY (ID_Pagina)
        REFERENCES tbl_WikiUnach_PaginasWiki(ID_Pagina) ON DELETE CASCADE
);

-- ── 3. Bloqueo de cuenta de usuario ─────────────────────────
ALTER TABLE tbl_WikiUnach_Usuarios
    ADD COLUMN Cuenta_Bloqueada TINYINT(1) NOT NULL DEFAULT 0;
