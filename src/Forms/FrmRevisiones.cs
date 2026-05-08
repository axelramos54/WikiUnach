using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WUNACH
{
    public partial class FrmRevisiones : Form
    {
        // ══════════════════════════════════════════════════════════════════════════
        // CLASES DE DATOS (public para que FrmDetallesTarea las use)
        // ══════════════════════════════════════════════════════════════════════════

        public class ArchivoNuevo
        {
            public string RutaLocal   { get; set; }
            public string Nombre      { get; set; }
            public string Descripcion { get; set; }
            public long   TamanoBytes { get; set; }
            public string Extension   { get; set; }
        }

        public class ArchivoEliminar
        {
            public string IdArchivo { get; set; }
            public string UrlS3     { get; set; }
            public string Nombre    { get; set; }
        }

        /// <summary>Datos para GUARDAR una revisión nueva.</summary>
        public class DatosRevision
        {
            public string             IdPagina          { get; set; }
            public string             IdMateria         { get; set; }
            public string             TituloPagina      { get; set; }
            public string             ContenidoNuevo    { get; set; }
            public string             ContenidoAnterior { get; set; }
            public List<string>       TagsAgregados     { get; set; } = new List<string>();
            public List<string>       TagsEliminados    { get; set; } = new List<string>();
            public List<ArchivoNuevo>    ArchivosNuevos   { get; set; } = new List<ArchivoNuevo>();
            public List<ArchivoEliminar> ArchivosEliminar { get; set; } = new List<ArchivoEliminar>();
        }

        /// <summary>Datos para VER una revisión ya guardada (solo lectura).</summary>
        public class RevisionExistente
        {
            public string   IdRevision           { get; set; }
            public string   TituloPagina         { get; set; }
            public string   NombreUsuario        { get; set; }
            public DateTime FechaCreacion        { get; set; }
            public string   InstantaneaContenido { get; set; }
            public string   MensajeCommit        { get; set; }
        }

        // ══════════════════════════════════════════════════════════════════════════
        // CAMPOS PRIVADOS
        // ══════════════════════════════════════════════════════════════════════════

        private readonly DatosRevision    _datos;       // modo guardar
        private readonly RevisionExistente _revision;   // modo ver
        private readonly bool              _modoVer;

        private Button _btnGuardar;

        // ══════════════════════════════════════════════════════════════════════════
        // CONSTRUCTORES
        // ══════════════════════════════════════════════════════════════════════════

        /// <summary>Modo GUARDAR: se llama desde FrmDetallesTarea en modo edición.</summary>
        public FrmRevisiones(DatosRevision datos)
        {
            InitializeComponent();
            _datos   = datos;
            _modoVer = false;
            this.Load += FrmRevisiones_Load;
        }

        /// <summary>Modo VER: se llama al hacer click en una revisión del historial.</summary>
        public FrmRevisiones(RevisionExistente revision)
        {
            InitializeComponent();
            _revision = revision;
            _modoVer  = true;
            this.Load += FrmRevisiones_Load;
        }

        // ══════════════════════════════════════════════════════════════════════════
        // CARGA INICIAL
        // ══════════════════════════════════════════════════════════════════════════

        private void FrmRevisiones_Load(object sender, EventArgs e)
        {
            if (_modoVer)
                ConfigurarModoVer();
            else
                ConfigurarModoGuardar();
        }

        // ── Modo VER ──────────────────────────────────────────────────────────────
        private void ConfigurarModoVer()
        {
            this.Text = "Revisión — " + _revision.FechaCreacion.ToString("dd/MM/yyyy HH:mm");

            // Todos los campos de solo lectura
            txtpagina.ReadOnly   = true;
            txtusuario.ReadOnly  = true;
            txtfecha.ReadOnly    = true;
            txtrevison.ReadOnly  = true;
            txtmensaje.ReadOnly  = true;

            txtpagina.Text   = _revision.TituloPagina;
            txtusuario.Text  = _revision.NombreUsuario;
            txtfecha.Text    = _revision.FechaCreacion.ToString("dd/MM/yyyy   HH:mm");
            txtrevison.Text  = _revision.InstantaneaContenido;
            txtmensaje.Text  = _revision.MensajeCommit;

            // Color de fondo ligeramente diferente para dejar claro que es lectura
            Color readBg = Color.FromArgb(248, 248, 252);
            txtrevison.BackColor  = readBg;
            txtmensaje.BackColor  = readBg;
            txtusuario.BackColor  = readBg;
            txtfecha.BackColor    = readBg;
            txtpagina.BackColor   = readBg;

            // lblhistorial: indicador visual del ID de revisión
            lblhistorial.Text = "ID: " + _revision.IdRevision.Substring(0, 8) + "…";

            // Botón "Regresar" queda como único botón de acción
            btnregreso.Text = "← Cerrar";
        }

        // ── Modo GUARDAR ──────────────────────────────────────────────────────────
        private void ConfigurarModoGuardar()
        {
            this.Text = "Subir Revisión";

            txtpagina.ReadOnly  = true;
            txtusuario.ReadOnly = true;
            txtfecha.ReadOnly   = true;
            txtrevison.ReadOnly = true;
            // txtmensaje queda editable

            txtpagina.Text  = _datos.TituloPagina;
            txtusuario.Text = SesionUsuario.Nombre;
            txtfecha.Text   = DateTime.Now.ToString("dd/MM/yyyy   HH:mm");
            txtrevison.Text = GenerarTextoDiff();

            lblhistorial.Text = "Cambios detectados automáticamente";

            // Botón "Guardar Revisión"
            _btnGuardar = new Button
            {
                Text      = "Guardar Revisión",
                Location  = new Point(btnregreso.Right + 20, btnregreso.Top),
                Size      = new Size(160, btnregreso.Height),
                BackColor = Color.FromArgb(30, 100, 210),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            _btnGuardar.FlatAppearance.BorderSize = 0;
            _btnGuardar.Click += BtnGuardar_Click;
            this.Controls.Add(_btnGuardar);
            _btnGuardar.BringToFront();
        }

        // ══════════════════════════════════════════════════════════════════════════
        // GENERACIÓN AUTOMÁTICA DEL DIFF
        // ══════════════════════════════════════════════════════════════════════════

        private string GenerarTextoDiff()
        {
            var lineas = new List<string>();

            if (!string.Equals(_datos.ContenidoAnterior?.Trim(),
                                _datos.ContenidoNuevo?.Trim(),
                                StringComparison.Ordinal))
                lineas.Add("• Descripción actualizada");

            if (_datos.TagsAgregados.Count > 0)
                lineas.Add("• Etiquetas añadidas: " + string.Join(", ", _datos.TagsAgregados));

            if (_datos.TagsEliminados.Count > 0)
                lineas.Add("• Etiquetas eliminadas: " + string.Join(", ", _datos.TagsEliminados));

            foreach (var a in _datos.ArchivosEliminar)
                lineas.Add("• Archivo eliminado: " + a.Nombre);

            foreach (var a in _datos.ArchivosNuevos)
                lineas.Add("• Archivo añadido: " + a.Nombre + a.Extension);

            return lineas.Count > 0
                   ? string.Join(Environment.NewLine, lineas)
                   : "Sin cambios registrados.";
        }

        // ══════════════════════════════════════════════════════════════════════════
        // GUARDAR REVISIÓN (async para no bloquear la UI)
        // ══════════════════════════════════════════════════════════════════════════

        private async void BtnGuardar_Click(object sender, EventArgs e)
        {
            string mensajeCommit = txtmensaje.Text.Trim();
            if (string.IsNullOrWhiteSpace(mensajeCommit))
            {
                MessageBox.Show("Escribe el motivo de los cambios en el campo de mensaje.",
                    "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtmensaje.Focus();
                return;
            }

            string instantanea = txtrevison.Text.Trim();
            if (instantanea == "Sin cambios registrados.")
            {
                MessageBox.Show("No hay cambios que guardar.",
                    "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _btnGuardar.Enabled = false;
            _btnGuardar.Text    = "⏳ Guardando...";
            btnregreso.Enabled  = false;

            var    datos       = _datos;
            string snapshot    = instantanea;
            string commitMsg   = mensajeCommit;

            try
            {
                var urlsSubidos = new List<(ArchivoNuevo Archivo, string Url)>();

                if (datos.ArchivosNuevos.Count > 0)
                {
                    _btnGuardar.Text = "⬆ Subiendo archivos...";
                    urlsSubidos = await Task.Run(() =>
                    {
                        var lista = new List<(ArchivoNuevo, string)>();
                        foreach (var archivo in datos.ArchivosNuevos)
                        {
                            string ext = archivo.Extension.TrimStart('.').ToLower();
                            string url = S3Service.SubirArchivo(archivo.RutaLocal, "tareas/" + ext);
                            lista.Add((archivo, url));
                        }
                        return lista;
                    });
                }

                _btnGuardar.Text = "💾 Guardando en BD...";
                await Task.Run(() => GuardarEnBD(datos, urlsSubidos, snapshot, commitMsg));

                MessageBox.Show("¡Revisión guardada exitosamente!",
                    "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar la revisión:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _btnGuardar.Enabled = true;
                _btnGuardar.Text    = "Guardar Revisión";
                btnregreso.Enabled  = true;
            }
        }

        // ══════════════════════════════════════════════════════════════════════════
        // LÓGICA DE BD (corre en background thread)
        // ══════════════════════════════════════════════════════════════════════════

        private void GuardarEnBD(DatosRevision datos,
                                  List<(ArchivoNuevo Archivo, string Url)> urlsSubidos,
                                  string instantanea, string mensajeCommit)
        {
            using (MySqlConnection conn = DBConexion.ObtenerConexion())
            {
                conn.Open();
                using (MySqlTransaction tx = conn.BeginTransaction())
                {
                    try
                    {
                        // 1. Actualizar contenido si cambió
                        if (!string.Equals(datos.ContenidoAnterior?.Trim(),
                                           datos.ContenidoNuevo?.Trim(),
                                           StringComparison.Ordinal))
                        {
                            using (var cmd = new MySqlCommand(@"
                                UPDATE tbl_WikiUnach_PaginasWiki
                                SET    Contenido_Actual    = @contenido,
                                       Fecha_Actualizacion = NOW()
                                WHERE  ID_Pagina           = @idPagina", conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@contenido", datos.ContenidoNuevo);
                                cmd.Parameters.AddWithValue("@idPagina",  datos.IdPagina);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // 2. Eliminar archivos marcados de la BD
                        foreach (var arch in datos.ArchivosEliminar)
                        {
                            using (var cmd = new MySqlCommand(@"
                                UPDATE tbl_WikiUnach_PaginasWiki
                                SET    ID_Archivo_Principal = NULL
                                WHERE  ID_Archivo_Principal = @idArch
                                  AND  ID_Pagina            = @idPag", conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@idArch", arch.IdArchivo);
                                cmd.Parameters.AddWithValue("@idPag",  datos.IdPagina);
                                cmd.ExecuteNonQuery();
                            }
                            using (var cmd = new MySqlCommand(@"
                                UPDATE tbl_WikiUnach_Comentarios
                                SET    ID_Archivo_Adjunto = NULL
                                WHERE  ID_Archivo_Adjunto = @idArch", conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@idArch", arch.IdArchivo);
                                cmd.ExecuteNonQuery();
                            }
                            using (var cmd = new MySqlCommand(@"
                                DELETE FROM tbl_WikiUnach_Archivos
                                WHERE  ID_Archivo = @idArch", conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@idArch", arch.IdArchivo);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // 3. Etiquetas eliminadas
                        foreach (string tag in datos.TagsEliminados)
                        {
                            string t = tag.TrimStart('#').Trim();
                            using (var cmd = new MySqlCommand(@"
                                DELETE pe
                                FROM   tbl_WikiUnach_Pagina_Etiquetas pe
                                INNER JOIN tbl_WikiUnach_Etiquetas e ON e.ID_Etiqueta = pe.ID_Etiqueta
                                WHERE  pe.ID_Pagina      = @idPagina
                                  AND  e.Nombre_Etiqueta = @nombre", conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@idPagina", datos.IdPagina);
                                cmd.Parameters.AddWithValue("@nombre",   t);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // 4. Etiquetas añadidas
                        foreach (string tag in datos.TagsAgregados)
                        {
                            string t = tag.TrimStart('#').Trim();
                            using (var cmd = new MySqlCommand(@"
                                INSERT IGNORE INTO tbl_WikiUnach_Etiquetas
                                    (ID_Etiqueta, Nombre_Etiqueta)
                                VALUES (@id, @nombre)", conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@id",     Guid.NewGuid().ToString());
                                cmd.Parameters.AddWithValue("@nombre", t);
                                cmd.ExecuteNonQuery();
                            }

                            string idEtiqueta;
                            using (var cmd = new MySqlCommand(@"
                                SELECT ID_Etiqueta FROM tbl_WikiUnach_Etiquetas
                                WHERE  Nombre_Etiqueta = @nombre", conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@nombre", t);
                                idEtiqueta = cmd.ExecuteScalar()?.ToString();
                            }

                            if (!string.IsNullOrEmpty(idEtiqueta))
                            {
                                using (var cmd = new MySqlCommand(@"
                                    INSERT IGNORE INTO tbl_WikiUnach_Pagina_Etiquetas
                                        (ID_Pagina, ID_Etiqueta)
                                    VALUES (@idPag, @idEt)", conn, tx))
                                {
                                    cmd.Parameters.AddWithValue("@idPag", datos.IdPagina);
                                    cmd.Parameters.AddWithValue("@idEt",  idEtiqueta);
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }

                        // 5. Insertar archivos nuevos
                        foreach (var (archivo, url) in urlsSubidos)
                        {
                            string idArchivo = Guid.NewGuid().ToString();
                            string ext       = archivo.Extension.TrimStart('.').ToLower();

                            using (var cmd = new MySqlCommand(@"
                                INSERT INTO tbl_WikiUnach_Archivos
                                    (ID_Archivo, ID_Materia, Titulo, Descripcion,
                                     URL_Archivo, Tipo_Archivo, Tamano_Bytes,
                                     Fecha_Creacion, ID_Usuario)
                                VALUES
                                    (@id, @idMat, @titulo, @desc,
                                     @url, @tipo, @tam, NOW(), @idUser)", conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@id",     idArchivo);
                                cmd.Parameters.AddWithValue("@idMat",  datos.IdMateria);
                                cmd.Parameters.AddWithValue("@titulo", archivo.Nombre);
                                cmd.Parameters.AddWithValue("@desc",   archivo.Descripcion ?? "");
                                cmd.Parameters.AddWithValue("@url",    url);
                                cmd.Parameters.AddWithValue("@tipo",   ext);
                                cmd.Parameters.AddWithValue("@tam",    archivo.TamanoBytes);
                                cmd.Parameters.AddWithValue("@idUser", SesionUsuario.ID_Usuario);
                                cmd.ExecuteNonQuery();
                            }

                            using (var cmd = new MySqlCommand(@"
                                INSERT INTO tbl_WikiUnach_Comentarios
                                    (ID_Comentario, ID_Pagina, ID_Usuario,
                                     Contenido, ID_Archivo_Adjunto)
                                VALUES
                                    (@idCom, @idPag, @idUser, @contenido, @idArch)", conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@idCom",     Guid.NewGuid().ToString());
                                cmd.Parameters.AddWithValue("@idPag",     datos.IdPagina);
                                cmd.Parameters.AddWithValue("@idUser",    SesionUsuario.ID_Usuario);
                                cmd.Parameters.AddWithValue("@contenido",
                                    "[Archivo adjunto: " + archivo.Nombre + archivo.Extension + "]");
                                cmd.Parameters.AddWithValue("@idArch",    idArchivo);
                                cmd.ExecuteNonQuery();
                            }
                        }

                        // 6. Insertar revisión
                        using (var cmd = new MySqlCommand(@"
                            INSERT INTO tbl_WikiUnach_RevisionesWiki
                                (ID_Revision, ID_Pagina, ID_Usuario,
                                 Instantanea_Contenido, Mensaje_Commit, Fecha_Creacion)
                            VALUES
                                (@id, @idPag, @idUser, @instantanea, @mensaje, NOW())", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@id",          Guid.NewGuid().ToString());
                            cmd.Parameters.AddWithValue("@idPag",       datos.IdPagina);
                            cmd.Parameters.AddWithValue("@idUser",      SesionUsuario.ID_Usuario);
                            cmd.Parameters.AddWithValue("@instantanea", instantanea);
                            cmd.Parameters.AddWithValue("@mensaje",     mensajeCommit);
                            cmd.ExecuteNonQuery();
                        }

                        tx.Commit();

                        // 7. Eliminar de S3 post-commit (no crítico)
                        foreach (var arch in datos.ArchivosEliminar)
                            try
                            {
                                string key = new Uri(arch.UrlS3).AbsolutePath.TrimStart('/');
                                S3Service.EliminarArchivo(key);
                            }
                            catch { }
                    }
                    catch
                    {
                        tx.Rollback();
                        foreach (var (_, url) in urlsSubidos)
                            try { S3Service.EliminarArchivo(new Uri(url).AbsolutePath.TrimStart('/')); }
                            catch { }
                        throw;
                    }
                }
            }
        }

        // ══════════════════════════════════════════════════════════════════════════
        // BOTÓN REGRESAR
        // ══════════════════════════════════════════════════════════════════════════

        private void btnregreso_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
