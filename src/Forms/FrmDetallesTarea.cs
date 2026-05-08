using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WUNACH
{
    public partial class FrmDetallesTarea : Form
    {
        public string ID_Pagina { get; private set; }

        // Carpetas posibles para los íconos de tipo de archivo (en orden).
        // Distribuye la app copiando "TiposArchivo" junto al .exe.
        private static readonly string[] RUTAS_ICONOS = new[]
        {
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TiposArchivo"),
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tipos de Archivo"),
            @"C:\Users\luisp\OneDrive\Pictures\WUNACH\Tipos de Archivo"
        };

        /// <summary>Devuelve la ruta de un ícono buscando en todas las carpetas configuradas.</summary>
        private static string ResolverRutaIcono(string nombreArchivo)
        {
            foreach (string carpeta in RUTAS_ICONOS)
            {
                string ruta = Path.Combine(carpeta, nombreArchivo);
                if (File.Exists(ruta)) return ruta;
            }
            return null;
        }

        // ── PANEL LATERAL DE COMENTARIOS ─────────────────────────────────────────
        private Panel               pnlComentariosSlide;
        private FlowLayoutPanel     flpComentariosSlide;
        private TextBox             txtNuevoComentarioSlide;
        private Button              btnEnviarComentarioSlide;
        private System.Windows.Forms.Timer timerComentarios;
        private bool                comentariosAbierto = false;

        // ── PANEL LATERAL DE ARCHIVOS ─────────────────────────────────────────────
        private Panel               pnlArchivosSlide;
        private FlowLayoutPanel     flpArchivosSlide;
        private System.Windows.Forms.Timer timerArchivos;
        private bool                archivosAbierto = false;

        // ── PANEL LATERAL DE REVISIONES ───────────────────────────────────────────
        private Panel               pnlRevisionesSlide;
        private FlowLayoutPanel     flpRevisionesSlide;
        private System.Windows.Forms.Timer timerRevisiones;
        private bool                revisionesAbierto = false;

        // ── MODO EDICIÓN ─────────────────────────────────────────────────────────
        private bool               _modoEdicion       = false;
        private string             _contenidoOriginal = "";
        private string             _idMateriaActual   = "";
        private string             _idAutor           = "";   // ID del usuario autor de la página
        private bool               _esBookmarked      = false;
        private List<string>       _tagsOriginales    = new List<string>();
        private List<string>       _tagsActuales      = new List<string>();
        private List<FrmRevisiones.ArchivoNuevo>    _archivosNuevos   = new List<FrmRevisiones.ArchivoNuevo>();
        private List<FrmRevisiones.ArchivoEliminar> _archivosEliminar = new List<FrmRevisiones.ArchivoEliminar>();
        private FlowLayoutPanel    _flpTags;

        public FrmDetallesTarea(string idPagina, bool modoEdicion = false)
        {
            InitializeComponent();
            ID_Pagina    = idPagina;
            _modoEdicion = modoEdicion;

            // Votos — botones del Designer
            btnLike.Click    += (s, e) => VotarPagina("LIKE");
            btnDislike.Click += (s, e) => VotarPagina("DISLIKE");

            // Bookmark
            btnBookmark.Click += (s, e) => AlternarBookmark();

            // Click en el nombre del autor → abrir su perfil
            lblInfoUsuario.Cursor = Cursors.Hand;
            lblInfoUsuario.Click += (s, e) =>
            {
                if (!string.IsNullOrEmpty(_idAutor))
                    new FrmCuenta(_idAutor).ShowDialog(this);
            };

            this.Load += (s, e) =>
            {
                // Hacer responsivo ANTES de crear paneles laterales (que dependen del tamaño)
                pnlCuerpoDetalle.Dock = DockStyle.Fill;
                pnlHeaderDetalle.SendToBack();
                panel1.SendToBack();
                pnlHeaderDetalle.BringToFront();
                panel1.BringToFront();
                pnlCuerpoDetalle.BringToFront();

                CrearPanelComentarios();
                CrearPanelArchivos();
                CrearPanelRevisiones();
                CargarPagina();          // carga datos reales de la BD
                CargarEstadoBookmark();
                if (_modoEdicion) ConfigurarModoEdicion();

                LayoutHelper.AplicarTitulo(this, "Detalles de Tarea");
                Tema.AplicarA(this);
            };

            this.MinimumSize = new System.Drawing.Size(900, 550);
        }

        // ── EVENTOS DE BOTONES (conectar en el Designer) ────────────────────────
        private void btnVerComentarios_Click(object sender, EventArgs e)
        {
            if (comentariosAbierto) CerrarComentarios();
            else                    AbrirComentarios();
        }

        private void btnVerArchivos_Click(object sender, EventArgs e)
        {
            if (archivosAbierto) CerrarArchivos();
            else                 AbrirArchivos();
        }

        // ── CONSTRUCCIÓN DEL PANEL LATERAL ──────────────────────────────────────
        private void CrearPanelComentarios()
        {
            int anchoSlide  = (int)(this.ClientSize.Width  * 0.75);
            int altoSlide   = this.ClientSize.Height - pnlHeaderDetalle.Height;
            int yOrigen     = pnlHeaderDetalle.Height;

            pnlComentariosSlide = new Panel
            {
                Size        = new Size(anchoSlide, altoSlide),
                Location    = new Point(this.ClientSize.Width, yOrigen), // fuera de pantalla
                BackColor   = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Visible     = false
            };

            // ── HEADER ───────────────────────────────────────────────────────────
            Panel pnlHeaderSlide = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 48,
                BackColor = Color.FromArgb(28, 28, 28)
            };

            Label lblTituloComentarios = new Label
            {
                Text      = "Comentarios",
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize  = true,
                Location  = new Point(15, 12)
            };

            Button btnCerrar = new Button
            {
                Text      = "✕",
                ForeColor = Color.White,
                BackColor = Color.FromArgb(28, 28, 28),
                FlatStyle = FlatStyle.Flat,
                Size      = new Size(40, 40),
                Location  = new Point(anchoSlide - 50, 4),
                Cursor    = Cursors.Hand,
                Font      = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 50, 50);
            btnCerrar.Click += (s, e) => CerrarComentarios();

            pnlHeaderSlide.Controls.Add(lblTituloComentarios);
            pnlHeaderSlide.Controls.Add(btnCerrar);

            // ── BARRA DE NUEVO COMENTARIO (bottom) ───────────────────────────────
            Panel pnlEntrada = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 80,
                BackColor = Color.FromArgb(248, 248, 248),
                Padding   = new Padding(12, 10, 12, 10)
            };

            if (SesionUsuario.Rol == "VISITANTE")
            {
                // Visitantes solo ven un aviso en lugar del campo de texto
                Label lblAvisoVisitante = new Label
                {
                    Text      = "🔒  Inicia sesión para comentar",
                    Location  = new Point(12, 28),
                    AutoSize  = true,
                    Font      = new Font("Segoe UI", 9.5f, FontStyle.Italic),
                    ForeColor = Color.FromArgb(160, 160, 160)
                };
                pnlEntrada.Controls.Add(lblAvisoVisitante);

                // Valores por defecto para evitar NullReferenceException en otros métodos
                txtNuevoComentarioSlide  = new TextBox { Visible = false };
                btnEnviarComentarioSlide = new Button  { Visible = false };
            }
            else
            {
                txtNuevoComentarioSlide = new TextBox
                {
                    Multiline = true,
                    Location  = new Point(12, 12),
                    Size      = new Size(anchoSlide - 140, 54),
                    Font      = new Font("Segoe UI", 10),
                    BackColor = Color.White,
                    ForeColor = Color.Gray,
                    Text      = "Escribe un comentario..."
                };
                // Placeholder manual (PlaceholderText no existe en .NET Framework 4.7.2)
                txtNuevoComentarioSlide.Enter += (s, e) =>
                {
                    if (txtNuevoComentarioSlide.ForeColor == Color.Gray)
                    {
                        txtNuevoComentarioSlide.Text      = "";
                        txtNuevoComentarioSlide.ForeColor = Color.Black;
                    }
                };
                txtNuevoComentarioSlide.Leave += (s, e) =>
                {
                    if (string.IsNullOrWhiteSpace(txtNuevoComentarioSlide.Text))
                    {
                        txtNuevoComentarioSlide.Text      = "Escribe un comentario...";
                        txtNuevoComentarioSlide.ForeColor = Color.Gray;
                    }
                };

                btnEnviarComentarioSlide = new Button
                {
                    Text      = "Enviar",
                    Location  = new Point(anchoSlide - 120, 22),
                    Size      = new Size(95, 38),
                    BackColor = Color.FromArgb(30, 100, 210),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor    = Cursors.Hand,
                    Font      = new Font("Segoe UI", 9, FontStyle.Bold)
                };
                btnEnviarComentarioSlide.FlatAppearance.BorderSize = 0;
                btnEnviarComentarioSlide.Click += BtnEnviarComentario_Click;

                pnlEntrada.Controls.Add(txtNuevoComentarioSlide);
                pnlEntrada.Controls.Add(btnEnviarComentarioSlide);
            }

            // ── LISTA SCROLLEABLE DE COMENTARIOS ─────────────────────────────────
            flpComentariosSlide = new FlowLayoutPanel
            {
                Dock          = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll    = true,
                WrapContents  = false,
                BackColor     = Color.White,
                Padding       = new Padding(12, 8, 12, 8)
            };

            // Orden: Fill primero, luego Bottom y Top
            pnlComentariosSlide.Controls.Add(flpComentariosSlide);
            pnlComentariosSlide.Controls.Add(pnlEntrada);
            pnlComentariosSlide.Controls.Add(pnlHeaderSlide);

            this.Controls.Add(pnlComentariosSlide);
            pnlComentariosSlide.BringToFront();

            timerComentarios      = new System.Windows.Forms.Timer { Interval = 12 };
            timerComentarios.Tick += TimerComentarios_Tick;
        }

        // ── PANEL LATERAL DE ARCHIVOS ────────────────────────────────────────────
        private void CrearPanelArchivos()
        {
            int anchoSlide = (int)(this.ClientSize.Width * 0.25);
            int yOrigen    = pnlCuerpoDetalle.Top;
            int altoSlide  = this.ClientSize.Height - yOrigen;

            pnlArchivosSlide = new Panel
            {
                Size        = new Size(anchoSlide, altoSlide),
                Location    = new Point(-anchoSlide, yOrigen), // fuera de pantalla izquierda
                BackColor   = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Visible     = false
            };

            // Header
            Panel pnlHeaderArchivos = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 48,
                BackColor = Color.FromArgb(28, 28, 28)
            };

            Label lblTituloArchivos = new Label
            {
                Text      = "Archivos adjuntos",
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 11, FontStyle.Bold),
                AutoSize  = true,
                Location  = new Point(15, 12)
            };

            Button btnCerrarArchivos = new Button
            {
                Text      = "✕",
                ForeColor = Color.White,
                BackColor = Color.FromArgb(28, 28, 28),
                FlatStyle = FlatStyle.Flat,
                Size      = new Size(40, 40),
                Location  = new Point(anchoSlide - 50, 4),
                Cursor    = Cursors.Hand,
                Font      = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnCerrarArchivos.FlatAppearance.BorderSize            = 0;
            btnCerrarArchivos.FlatAppearance.MouseOverBackColor    = Color.FromArgb(200, 50, 50);
            btnCerrarArchivos.Click += (s, e) => CerrarArchivos();

            pnlHeaderArchivos.Controls.Add(lblTituloArchivos);
            pnlHeaderArchivos.Controls.Add(btnCerrarArchivos);

            // Lista scrolleable
            flpArchivosSlide = new FlowLayoutPanel
            {
                Dock          = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll    = true,
                WrapContents  = false,
                BackColor     = Color.White,
                Padding       = new Padding(10, 8, 10, 8)
            };

            pnlArchivosSlide.Controls.Add(flpArchivosSlide);
            pnlArchivosSlide.Controls.Add(pnlHeaderArchivos);

            this.Controls.Add(pnlArchivosSlide);
            pnlArchivosSlide.BringToFront();

            timerArchivos      = new System.Windows.Forms.Timer { Interval = 12 };
            timerArchivos.Tick += TimerArchivos_Tick;
        }

        // ── ANIMACIÓN COMENTARIOS (desliza desde la DERECHA) ─────────────────────
        private void AbrirComentarios()
        {
            int xDestino = (int)(this.ClientSize.Width * 0.25);
            pnlComentariosSlide.Size     = new Size(
                (int)(this.ClientSize.Width * 0.75),
                this.ClientSize.Height - pnlCuerpoDetalle.Top);
            pnlComentariosSlide.Location = new Point(this.ClientSize.Width, pnlCuerpoDetalle.Top);
            pnlComentariosSlide.Visible  = true;
            pnlComentariosSlide.BringToFront();
            comentariosAbierto       = true;
            timerComentarios.Tag     = xDestino;
            timerComentarios.Start();
            CargarComentarios();
        }

        private void CerrarComentarios()
        {
            comentariosAbierto   = false;
            timerComentarios.Tag = this.ClientSize.Width;
            timerComentarios.Start();
        }

        private void TimerComentarios_Tick(object sender, EventArgs e)
        {
            int target  = (int)timerComentarios.Tag;
            int current = pnlComentariosSlide.Left;
            int paso    = (target - current) / 4;

            if (Math.Abs(paso) < 8)
            {
                pnlComentariosSlide.Left = target;
                timerComentarios.Stop();
                if (!comentariosAbierto) pnlComentariosSlide.Visible = false;
            }
            else
            {
                pnlComentariosSlide.Left += paso;
            }
        }

        // ── ANIMACIÓN ARCHIVOS (desliza desde la IZQUIERDA) ──────────────────────
        private void AbrirArchivos()
        {
            int anchoSlide = (int)(this.ClientSize.Width * 0.25);
            pnlArchivosSlide.Size     = new Size(anchoSlide,
                                                 this.ClientSize.Height - pnlCuerpoDetalle.Top);
            pnlArchivosSlide.Location = new Point(-anchoSlide, pnlCuerpoDetalle.Top);
            pnlArchivosSlide.Visible  = true;
            pnlArchivosSlide.BringToFront();
            archivosAbierto      = true;
            timerArchivos.Tag    = 0;
            timerArchivos.Start();
            CargarArchivos();
        }

        private void CerrarArchivos()
        {
            archivosAbierto  = false;
            timerArchivos.Tag = -(int)(this.ClientSize.Width * 0.25);
            timerArchivos.Start();
        }

        private void TimerArchivos_Tick(object sender, EventArgs e)
        {
            int target  = (int)timerArchivos.Tag;
            int current = pnlArchivosSlide.Left;
            int paso    = (target - current) / 4;

            if (Math.Abs(paso) < 8)
            {
                pnlArchivosSlide.Left = target;
                timerArchivos.Stop();
                if (!archivosAbierto) pnlArchivosSlide.Visible = false;
            }
            else
            {
                pnlArchivosSlide.Left += paso;
            }
        }

        // ── CARGA PRINCIPAL DE LA PÁGINA ─────────────────────────────────────────
        private void CargarPagina()
        {
            try
            {
                using (MySqlConnection conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();

                    // Obtiene datos de la página, la materia y el autor (primera revisión)
                    string sql = @"
                        SELECT
                            pw.Titulo,
                            pw.Contenido_Actual,
                            pw.Tipo_Actividad,
                            pw.Votos_Positivos,
                            pw.Votos_Negativos,
                            pw.Fecha_Creacion,
                            pw.Fecha_Actualizacion,
                            pw.ID_Materia,
                            m.Nombre_Materia,
                            u.Nombre     AS NombreAutor,
                            u.ID_Usuario AS IdAutor
                        FROM tbl_WikiUnach_PaginasWiki pw
                        INNER JOIN tbl_WikiUnach_Materias m
                               ON pw.ID_Materia = m.ID_Materia
                        LEFT JOIN (
                            SELECT r1.ID_Pagina, r1.ID_Usuario
                            FROM tbl_WikiUnach_RevisionesWiki r1
                            INNER JOIN (
                                SELECT ID_Pagina, MIN(Fecha_Creacion) AS PrimeraFecha
                                FROM tbl_WikiUnach_RevisionesWiki
                                GROUP BY ID_Pagina
                            ) r2 ON r1.ID_Pagina = r2.ID_Pagina
                               AND r1.Fecha_Creacion = r2.PrimeraFecha
                        ) r ON r.ID_Pagina = pw.ID_Pagina
                        LEFT JOIN tbl_WikiUnach_Usuarios u
                               ON r.ID_Usuario = u.ID_Usuario
                        WHERE pw.ID_Pagina = @idPagina";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@idPagina", ID_Pagina);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblTituloTarea.Text = reader["Titulo"].ToString();
                                richTextBox1.Text   = reader["Contenido_Actual"].ToString();
                                _contenidoOriginal  = richTextBox1.Text;
                                _idMateriaActual    = reader["ID_Materia"].ToString();
                                lblMateriaTag.Text  = reader["Nombre_Materia"].ToString();
                                lblInfoUsuario.Text = reader["NombreAutor"] == DBNull.Value
                                                      ? "Autor desconocido"
                                                      : reader["NombreAutor"].ToString();
                                _idAutor = reader["IdAutor"] == DBNull.Value
                                           ? ""
                                           : reader["IdAutor"].ToString();

                                // Labels que debes tener en el Designer:
                                lblTipoActividad.Text      = "📌 " + reader["Tipo_Actividad"].ToString();
                                lblfechaCreacion.Text      = "Creado: "      + Convert.ToDateTime(reader["Fecha_Creacion"]).ToString("dd/MM/yyyy");
                                lblfechaActualizacion.Text = "Actualizado: " + Convert.ToDateTime(reader["Fecha_Actualizacion"]).ToString("dd/MM/yyyy");

                                ActualizarContadorVotos(
                                    Convert.ToInt32(reader["Votos_Positivos"]),
                                    Convert.ToInt32(reader["Votos_Negativos"]));
                            }
                        }
                    }

                    CargarEtiquetas(conn);
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al cargar la página:\n" + ex.Message,
                    "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarEtiquetas(MySqlConnection conn)
        {
            string sql = @"
                SELECT e.Nombre_Etiqueta
                FROM tbl_WikiUnach_Etiquetas e
                INNER JOIN tbl_WikiUnach_Pagina_Etiquetas pe
                        ON e.ID_Etiqueta = pe.ID_Etiqueta
                WHERE pe.ID_Pagina = @idPagina";

            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@idPagina", ID_Pagina);
                var etiquetas = new List<string>();

                _tagsOriginales.Clear();
                _tagsActuales.Clear();

                using (MySqlDataReader reader = cmd.ExecuteReader())
                    while (reader.Read())
                    {
                        string nombre = reader["Nombre_Etiqueta"].ToString();
                        etiquetas.Add("🏷 " + nombre);
                        _tagsOriginales.Add("#" + nombre);
                        _tagsActuales.Add("#" + nombre);
                    }

                lbltags.Text = etiquetas.Count > 0
                               ? string.Join("   ", etiquetas)
                               : "Sin etiquetas";
            }
        }

        // ── VOTOS ─────────────────────────────────────────────────────────────────
        private void ActualizarContadorVotos(int likes, int dislikes)
        {
            lblContadorVotos.Text = $"👍 {likes}   👎 {dislikes}";
        }

        /// <summary>
        /// tipoVoto = "LIKE" o "DISLIKE".
        /// Lógica: si el usuario nunca votó → inserta y +1 al contador correspondiente.
        ///         si vota lo mismo otra vez → quita el voto y -1.
        ///         si cambia de tipo → -1 al anterior, +1 al nuevo, UPDATE en la tabla.
        /// </summary>
        private void VotarPagina(string tipoVoto)
        {
            if (SesionUsuario.Rol == "VISITANTE")
            {
                MessageBox.Show("Los visitantes no pueden votar.\nInicia sesión para participar.",
                    "Acción no disponible", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (MySqlConnection conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();

                    // Buscar voto previo del usuario
                    string votoPrevio = null;
                    using (MySqlCommand cmd = new MySqlCommand(
                        "SELECT Tipo_Voto FROM tbl_WikiUnach_VotosUsuario " +
                        "WHERE ID_Usuario=@u AND ID_Pagina=@p", conn))
                    {
                        cmd.Parameters.AddWithValue("@u", SesionUsuario.ID_Usuario);
                        cmd.Parameters.AddWithValue("@p", ID_Pagina);
                        object r = cmd.ExecuteScalar();
                        if (r != null && r != DBNull.Value) votoPrevio = r.ToString();
                    }

                    using (MySqlTransaction tx = conn.BeginTransaction())
                    {
                        if (votoPrevio == null)
                        {
                            // Nuevo voto
                            using (MySqlCommand cmd = new MySqlCommand(
                                "INSERT INTO tbl_WikiUnach_VotosUsuario " +
                                "(ID_Usuario, ID_Pagina, Tipo_Voto) VALUES (@u,@p,@t)", conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@u", SesionUsuario.ID_Usuario);
                                cmd.Parameters.AddWithValue("@p", ID_Pagina);
                                cmd.Parameters.AddWithValue("@t", tipoVoto);
                                cmd.ExecuteNonQuery();
                            }
                            AjustarContador(conn, tx, tipoVoto, +1);
                        }
                        else if (votoPrevio == tipoVoto)
                        {
                            // Quitar el voto (toggle off)
                            using (MySqlCommand cmd = new MySqlCommand(
                                "DELETE FROM tbl_WikiUnach_VotosUsuario " +
                                "WHERE ID_Usuario=@u AND ID_Pagina=@p", conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@u", SesionUsuario.ID_Usuario);
                                cmd.Parameters.AddWithValue("@p", ID_Pagina);
                                cmd.ExecuteNonQuery();
                            }
                            AjustarContador(conn, tx, tipoVoto, -1);
                        }
                        else
                        {
                            // Cambiar de like a dislike o viceversa
                            using (MySqlCommand cmd = new MySqlCommand(
                                "UPDATE tbl_WikiUnach_VotosUsuario SET Tipo_Voto=@t " +
                                "WHERE ID_Usuario=@u AND ID_Pagina=@p", conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@t", tipoVoto);
                                cmd.Parameters.AddWithValue("@u", SesionUsuario.ID_Usuario);
                                cmd.Parameters.AddWithValue("@p", ID_Pagina);
                                cmd.ExecuteNonQuery();
                            }
                            AjustarContador(conn, tx, votoPrevio, -1);
                            AjustarContador(conn, tx, tipoVoto,   +1);
                        }
                        tx.Commit();
                    }

                    // Refrescar contador en pantalla
                    using (MySqlCommand cmd = new MySqlCommand(
                        "SELECT Votos_Positivos, Votos_Negativos FROM tbl_WikiUnach_PaginasWiki " +
                        "WHERE ID_Pagina=@p", conn))
                    {
                        cmd.Parameters.AddWithValue("@p", ID_Pagina);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                            if (reader.Read())
                                ActualizarContadorVotos(
                                    Convert.ToInt32(reader["Votos_Positivos"]),
                                    Convert.ToInt32(reader["Votos_Negativos"]));
                    }
                }

                // Notificar al autor solo en LIKE (no spam con dislikes)
                if (tipoVoto == "LIKE")
                    NotificarAutorPagina(
                        "LIKE",
                        $"👍 A {SesionUsuario.Nombre} le gustó tu tarea \"{lblTituloTarea.Text}\"");
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al registrar el voto:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>Busca el autor original de la página (primera revisión) y le crea una notificación.</summary>
        private void NotificarAutorPagina(string tipo, string mensaje)
        {
            try
            {
                using (var conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();
                    string sql = @"SELECT r.ID_Usuario
                                   FROM tbl_WikiUnach_RevisionesWiki r
                                   WHERE r.ID_Pagina = @id
                                   ORDER BY r.Fecha_Creacion ASC
                                   LIMIT 1";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", ID_Pagina);
                        object res = cmd.ExecuteScalar();
                        if (res != null && res != DBNull.Value)
                            Notificaciones.Crear(res.ToString(), tipo, mensaje, ID_Pagina);
                    }
                }
            }
            catch { /* no crítico */ }
        }

        private void AjustarContador(MySqlConnection conn, MySqlTransaction tx,
                                     string tipoVoto, int delta)
        {
            string columna = tipoVoto == "LIKE" ? "Votos_Positivos" : "Votos_Negativos";
            string signo   = delta >= 0 ? "+" : "-";
            string sql = $"UPDATE tbl_WikiUnach_PaginasWiki SET {columna} = " +
                         $"GREATEST({columna} {signo} 1, 0) WHERE ID_Pagina=@p";
            using (MySqlCommand cmd = new MySqlCommand(sql, conn, tx))
            {
                cmd.Parameters.AddWithValue("@p", ID_Pagina);
                cmd.ExecuteNonQuery();
            }
        }

        // ── BOOKMARK ──────────────────────────────────────────────────────────────
        private void CargarEstadoBookmark()
        {
            if (string.IsNullOrEmpty(SesionUsuario.ID_Usuario))
            {
                btnBookmark.Text    = "🔒 Bookmark";
                btnBookmark.Enabled = false;
                return;
            }

            try
            {
                using (MySqlConnection conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(
                        "SELECT 1 FROM tbl_WikiUnach_Bookmarks " +
                        "WHERE ID_Usuario=@u AND ID_Pagina=@p", conn))
                    {
                        cmd.Parameters.AddWithValue("@u", SesionUsuario.ID_Usuario);
                        cmd.Parameters.AddWithValue("@p", ID_Pagina);
                        _esBookmarked = cmd.ExecuteScalar() != null;
                    }
                }
                ActualizarVisualBookmark();
            }
            catch { /* si la tabla no existe aún, ignorar */ }
        }

        private void ActualizarVisualBookmark()
        {
            btnBookmark.Text      = _esBookmarked ? "★ Guardado" : "☆ Guardar";
            btnBookmark.BackColor = _esBookmarked ? Color.Gold   : Color.WhiteSmoke;
            btnBookmark.ForeColor = _esBookmarked ? Color.Black  : Color.FromArgb(80, 80, 80);
        }

        private void AlternarBookmark()
        {
            if (SesionUsuario.Rol == "VISITANTE" || string.IsNullOrEmpty(SesionUsuario.ID_Usuario))
            {
                MessageBox.Show("Inicia sesión para guardar tareas.", "Acción no disponible",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                using (MySqlConnection conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();
                    if (_esBookmarked)
                    {
                        using (MySqlCommand cmd = new MySqlCommand(
                            "DELETE FROM tbl_WikiUnach_Bookmarks " +
                            "WHERE ID_Usuario=@u AND ID_Pagina=@p", conn))
                        {
                            cmd.Parameters.AddWithValue("@u", SesionUsuario.ID_Usuario);
                            cmd.Parameters.AddWithValue("@p", ID_Pagina);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        using (MySqlCommand cmd = new MySqlCommand(
                            "INSERT IGNORE INTO tbl_WikiUnach_Bookmarks " +
                            "(ID_Usuario, ID_Pagina) VALUES (@u, @p)", conn))
                        {
                            cmd.Parameters.AddWithValue("@u", SesionUsuario.ID_Usuario);
                            cmd.Parameters.AddWithValue("@p", ID_Pagina);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                _esBookmarked = !_esBookmarked;
                ActualizarVisualBookmark();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al guardar:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── CARGA DE ARCHIVOS REALES ──────────────────────────────────────────────
        private void CargarArchivos()
        {
            flpArchivosSlide.Controls.Clear();
            int anchoItem = flpArchivosSlide.ClientSize.Width - 24;
            if (anchoItem < 150) anchoItem = 200;

            // En modo edición: botón para adjuntar nuevo archivo
            if (_modoEdicion)
            {
                Button btnAgregarArchivo = new Button
                {
                    Text      = "➕  Adjuntar archivo...",
                    Size      = new Size(anchoItem, 36),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(230, 244, 255),
                    ForeColor = Color.FromArgb(30, 100, 210),
                    Font      = new Font("Segoe UI", 9, FontStyle.Bold),
                    Cursor    = Cursors.Hand,
                    Margin    = new Padding(0, 0, 0, 8)
                };
                btnAgregarArchivo.FlatAppearance.BorderColor = Color.FromArgb(180, 210, 245);
                btnAgregarArchivo.FlatAppearance.BorderSize  = 1;
                btnAgregarArchivo.Click += (s, e) =>
                {
                    using (OpenFileDialog dlg = new OpenFileDialog())
                    {
                        dlg.Title  = "Seleccionar archivo para adjuntar";
                        dlg.Filter = "Todos los archivos (*.*)|*.*";
                        if (dlg.ShowDialog() == DialogResult.OK)
                        {
                            string ruta      = dlg.FileName;
                            string nombre    = Path.GetFileNameWithoutExtension(ruta);
                            string extension = Path.GetExtension(ruta);
                            long   tamano    = new FileInfo(ruta).Length;

                            var nuevoArch = new FrmRevisiones.ArchivoNuevo
                            {
                                RutaLocal   = ruta,
                                Nombre      = nombre,
                                Descripcion = "",
                                TamanoBytes = tamano,
                                Extension   = extension
                            };
                            _archivosNuevos.Add(nuevoArch);

                            // ── Panel "pendiente de subir" ──────────────────────
                            Panel pendiente = new Panel
                            {
                                Size        = new Size(anchoItem, 52),
                                BackColor   = Color.FromArgb(240, 248, 255),
                                Margin      = new Padding(0, 0, 0, 4),
                                BorderStyle = BorderStyle.FixedSingle
                            };

                            Image iconoPend = ObtenerImagenPorExtension(extension);
                            PictureBox pbPend = new PictureBox
                            {
                                Location  = new Point(6, 8),
                                Size      = new Size(34, 34),
                                SizeMode  = PictureBoxSizeMode.Zoom,
                                Image     = iconoPend,
                                BackColor = Color.Transparent
                            };

                            Label lblNomPend = new Label
                            {
                                Text      = nombre + extension,
                                Location  = new Point(48, 6),
                                Size      = new Size(anchoItem - 90, 18),
                                Font      = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                                ForeColor = Color.FromArgb(30, 60, 130)
                            };

                            Label lblEstPend = new Label
                            {
                                Text      = "📎 Listo — se subirá al guardar la revisión",
                                Location  = new Point(48, 26),
                                Size      = new Size(anchoItem - 90, 16),
                                Font      = new Font("Segoe UI", 7.5f),
                                ForeColor = Color.FromArgb(80, 110, 180)
                            };

                            // Botón X para quitar el archivo antes de guardar
                            var capArch   = nuevoArch;
                            var capPanel  = pendiente;
                            Button btnQuitarPend = new Button
                            {
                                Text      = "✕",
                                Location  = new Point(anchoItem - 30, 14),
                                Size      = new Size(22, 22),
                                FlatStyle = FlatStyle.Flat,
                                Font      = new Font("Segoe UI", 8, FontStyle.Bold),
                                ForeColor = Color.FromArgb(160, 40, 40),
                                BackColor = Color.Transparent,
                                Cursor    = Cursors.Hand,
                                TabStop   = false
                            };
                            btnQuitarPend.FlatAppearance.BorderSize = 0;
                            btnQuitarPend.Click += (sv, ev) =>
                            {
                                _archivosNuevos.Remove(capArch);
                                flpArchivosSlide.Controls.Remove(capPanel);
                            };

                            pendiente.Controls.Add(pbPend);
                            pendiente.Controls.Add(lblNomPend);
                            pendiente.Controls.Add(lblEstPend);
                            pendiente.Controls.Add(btnQuitarPend);
                            flpArchivosSlide.Controls.Add(pendiente);
                        }
                    }
                };
                flpArchivosSlide.Controls.Add(btnAgregarArchivo);
            }

            try
            {
                using (MySqlConnection conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();

                    // Archivo principal de la página + archivos adjuntos en comentarios
                    string sql = @"
                        SELECT a.ID_Archivo, a.Titulo, a.Descripcion, a.URL_Archivo,
                               a.Tipo_Archivo, a.Tamano_Bytes, a.Fecha_Creacion
                        FROM tbl_WikiUnach_Archivos a
                        WHERE a.ID_Archivo = (
                            SELECT ID_Archivo_Principal
                            FROM tbl_WikiUnach_PaginasWiki
                            WHERE ID_Pagina = @id1
                              AND ID_Archivo_Principal IS NOT NULL)
                        UNION
                        SELECT a.ID_Archivo, a.Titulo, a.Descripcion, a.URL_Archivo,
                               a.Tipo_Archivo, a.Tamano_Bytes, a.Fecha_Creacion
                        FROM tbl_WikiUnach_Archivos a
                        INNER JOIN tbl_WikiUnach_Comentarios c
                                ON a.ID_Archivo = c.ID_Archivo_Adjunto
                        WHERE c.ID_Pagina = @id2";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id1", ID_Pagina);
                        cmd.Parameters.AddWithValue("@id2", ID_Pagina);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string ext        = "." + reader["Tipo_Archivo"].ToString().TrimStart('.');
                                Image  icono      = ObtenerImagenPorExtension(ext);
                                string urlArchivo = reader["URL_Archivo"].ToString();
                                string idArchivo  = reader["ID_Archivo"].ToString();

                                Panel item = CrearItemArchivo(
                                    reader["Titulo"].ToString(),
                                    reader["Descripcion"].ToString(),
                                    Convert.ToInt64(reader["Tamano_Bytes"]),
                                    Convert.ToDateTime(reader["Fecha_Creacion"]),
                                    icono, anchoItem, urlArchivo, idArchivo);

                                flpArchivosSlide.Controls.Add(item);
                            }
                        }
                    }
                }

                if (flpArchivosSlide.Controls.Count == 0)
                    flpArchivosSlide.Controls.Add(new Label
                    {
                        Text      = "No hay archivos adjuntos.",
                        AutoSize  = true,
                        Margin    = new Padding(15, 20, 0, 0),
                        ForeColor = Color.Gray
                    });

                Tema.AplicarA(flpArchivosSlide);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al cargar archivos:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Crea el panel visual de un archivo en la lista.
        /// </summary>
        private Panel CrearItemArchivo(string titulo, string descripcion,
                                       long tamanoBytes, DateTime fecha,
                                       Image icono, int ancho, string urlArchivo,
                                       string idArchivo = null)
        {
            int altoPnl = _modoEdicion ? 108 : 88;   // altura extra para botón eliminar
            Panel pnlItem = new Panel
            {
                Size      = new Size(ancho, altoPnl),
                BackColor = Color.White,
                Cursor    = Cursors.Hand,
                Margin    = new Padding(0, 0, 0, 6)
            };

            PictureBox pb = new PictureBox
            {
                Location  = new Point(8, 12),
                Size      = new Size(48, 48),
                SizeMode  = PictureBoxSizeMode.Zoom,
                Image     = icono,
                BackColor = Color.FromArgb(245, 245, 245)
            };

            Label lblNombre = new Label
            {
                Text      = titulo,
                Location  = new Point(66, 10),
                Size      = new Size(ancho - 76, 22),
                Font      = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30)
            };

            Label lblFecha = new Label
            {
                Text      = fecha.ToString("dd/MM/yyyy"),
                Location  = new Point(66, 34),
                AutoSize  = true,
                Font      = new Font("Segoe UI", 7.5f),
                ForeColor = Color.FromArgb(160, 160, 160)
            };

            // Botón descargar
            Button btnDescargar = new Button
            {
                Text      = "📥 Descargar",
                Location  = new Point(66, 56),
                Size      = new Size(105, 24),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(230, 244, 255),
                ForeColor = Color.FromArgb(30, 100, 210),
                Font      = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnDescargar.FlatAppearance.BorderColor = Color.FromArgb(180, 210, 245);
            btnDescargar.FlatAppearance.BorderSize  = 1;
            btnDescargar.Click += (s, e) => DescargarArchivo(urlArchivo, titulo);

            // ── Botón Eliminar (solo en modo edición) ────────────────────────────
            if (_modoEdicion && !string.IsNullOrEmpty(idArchivo))
            {
                string capId    = idArchivo;
                string capUrl   = urlArchivo;
                string capNom   = titulo;
                Panel  capPanel = pnlItem;

                Button btnElim = new Button
                {
                    Text      = "🗑 Eliminar al guardar",
                    Location  = new Point(66, 78),
                    Size      = new Size(160, 22),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(255, 240, 240),
                    ForeColor = Color.FromArgb(200, 50, 50),
                    Font      = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                    Cursor    = Cursors.Hand
                };
                btnElim.FlatAppearance.BorderColor = Color.FromArgb(220, 150, 150);
                btnElim.FlatAppearance.BorderSize  = 1;
                btnElim.Click += (s, e) =>
                {
                    // ¿Ya estaba marcado para eliminar?
                    bool yaMarcado = _archivosEliminar.Exists(a => a.IdArchivo == capId);
                    if (yaMarcado)
                    {
                        // Desmarcar
                        _archivosEliminar.RemoveAll(a => a.IdArchivo == capId);
                        capPanel.BackColor        = Color.White;
                        ((Button)s).Text          = "🗑 Eliminar al guardar";
                        ((Button)s).BackColor     = Color.FromArgb(255, 240, 240);
                        ((Button)s).ForeColor     = Color.FromArgb(200, 50, 50);
                    }
                    else
                    {
                        // Marcar para eliminar
                        _archivosEliminar.Add(new FrmRevisiones.ArchivoEliminar
                        {
                            IdArchivo = capId,
                            UrlS3     = capUrl,
                            Nombre    = capNom
                        });
                        capPanel.BackColor        = Color.FromArgb(255, 235, 235);
                        ((Button)s).Text          = "↩ Deshacer eliminación";
                        ((Button)s).BackColor     = Color.FromArgb(255, 220, 220);
                        ((Button)s).ForeColor     = Color.FromArgb(140, 30, 30);
                    }
                };
                pnlItem.Controls.Add(btnElim);
            }

            // Separador inferior del item
            Panel linea = new Panel
            {
                Location  = new Point(0, altoPnl - 2),
                Size      = new Size(ancho, 1),
                BackColor = Color.FromArgb(235, 235, 235)
            };

            pnlItem.Controls.Add(pb);
            pnlItem.Controls.Add(lblNombre);
            pnlItem.Controls.Add(lblFecha);
            pnlItem.Controls.Add(btnDescargar);
            pnlItem.Controls.Add(linea);

            // Efecto hover
            EventHandler mouseEnter = (s, e2) => pnlItem.BackColor = Color.FromArgb(240, 247, 255);
            EventHandler mouseLeave = (s, e2) => pnlItem.BackColor = Color.White;
            pnlItem.MouseEnter   += mouseEnter;
            pb.MouseEnter        += mouseEnter;
            lblNombre.MouseEnter += mouseEnter;
            lblFecha.MouseEnter  += mouseEnter;
            pnlItem.MouseLeave   += mouseLeave;
            pb.MouseLeave        += mouseLeave;
            lblNombre.MouseLeave += mouseLeave;
            lblFecha.MouseLeave  += mouseLeave;

            // Click en el panel/ícono/labels: abrir FrmInfoArchivo
            EventHandler click = (s, e2) =>
            {
                string urlFirma = GenerarUrlFirmada(urlArchivo);
                if (urlFirma == null) return;

                using (FrmInfoArchivo info = new FrmInfoArchivo(
                    titulo, descripcion, tamanoBytes, fecha, icono, urlFirma))
                {
                    info.ShowDialog(this);
                }
            };
            pnlItem.Click    += click;
            pb.Click         += click;
            lblNombre.Click  += click;
            lblFecha.Click   += click;

            return pnlItem;
        }

        // ── HELPERS S3 ───────────────────────────────────────────────────────────

        /// <summary>
        /// Extrae la clave (key) del objeto S3 a partir de la URL almacenada en la BD.
        /// Ejemplo: "https://wikiunach-archivos.s3.amazonaws.com/tareas/file.pdf"
        ///           → "tareas/file.pdf"
        /// </summary>
        private string ExtraerKey(string url)
        {
            try { return new Uri(url).AbsolutePath.TrimStart('/'); }
            catch { return url; }
        }

        /// <summary>
        /// Genera una URL pre-firmada de descarga (válida 15 minutos).
        /// Devuelve null si ocurre algún error.
        /// </summary>
        private string GenerarUrlFirmada(string urlArchivo)
        {
            try
            {
                string key = ExtraerKey(urlArchivo);
                return S3Service.ObtenerUrlDescarga(key, 15);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo generar el enlace de descarga:\n" + ex.Message,
                    "Error S3", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// Genera URL pre-firmada y descarga el archivo en la carpeta Descargas del usuario.
        /// </summary>
        private void DescargarArchivo(string urlArchivo, string titulo)
        {
            string urlFirma = GenerarUrlFirmada(urlArchivo);
            if (urlFirma == null) return;

            try
            {
                string carpeta   = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string descargas = Path.Combine(carpeta, "Downloads");
                if (!Directory.Exists(descargas)) descargas = carpeta;

                // Usar extensión original del key
                string key      = ExtraerKey(urlArchivo);
                string ext      = Path.GetExtension(key);
                string nombreLimpio = string.Concat(titulo.Split(Path.GetInvalidFileNameChars()));
                string destino  = Path.Combine(descargas, nombreLimpio + ext);

                // Si ya existe, agregar sufijo numérico
                int n = 1;
                while (File.Exists(destino))
                    destino = Path.Combine(descargas, $"{nombreLimpio}_{n++}{ext}");

                using (WebClient wc = new WebClient())
                    wc.DownloadFile(urlFirma, destino);

                MessageBox.Show($"Archivo guardado en:\n{destino}",
                    "Descarga completada", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al descargar el archivo:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Devuelve la imagen correspondiente a la extensión del archivo.
        /// Si no hay coincidencia usa txt.png como ícono por defecto.
        /// </summary>
        private Image ObtenerImagenPorExtension(string extension)
        {
            string ext = extension.ToLower().TrimStart('.');
            string archivo;

            switch (ext)
            {
                case "cs":                                      archivo = "C#.png";         break;
                case "cpp": case "cc": case "h": case "hpp":   archivo = "C++.png";        break;
                case "xlsx": case "xls":                        archivo = "excel.png";      break;
                case "java":                                    archivo = "java.png";       break;
                case "mp3": case "wav": case "ogg":             archivo = "mp3.png";        break;
                case "mp4": case "avi": case "mov": case "mkv": archivo = "mp4.png";        break;
                case "mwb":                                     archivo = "mwb.png";        break;
                case "pdf":                                     archivo = "pdf.png";        break;
                case "pptx": case "ppt":                        archivo = "powerpoint.png"; break;
                case "py":                                      archivo = "python.png";     break;
                case "rar":                                     archivo = "rar.png";        break;
                case "docx": case "doc":                        archivo = "word.png";       break;
                case "zip": case "7z":                          archivo = "zip.png";        break;
                default:                                        archivo = "txt.png";        break;
            }

            string ruta = ResolverRutaIcono(archivo);
            if (ruta != null) return Image.FromFile(ruta);

            // Fallback a txt.png
            string rutaDefault = ResolverRutaIcono("txt.png");
            return rutaDefault != null ? Image.FromFile(rutaDefault) : null;
        }

        // ── CARGA DE COMENTARIOS REALES ──────────────────────────────────────────
        private void CargarComentarios()
        {
            flpComentariosSlide.Controls.Clear();
            int anchoItem = flpComentariosSlide.ClientSize.Width - 30;

            try
            {
                using (MySqlConnection conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();

                    string sql = @"
                        SELECT c.ID_Comentario, c.Contenido, c.Fecha_Creacion,
                               u.Nombre, u.ID_Usuario AS IdAutorComentario
                        FROM tbl_WikiUnach_Comentarios c
                        INNER JOIN tbl_WikiUnach_Usuarios u
                                ON c.ID_Usuario = u.ID_Usuario
                        WHERE c.ID_Pagina = @idPagina
                        ORDER BY c.Fecha_Creacion DESC";

                    bool esAdmin = SesionUsuario.Rol == "ADMINISTRADOR";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@idPagina", ID_Pagina);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string idComentario = reader["ID_Comentario"].ToString();
                                string idAutorCom   = reader["IdAutorComentario"].ToString();
                                UCComentarios comentario = new UCComentarios
                                {
                                    Usuario = reader["Nombre"].ToString(),
                                    Fecha   = Convert.ToDateTime(reader["Fecha_Creacion"]).ToString("dd/MM/yyyy"),
                                    Texto   = reader["Contenido"].ToString(),
                                    Width   = anchoItem
                                };

                                // Hacer el nombre clickable → abre el perfil
                                string capIdAutor = idAutorCom;
                                foreach (Control c in comentario.Controls)
                                {
                                    if (c is Label lbl && lbl.Text == comentario.Usuario)
                                    {
                                        lbl.Cursor = Cursors.Hand;
                                        lbl.Click += (s, ev) =>
                                            new FrmCuenta(capIdAutor).ShowDialog(this);
                                        break;
                                    }
                                }

                                // Botón "🗑" sobreimpuesto solo para admin
                                if (esAdmin)
                                {
                                    string capId = idComentario;
                                    UCComentarios capCom = comentario;
                                    Button btnElim = new Button
                                    {
                                        Text      = "🗑",
                                        Size      = new Size(28, 24),
                                        Location  = new Point(comentario.Width - 36, 4),
                                        FlatStyle = FlatStyle.Flat,
                                        BackColor = Color.FromArgb(255, 235, 235),
                                        ForeColor = Color.FromArgb(180, 30, 30),
                                        Font      = new Font("Segoe UI", 8, FontStyle.Bold),
                                        Cursor    = Cursors.Hand,
                                        TabStop   = false
                                    };
                                    btnElim.FlatAppearance.BorderColor = Color.FromArgb(220, 150, 150);
                                    btnElim.FlatAppearance.BorderSize  = 1;
                                    btnElim.Click += (s, ev) => EliminarComentarioAdmin(capId, capCom);
                                    comentario.Controls.Add(btnElim);
                                    btnElim.BringToFront();
                                }

                                flpComentariosSlide.Controls.Add(comentario);
                            }
                        }
                    }
                }

                if (flpComentariosSlide.Controls.Count == 0)
                    flpComentariosSlide.Controls.Add(new Label
                    {
                        Text      = "Sé el primero en comentar.",
                        AutoSize  = true,
                        Margin    = new Padding(15, 20, 0, 0),
                        ForeColor = Color.Gray
                    });

                Tema.AplicarA(flpComentariosSlide);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al cargar comentarios:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEnviarComentario_Click(object sender, EventArgs e)
        {
            if (SesionUsuario.Rol == "VISITANTE")
            {
                MessageBox.Show(
                    "Los visitantes no pueden comentar.\nInicia sesión para participar.",
                    "Acción no disponible",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            string texto = txtNuevoComentarioSlide.Text.Trim();

            if (string.IsNullOrEmpty(texto) || txtNuevoComentarioSlide.ForeColor == Color.Gray)
            {
                MessageBox.Show("Escribe un comentario antes de enviar.",
                    "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();

                    string sql = @"
                        INSERT INTO tbl_WikiUnach_Comentarios
                            (ID_Comentario, ID_Pagina, ID_Usuario, Contenido)
                        VALUES
                            (@id, @idPagina, @idUsuario, @contenido)";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id",        Guid.NewGuid().ToString());
                        cmd.Parameters.AddWithValue("@idPagina",  ID_Pagina);
                        cmd.Parameters.AddWithValue("@idUsuario", SesionUsuario.ID_Usuario);
                        cmd.Parameters.AddWithValue("@contenido", texto);
                        cmd.ExecuteNonQuery();
                    }
                }

                // Limpiar textbox y recargar lista
                txtNuevoComentarioSlide.Text      = "Escribe un comentario...";
                txtNuevoComentarioSlide.ForeColor = Color.Gray;
                CargarComentarios();

                // Notificar al autor de la página
                NotificarAutorPagina(
                    "COMENTARIO",
                    $"💬 {SesionUsuario.Nombre} comentó tu tarea \"{lblTituloTarea.Text}\"");
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al enviar el comentario:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── ELIMINAR COMENTARIO (solo admin) ─────────────────────────────────────
        private void EliminarComentarioAdmin(string idComentario, UCComentarios comentario)
        {
            if (MessageBox.Show("¿Eliminar este comentario?\nEsta acción no se puede deshacer.",
                "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                != DialogResult.Yes) return;

            try
            {
                using (MySqlConnection conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(
                        "DELETE FROM tbl_WikiUnach_Comentarios WHERE ID_Comentario=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idComentario);
                        cmd.ExecuteNonQuery();
                    }
                }
                flpComentariosSlide.Controls.Remove(comentario);
                comentario.Dispose();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al eliminar:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── NAVEGACIÓN ───────────────────────────────────────────────────────────
        private void btnRegresar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRevisiones_Click(object sender, EventArgs e)
        {
            if (!_modoEdicion)
            {
                if (revisionesAbierto) CerrarRevisiones();
                else                   AbrirRevisiones();
                return;
            }

            // ── Recopilar cambios ────────────────────────────────────────────────
            string contenidoNuevo = richTextBox1.Text;

            var tagsAgregados  = _tagsActuales.Except(_tagsOriginales).ToList();
            var tagsEliminados = _tagsOriginales.Except(_tagsActuales).ToList();

            bool hayCambios = !string.Equals(_contenidoOriginal?.Trim(),
                                             contenidoNuevo?.Trim(),
                                             StringComparison.Ordinal)
                              || tagsAgregados.Count    > 0
                              || tagsEliminados.Count   > 0
                              || _archivosNuevos.Count  > 0
                              || _archivosEliminar.Count > 0;

            if (!hayCambios)
            {
                MessageBox.Show("No has realizado ningún cambio.",
                    "Sin cambios", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var datos = new FrmRevisiones.DatosRevision
            {
                IdPagina          = ID_Pagina,
                IdMateria         = _idMateriaActual,
                TituloPagina      = lblTituloTarea.Text,
                ContenidoNuevo    = contenidoNuevo,
                ContenidoAnterior = _contenidoOriginal,
                TagsAgregados     = tagsAgregados,
                TagsEliminados    = tagsEliminados,
                ArchivosNuevos    = new List<FrmRevisiones.ArchivoNuevo>(_archivosNuevos),
                ArchivosEliminar  = new List<FrmRevisiones.ArchivoEliminar>(_archivosEliminar)
            };

            using (FrmRevisiones frm = new FrmRevisiones(datos))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                {
                    // Revisión guardada → restaurar modo visualización
                    _archivosNuevos.Clear();
                    _archivosEliminar.Clear();
                    _modoEdicion           = false;
                    btnRevisiones.Text     = "Revisiones";
                    richTextBox1.ReadOnly  = true;
                    richTextBox1.BackColor = Color.White;
                    lbltags.Visible        = true;

                    if (_flpTags != null)
                    {
                        pnlHeaderDetalle.Controls.Remove(_flpTags);
                        _flpTags.Dispose();
                        _flpTags = null;
                    }

                    CargarPagina();   // refresca datos desde BD
                }
            }
        }

        // ── MODO EDICIÓN: configuración visual ───────────────────────────────────
        private void ConfigurarModoEdicion()
        {
            btnRevisiones.Text     = "Subir Revisión";
            richTextBox1.ReadOnly  = false;
            richTextBox1.BackColor = Color.FromArgb(255, 255, 245);
            AgregarControlesTagEdicion();
        }

        private void AgregarControlesTagEdicion()
        {
            lbltags.Visible = false;

            _flpTags = new FlowLayoutPanel
            {
                Location      = new Point(lbltags.Left, lbltags.Top - 6),
                Size          = new Size(pnlHeaderDetalle.Width - lbltags.Left - 10, 52),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents  = true,
                AutoSize      = false,
                BackColor     = pnlHeaderDetalle.BackColor
            };

            // Crear pill por cada tag actual
            foreach (string tag in _tagsActuales.ToList())
                _flpTags.Controls.Add(CrearPillTag(tag));

            // TextBox para nueva etiqueta
            TextBox txtTagInput = new TextBox
            {
                Size      = new Size(90, 22),
                Font      = new Font("Segoe UI", 8.5f),
                Margin    = new Padding(2, 4, 2, 0)
            };

            // Botón "+"
            Button btnAgregar = new Button
            {
                Text      = "+",
                Size      = new Size(24, 22),
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(30, 100, 210),
                ForeColor = Color.White,
                Cursor    = Cursors.Hand,
                Margin    = new Padding(2, 4, 2, 0)
            };
            btnAgregar.FlatAppearance.BorderSize = 0;
            btnAgregar.Click += (s, e) =>
            {
                string tagNuevo = txtTagInput.Text.Trim().TrimStart('#');
                if (string.IsNullOrEmpty(tagNuevo)) return;

                string tagConHash = "#" + tagNuevo;
                if (!_tagsActuales.Contains(tagConHash))
                {
                    _tagsActuales.Add(tagConHash);
                    // Insertar pill antes del txtTagInput
                    int idx = _flpTags.Controls.IndexOf(txtTagInput);
                    Panel pill = CrearPillTag(tagConHash);
                    _flpTags.Controls.Add(pill);
                    _flpTags.Controls.SetChildIndex(pill, idx);
                }
                txtTagInput.Text = "";
                txtTagInput.Focus();
            };

            _flpTags.Controls.Add(txtTagInput);
            _flpTags.Controls.Add(btnAgregar);

            pnlHeaderDetalle.Controls.Add(_flpTags);
            _flpTags.BringToFront();
        }

        private Panel CrearPillTag(string tag)
        {
            Panel pill = new Panel
            {
                Size      = new Size(82, 22),
                BackColor = Color.FromArgb(220, 235, 255),
                Margin    = new Padding(2, 4, 2, 0)
            };

            Label lblTag = new Label
            {
                Text      = tag,
                AutoSize  = false,
                Size      = new Size(60, 22),
                Location  = new Point(2, 2),
                Font      = new Font("Segoe UI", 7.5f),
                ForeColor = Color.FromArgb(30, 60, 130)
            };

            Button btnX = new Button
            {
                Text      = "✕",
                Location  = new Point(62, 1),
                Size      = new Size(18, 20),
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 7, FontStyle.Bold),
                ForeColor = Color.FromArgb(130, 130, 130),
                BackColor = Color.Transparent,
                Cursor    = Cursors.Hand,
                TabStop   = false
            };
            btnX.FlatAppearance.BorderSize = 0;

            string capTag  = tag;
            Panel  capPill = pill;
            btnX.Click += (s, e) =>
            {
                _tagsActuales.Remove(capTag);
                if (_flpTags != null)
                    _flpTags.Controls.Remove(capPill);
            };

            pill.Controls.Add(lblTag);
            pill.Controls.Add(btnX);
            return pill;
        }

        // ── PANEL LATERAL DE REVISIONES ──────────────────────────────────────────
        private void CrearPanelRevisiones()
        {
            const int ANCHO = 340;
            int yOrigen   = pnlCuerpoDetalle.Top;
            int altoSlide = this.ClientSize.Height - yOrigen;

            pnlRevisionesSlide = new Panel
            {
                Size        = new Size(ANCHO, altoSlide),
                Location    = new Point(this.ClientSize.Width, yOrigen), // fuera de pantalla
                BackColor   = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Visible     = false
            };

            // ── Header ───────────────────────────────────────────────────────────
            Panel pnlHeader = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 48,
                BackColor = Color.FromArgb(35, 55, 95)
            };

            Label lblTitulo = new Label
            {
                Text      = "Historial de Revisiones",
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 11, FontStyle.Bold),
                AutoSize  = true,
                Location  = new Point(15, 12)
            };

            Button btnCerrar = new Button
            {
                Text      = "✕",
                ForeColor = Color.White,
                BackColor = Color.FromArgb(35, 55, 95),
                FlatStyle = FlatStyle.Flat,
                Size      = new Size(40, 40),
                Location  = new Point(ANCHO - 50, 4),
                Cursor    = Cursors.Hand,
                Font      = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 50, 50);
            btnCerrar.Click += (s, e) => CerrarRevisiones();

            pnlHeader.Controls.Add(lblTitulo);
            pnlHeader.Controls.Add(btnCerrar);

            // ── Lista scrolleable ────────────────────────────────────────────────
            flpRevisionesSlide = new FlowLayoutPanel
            {
                Dock          = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll    = true,
                WrapContents  = false,
                BackColor     = Color.White,
                Padding       = new Padding(10, 8, 10, 8)
            };

            // Orden Fill → Top
            pnlRevisionesSlide.Controls.Add(flpRevisionesSlide);
            pnlRevisionesSlide.Controls.Add(pnlHeader);

            this.Controls.Add(pnlRevisionesSlide);
            pnlRevisionesSlide.BringToFront();

            timerRevisiones      = new System.Windows.Forms.Timer { Interval = 12 };
            timerRevisiones.Tick += TimerRevisiones_Tick;
        }

        private void AbrirRevisiones()
        {
            const int ANCHO = 340;
            pnlRevisionesSlide.Size     = new Size(ANCHO, this.ClientSize.Height - pnlCuerpoDetalle.Top);
            pnlRevisionesSlide.Location = new Point(this.ClientSize.Width, pnlCuerpoDetalle.Top);
            pnlRevisionesSlide.Visible  = true;
            pnlRevisionesSlide.BringToFront();
            revisionesAbierto    = true;
            timerRevisiones.Tag  = this.ClientSize.Width - ANCHO;
            timerRevisiones.Start();
            CargarRevisionesLista();
        }

        private void CerrarRevisiones()
        {
            revisionesAbierto   = false;
            timerRevisiones.Tag = this.ClientSize.Width;
            timerRevisiones.Start();
        }

        private void TimerRevisiones_Tick(object sender, EventArgs e)
        {
            int target  = (int)timerRevisiones.Tag;
            int current = pnlRevisionesSlide.Left;
            int paso    = (target - current) / 4;

            if (Math.Abs(paso) < 8)
            {
                pnlRevisionesSlide.Left = target;
                timerRevisiones.Stop();
                if (!revisionesAbierto) pnlRevisionesSlide.Visible = false;
            }
            else
            {
                pnlRevisionesSlide.Left += paso;
            }
        }

        // ── CARGA DE LA LISTA DE REVISIONES ──────────────────────────────────────
        private void CargarRevisionesLista()
        {
            flpRevisionesSlide.Controls.Clear();
            int anchoItem = flpRevisionesSlide.ClientSize.Width - 24;
            if (anchoItem < 200) anchoItem = 300;

            try
            {
                using (MySqlConnection conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();
                    string sql = @"
                        SELECT r.ID_Revision, r.Mensaje_Commit,
                               r.Instantanea_Contenido, r.Fecha_Creacion,
                               u.Nombre    AS NombreUsuario,
                               pw.Titulo   AS TituloPagina
                        FROM   tbl_WikiUnach_RevisionesWiki r
                        INNER JOIN tbl_WikiUnach_Usuarios    u  ON u.ID_Usuario  = r.ID_Usuario
                        INNER JOIN tbl_WikiUnach_PaginasWiki pw ON pw.ID_Pagina  = r.ID_Pagina
                        WHERE  r.ID_Pagina = @idPagina
                        ORDER  BY r.Fecha_Creacion DESC";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@idPagina", ID_Pagina);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                flpRevisionesSlide.Controls.Add(
                                    CrearItemRevision(
                                        reader["ID_Revision"].ToString(),
                                        reader["TituloPagina"].ToString(),
                                        reader["Mensaje_Commit"].ToString(),
                                        reader["Instantanea_Contenido"].ToString(),
                                        Convert.ToDateTime(reader["Fecha_Creacion"]),
                                        reader["NombreUsuario"].ToString(),
                                        anchoItem));
                            }
                        }
                    }
                }

                if (flpRevisionesSlide.Controls.Count == 0)
                    flpRevisionesSlide.Controls.Add(new Label
                    {
                        Text      = "No hay revisiones para esta página.",
                        AutoSize  = true,
                        Margin    = new Padding(15, 20, 0, 0),
                        ForeColor = Color.Gray
                    });

                Tema.AplicarA(flpRevisionesSlide);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al cargar revisiones:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Panel CrearItemRevision(string idRevision, string tituloPagina,
                                         string mensaje,   string instantanea,
                                         DateTime fecha,   string usuario,
                                         int ancho)
        {
            // Preview: primeras 10 letras del mensaje
            string preview = mensaje.Length > 10
                             ? "\"" + mensaje.Substring(0, 10) + "...\""
                             : "\"" + mensaje + "\"";

            Panel pnl = new Panel
            {
                Size      = new Size(ancho, 72),
                BackColor = Color.White,
                Cursor    = Cursors.Hand,
                Margin    = new Padding(0, 0, 0, 4)
            };

            Panel borde = new Panel
            {
                Location  = new Point(0, 0),
                Size      = new Size(4, 72),
                BackColor = Color.FromArgb(35, 55, 95)
            };

            Label lblFecha = new Label
            {
                Text      = fecha.ToString("dd/MM/yyyy   HH:mm"),
                Location  = new Point(14, 8),
                AutoSize  = true,
                Font      = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(40, 40, 40)
            };

            Label lblPreview = new Label
            {
                Text      = preview,
                Location  = new Point(14, 28),
                Size      = new Size(ancho - 20, 18),
                Font      = new Font("Segoe UI", 8f, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 100, 100)
            };

            Label lblUsuario = new Label
            {
                Text      = "👤 " + usuario,
                Location  = new Point(14, 50),
                AutoSize  = true,
                Font      = new Font("Segoe UI", 7.5f),
                ForeColor = Color.FromArgb(140, 140, 140)
            };

            Panel linea = new Panel
            {
                Location  = new Point(0, 70),
                Size      = new Size(ancho, 1),
                BackColor = Color.FromArgb(230, 230, 230)
            };

            pnl.Controls.Add(borde);
            pnl.Controls.Add(lblFecha);
            pnl.Controls.Add(lblPreview);
            pnl.Controls.Add(lblUsuario);
            pnl.Controls.Add(linea);

            // Hover
            EventHandler enter = (s, ev) => pnl.BackColor = Color.FromArgb(238, 244, 255);
            EventHandler leave = (s, ev) => pnl.BackColor = Color.White;
            pnl.MouseEnter     += enter; lblFecha.MouseEnter   += enter;
            lblPreview.MouseEnter += enter; lblUsuario.MouseEnter += enter;
            pnl.MouseLeave     += leave; lblFecha.MouseLeave   += leave;
            lblPreview.MouseLeave += leave; lblUsuario.MouseLeave += leave;

            // Click → abrir FrmRevisiones en modo visualización
            string   capId       = idRevision;
            string   capTitulo   = tituloPagina;
            string   capMensaje  = mensaje;
            string   capInst     = instantanea;
            DateTime capFecha    = fecha;
            string   capUsuario  = usuario;

            EventHandler click = (s, ev) =>
            {
                var rev = new FrmRevisiones.RevisionExistente
                {
                    IdRevision            = capId,
                    TituloPagina          = capTitulo,
                    NombreUsuario         = capUsuario,
                    FechaCreacion         = capFecha,
                    InstantaneaContenido  = capInst,
                    MensajeCommit         = capMensaje
                };
                using (var frm = new FrmRevisiones(rev))
                    frm.ShowDialog(this);
            };

            pnl.Click          += click;
            lblFecha.Click     += click;
            lblPreview.Click   += click;
            lblUsuario.Click   += click;

            return pnl;
        }
    }
}
