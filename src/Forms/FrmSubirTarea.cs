using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WUNACH
{
    public partial class FrmSubirTarea : Form
    {
        // ── CARPETAS DE ÍCONOS DE TIPO DE ARCHIVO ─────────────────────────────────
        // Buscadas EN ORDEN; la primera existente se usa.
        private static readonly string[] RUTAS_ICONOS = new[]
        {
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TiposArchivo"),
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tipos de Archivo"),
            @"C:\Users\luisp\OneDrive\Pictures\WUNACH\Tipos de Archivo"
        };

        private static string ResolverRutaIcono(string nombreArchivo)
        {
            foreach (string carpeta in RUTAS_ICONOS)
            {
                string ruta = Path.Combine(carpeta, nombreArchivo);
                if (File.Exists(ruta)) return ruta;
            }
            return null;
        }

        // ── PANEL LATERAL DE ARCHIVOS ─────────────────────────────────────────────
        private Panel             pnlArchivosSlide;
        private FlowLayoutPanel   flpArchivosSlide;
        private System.Windows.Forms.Timer timerArchivos;
        private bool              archivosAbierto = false;

        // ── DATOS EN MEMORIA ──────────────────────────────────────────────────────
        private readonly List<string>         _tags             = new List<string>();
        private readonly List<ArchivoAdjunto> _archivosAdjuntos = new List<ArchivoAdjunto>();

        // ── CLASES HELPER ─────────────────────────────────────────────────────────
        private class ArchivoAdjunto
        {
            public string RutaLocal   { get; set; }
            public string Nombre      { get; set; }
            public string Descripcion { get; set; }
            public long   TamanoBytes { get; set; }
            public string Extension   { get; set; }
        }

        private class FacultadItem
        {
            public string ID     { get; set; }
            public string Nombre { get; set; }
            public override string ToString() => Nombre;
        }
        private class LicenciaturaItem
        {
            public string ID     { get; set; }
            public string Nombre { get; set; }
            public override string ToString() => Nombre;
        }
        private class SemestreItem
        {
            public string Valor  { get; set; }
            public override string ToString() => "Semestre " + Valor;
        }
        private class MateriaItem
        {
            public string ID     { get; set; }
            public string Nombre { get; set; }
            public override string ToString() => Nombre;
        }

        // ── CONTROLES EXTRA (solo tags — el resto viene del Designer) ─────────────
        private TextBox  txtTagsInput;
        private Button   btnAgregarTag;

        // ── CONSTRUCTOR ───────────────────────────────────────────────────────────
        public FrmSubirTarea()
        {
            InitializeComponent();

            btnarchivo.Click += (s, e) => { if (archivosAbierto) CerrarArchivos(); else AbrirArchivos(); };
            btnSubir.Click   += BtnSubir_Click;

            // Cascada de comboboxes
            cmbFacultad.SelectedIndexChanged    += CmbFacultad_Changed;
            cmbLicenciatura.SelectedIndexChanged += CmbLicenciatura_Changed;
            cmbSemestre.SelectedIndexChanged     += CmbSemestre_Changed;

            this.Load += FrmSubirTarea_Load;
            this.Load += (s, e) =>
            {
                LayoutHelper.AplicarTitulo(this, "Subir Tarea");
                // Solo los campos single-line se enlazan; txtDescripcion es multilínea
                LayoutHelper.EnterPasaAlSiguiente(txtTitulo);
                Tema.AplicarA(this);
            };

            this.MinimumSize = new System.Drawing.Size(1100, 600);
        }

        private void FrmSubirTarea_Load(object sender, EventArgs e)
        {
            // ── Datos automáticos ─────────────────────────────────────────────────
            txtNombre.Text    = SesionUsuario.Nombre    ?? "";
            txtMatricula.Text = SesionUsuario.Matricula ?? "";
            lblfecha.Text     = "📅 " + DateTime.Now.ToString("dd/MM/yyyy");

            // ── Scroll en descripción ─────────────────────────────────────────────
            txtDescripcion.ScrollBars = ScrollBars.Vertical;
            txtDescripcion.MaxLength  = 0;

            // ── ComboBox tipo tarea ───────────────────────────────────────────────
            if (cmbtipotarea.Items.Count > 0 && cmbtipotarea.SelectedIndex < 0)
                cmbtipotarea.SelectedIndex = 0;

            // ── Inicializar cascada ───────────────────────────────────────────────
            InicializarComboboxes();

            // ── Controles de tags ─────────────────────────────────────────────────
            AgregarControlesTags();

            // ── Panel lateral de archivos ─────────────────────────────────────────
            CrearPanelArchivos();

            // ── Cargar facultades y aplicar restricciones por rol ─────────────────
            CargarFacultades();
            AplicarRestriccionesPorRol();
        }

        // ── RESTRICCIONES SEGÚN ROL ───────────────────────────────────────────────
        /// <summary>
        /// Bloquea las opciones de la cascada según el rol del usuario:
        ///   ALUMNO   → Facultad + Licenciatura + Semestre bloqueados (solo elige Materia)
        ///   MAESTRO  → Solo Facultad bloqueada
        /// </summary>
        private void AplicarRestriccionesPorRol()
        {
            string rol = SesionUsuario.Rol ?? "";

            if (rol == "ALUMNO")
            {
                // Licenciatura: auto-seleccionar la del usuario
                if (!string.IsNullOrEmpty(SesionUsuario.ID_Licenciatura))
                {
                    for (int i = 1; i < cmbLicenciatura.Items.Count; i++)
                        if (cmbLicenciatura.Items[i] is LicenciaturaItem li &&
                            li.ID == SesionUsuario.ID_Licenciatura)
                        {
                            cmbLicenciatura.SelectedIndex = i; // dispara CmbLicenciatura_Changed → carga semestres
                            break;
                        }
                }

                // Semestre: auto-seleccionar el del usuario
                if (!string.IsNullOrEmpty(SesionUsuario.Semestre))
                {
                    for (int i = 1; i < cmbSemestre.Items.Count; i++)
                        if (cmbSemestre.Items[i] is SemestreItem si &&
                            si.Valor == SesionUsuario.Semestre)
                        {
                            cmbSemestre.SelectedIndex = i; // dispara CmbSemestre_Changed → carga materias
                            break;
                        }
                }

                // Bloquear los tres primeros niveles
                BloquearCombo(cmbFacultad);
                BloquearCombo(cmbLicenciatura);
                BloquearCombo(cmbSemestre);
            }
            else if (rol == "MAESTRO")
            {
                // Solo bloquear Facultad
                BloquearCombo(cmbFacultad);
            }
        }

        private void BloquearCombo(ComboBox cmb)
        {
            cmb.Enabled   = false;
            cmb.BackColor = System.Drawing.SystemColors.Control;
        }

        // ── INICIALIZACIÓN DE COMBOBOXES ──────────────────────────────────────────
        private void InicializarComboboxes()
        {
            cmbFacultad.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFacultad.Items.Clear();
            cmbFacultad.Items.Add("── Facultad ──");
            cmbFacultad.SelectedIndex = 0;

            cmbLicenciatura.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLicenciatura.Items.Clear();
            cmbLicenciatura.Items.Add("── Licenciatura ──");
            cmbLicenciatura.SelectedIndex = 0;
            cmbLicenciatura.Enabled = false;

            cmbSemestre.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSemestre.Items.Clear();
            cmbSemestre.Items.Add("── Semestre ──");
            cmbSemestre.SelectedIndex = 0;
            cmbSemestre.Enabled = false;

            cmbMateria.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMateria.Items.Clear();
            cmbMateria.Items.Add("── Materia ──");
            cmbMateria.SelectedIndex = 0;
            cmbMateria.Enabled = false;
        }

        // ── CARGA DE FACULTADES ───────────────────────────────────────────────────
        private void CargarFacultades()
        {
            cmbFacultad.Items.Clear();
            cmbFacultad.Items.Add("── Facultad ──");

            try
            {
                using (MySqlConnection conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();
                    string sql = @"SELECT ID_Facultad, Nombre_Facultad
                                   FROM tbl_WikiUnach_Facultades
                                   WHERE ID_Facultad != 'FAC_GENERAL'
                                   ORDER BY Nombre_Facultad";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    using (MySqlDataReader r = cmd.ExecuteReader())
                        while (r.Read())
                            cmbFacultad.Items.Add(new FacultadItem
                            {
                                ID     = r["ID_Facultad"].ToString(),
                                Nombre = r["Nombre_Facultad"].ToString()
                            });
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al cargar facultades:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Preseleccionar la facultad del usuario si aplica
            for (int i = 1; i < cmbFacultad.Items.Count; i++)
                if (cmbFacultad.Items[i] is FacultadItem fi &&
                    fi.ID == SesionUsuario.ID_Facultad)
                { cmbFacultad.SelectedIndex = i; return; }

            cmbFacultad.SelectedIndex = 0;
        }

        // ── EVENTOS EN CASCADA ────────────────────────────────────────────────────
        private void CmbFacultad_Changed(object sender, EventArgs e)
        {
            ResetearCombo(cmbLicenciatura, "── Licenciatura ──");
            ResetearCombo(cmbSemestre,     "── Semestre ──");
            ResetearCombo(cmbMateria,      "── Materia ──");

            if (!(cmbFacultad.SelectedItem is FacultadItem fi)) return;
            CargarLicenciaturas(fi.ID);
        }

        private void CmbLicenciatura_Changed(object sender, EventArgs e)
        {
            ResetearCombo(cmbSemestre, "── Semestre ──");
            ResetearCombo(cmbMateria,  "── Materia ──");

            if (!(cmbLicenciatura.SelectedItem is LicenciaturaItem li)) return;
            CargarSemestres(li.ID);
        }

        private void CmbSemestre_Changed(object sender, EventArgs e)
        {
            ResetearCombo(cmbMateria, "── Materia ──");

            if (!(cmbLicenciatura.SelectedItem is LicenciaturaItem li)) return;
            if (!(cmbSemestre.SelectedItem is SemestreItem si))         return;
            CargarMaterias(li.ID, si.Valor);
        }

        private void ResetearCombo(ComboBox cmb, string placeholder)
        {
            cmb.Items.Clear();
            cmb.Items.Add(placeholder);
            cmb.SelectedIndex = 0;
            cmb.Enabled = false;
        }

        // ── CARGA LICENCIATURAS ───────────────────────────────────────────────────
        private void CargarLicenciaturas(string idFacultad)
        {
            cmbLicenciatura.Items.Clear();
            cmbLicenciatura.Items.Add("── Licenciatura ──");
            try
            {
                using (MySqlConnection conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();
                    string sql = @"SELECT ID_Licenciatura, Nombre_Licenciatura
                                   FROM tbl_WikiUnach_Licenciaturas
                                   WHERE ID_Facultad = @id
                                   ORDER BY Nombre_Licenciatura";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idFacultad);
                        using (MySqlDataReader r = cmd.ExecuteReader())
                            while (r.Read())
                                cmbLicenciatura.Items.Add(new LicenciaturaItem
                                {
                                    ID     = r["ID_Licenciatura"].ToString(),
                                    Nombre = r["Nombre_Licenciatura"].ToString()
                                });
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al cargar licenciaturas:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            cmbLicenciatura.SelectedIndex = 0;
            cmbLicenciatura.Enabled = true;
        }

        // ── CARGA SEMESTRES ───────────────────────────────────────────────────────
        private void CargarSemestres(string idLicenciatura)
        {
            cmbSemestre.Items.Clear();
            cmbSemestre.Items.Add("── Semestre ──");
            try
            {
                using (MySqlConnection conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();
                    string sql = @"SELECT DISTINCT Semestre
                                   FROM tbl_WikiUnach_Materias
                                   WHERE ID_Licenciatura = @id
                                   ORDER BY LENGTH(Semestre), Semestre";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idLicenciatura);
                        using (MySqlDataReader r = cmd.ExecuteReader())
                            while (r.Read())
                                cmbSemestre.Items.Add(new SemestreItem
                                {
                                    Valor = r["Semestre"].ToString()
                                });
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al cargar semestres:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            cmbSemestre.SelectedIndex = 0;
            cmbSemestre.Enabled = true;
        }

        // ── CARGA MATERIAS ────────────────────────────────────────────────────────
        private void CargarMaterias(string idLicenciatura, string semestre)
        {
            cmbMateria.Items.Clear();
            cmbMateria.Items.Add("── Materia ──");
            try
            {
                using (MySqlConnection conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();
                    string sql = @"SELECT ID_Materia, Nombre_Materia
                                   FROM tbl_WikiUnach_Materias
                                   WHERE ID_Licenciatura = @idLic
                                     AND Semestre        = @sem
                                   ORDER BY Nombre_Materia";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@idLic", idLicenciatura);
                        cmd.Parameters.AddWithValue("@sem",   semestre);
                        using (MySqlDataReader r = cmd.ExecuteReader())
                            while (r.Read())
                                cmbMateria.Items.Add(new MateriaItem
                                {
                                    ID     = r["ID_Materia"].ToString(),
                                    Nombre = r["Nombre_Materia"].ToString()
                                });
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al cargar materias:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            cmbMateria.SelectedIndex = 0;
            cmbMateria.Enabled = true;
        }

        // ── CONTROLES DE TAGS (en código, junto a lbltags) ────────────────────────
        private void AgregarControlesTags()
        {
            txtTagsInput = new TextBox
            {
                Location  = new Point(lbltags.Right + 10, lbltags.Top - 2),
                Size      = new Size(180, 24),
                Font      = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                Text      = "nueva etiqueta..."
            };
            txtTagsInput.Enter += (s, ev) =>
            {
                if (txtTagsInput.ForeColor == Color.Gray)
                { txtTagsInput.Text = ""; txtTagsInput.ForeColor = Color.Black; }
            };
            txtTagsInput.Leave += (s, ev) =>
            {
                if (string.IsNullOrWhiteSpace(txtTagsInput.Text))
                { txtTagsInput.Text = "nueva etiqueta..."; txtTagsInput.ForeColor = Color.Gray; }
            };
            txtTagsInput.KeyDown += (s, ev) =>
            {
                if (ev.KeyCode == Keys.Enter) { AgregarTag(); ev.SuppressKeyPress = true; }
            };

            btnAgregarTag = new Button
            {
                Text      = "+ Tag",
                Location  = new Point(txtTagsInput.Right + 6, lbltags.Top - 3),
                Size      = new Size(60, 26),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(30, 100, 210),
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnAgregarTag.FlatAppearance.BorderSize = 0;
            btnAgregarTag.Click += (s, ev) => AgregarTag();

            lbltags.Text = "🏷 Sin etiquetas";

            this.Controls.Add(txtTagsInput);
            this.Controls.Add(btnAgregarTag);
            txtTagsInput.BringToFront();
            btnAgregarTag.BringToFront();
        }

        // ── LÓGICA DE ETIQUETAS ───────────────────────────────────────────────────
        private void AgregarTag()
        {
            string tag = txtTagsInput.Text.Trim();
            if (string.IsNullOrEmpty(tag) || txtTagsInput.ForeColor == Color.Gray) return;

            if (!tag.StartsWith("#")) tag = "#" + tag;
            tag = tag.Replace(" ", "_");

            if (!_tags.Contains(tag))
            {
                _tags.Add(tag);
                lbltags.Text = string.Join("  ", _tags);
            }

            txtTagsInput.Text      = "nueva etiqueta...";
            txtTagsInput.ForeColor = Color.Gray;
        }

        // ── PANEL LATERAL DE ARCHIVOS (desliza desde la DERECHA) ─────────────────
        private void CrearPanelArchivos()
        {
            int anchoSlide = (int)(this.ClientSize.Width * 0.42);

            pnlArchivosSlide = new Panel
            {
                Size        = new Size(anchoSlide, this.ClientSize.Height),
                Location    = new Point(this.ClientSize.Width, 0),
                BackColor   = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AllowDrop   = true,
                Visible     = false
            };

            // Header
            Panel pnlHeader = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 48,
                BackColor = Color.FromArgb(28, 28, 28)
            };
            Label lblTitPanel = new Label
            {
                Text      = "Archivos adjuntos",
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 11, FontStyle.Bold),
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
            btnCerrar.Click += (s, e) => CerrarArchivos();
            pnlHeader.Controls.Add(lblTitPanel);
            pnlHeader.Controls.Add(btnCerrar);

            // Sub-header con botón examinar
            Panel pnlAccion = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 54,
                BackColor = Color.FromArgb(245, 247, 250)
            };
            Button btnExaminar = new Button
            {
                Text      = "📂  Subir Archivos...",
                Location  = new Point(12, 11),
                Size      = new Size(175, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(30, 100, 210),
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnExaminar.FlatAppearance.BorderSize = 0;
            btnExaminar.Click += BtnExaminar_Click;

            Label lblHint = new Label
            {
                Text      = "o arrastra archivos aquí",
                Location  = new Point(198, 18),
                AutoSize  = true,
                Font      = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(140, 140, 140)
            };
            pnlAccion.Controls.Add(btnExaminar);
            pnlAccion.Controls.Add(lblHint);

            // Lista scrolleable
            flpArchivosSlide = new FlowLayoutPanel
            {
                Dock          = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll    = true,
                WrapContents  = false,
                BackColor     = Color.FromArgb(250, 251, 252),
                Padding       = new Padding(10, 8, 10, 8),
                AllowDrop     = true
            };
            flpArchivosSlide.DragEnter += (s, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    e.Effect = DragDropEffects.Copy;
            };
            flpArchivosSlide.DragDrop += (s, e) =>
            {
                foreach (string ruta in (string[])e.Data.GetData(DataFormats.FileDrop))
                    AgregarArchivo(ruta);
            };
            pnlArchivosSlide.DragEnter += (s, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    e.Effect = DragDropEffects.Copy;
            };
            pnlArchivosSlide.DragDrop += (s, e) =>
            {
                foreach (string ruta in (string[])e.Data.GetData(DataFormats.FileDrop))
                    AgregarArchivo(ruta);
            };

            pnlArchivosSlide.Controls.Add(flpArchivosSlide);
            pnlArchivosSlide.Controls.Add(pnlAccion);
            pnlArchivosSlide.Controls.Add(pnlHeader);

            this.Controls.Add(pnlArchivosSlide);
            pnlArchivosSlide.BringToFront();

            timerArchivos      = new System.Windows.Forms.Timer { Interval = 12 };
            timerArchivos.Tick += TimerArchivos_Tick;
        }

        // ── EXAMINAR ARCHIVOS ─────────────────────────────────────────────────────
        private void BtnExaminar_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title       = "Seleccionar archivo(s)";
                ofd.Multiselect = true;
                ofd.Filter      = "Todos los archivos (*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK)
                    foreach (string ruta in ofd.FileNames)
                        AgregarArchivo(ruta);
            }
        }

        private void AgregarArchivo(string rutaLocal)
        {
            if (!File.Exists(rutaLocal)) return;
            FileInfo fi = new FileInfo(rutaLocal);

            var arch = new ArchivoAdjunto
            {
                RutaLocal   = rutaLocal,
                Nombre      = Path.GetFileNameWithoutExtension(rutaLocal),
                Descripcion = "",
                TamanoBytes = fi.Length,
                Extension   = fi.Extension.TrimStart('.').ToLower()
            };
            _archivosAdjuntos.Add(arch);

            int ancho = flpArchivosSlide.ClientSize.Width - 24;
            if (ancho < 120) ancho = 380;
            flpArchivosSlide.Controls.Add(CrearItemArchivoUpload(arch, ancho));
        }

        private Panel CrearItemArchivoUpload(ArchivoAdjunto arch, int ancho)
        {
            Panel pnl = new Panel
            {
                Size      = new Size(ancho, 122),
                BackColor = Color.White,
                Margin    = new Padding(0, 0, 0, 8)
            };

            PictureBox pb = new PictureBox
            {
                Location  = new Point(8, 14),
                Size      = new Size(40, 40),
                SizeMode  = PictureBoxSizeMode.Zoom,
                Image     = ObtenerImagenPorExtension(arch.Extension),
                BackColor = Color.FromArgb(245, 245, 245)
            };

            TextBox txtNom = new TextBox
            {
                Location    = new Point(56, 12),
                Size        = new Size(ancho - 108, 24),
                Text        = arch.Nombre,
                Font        = new Font("Segoe UI", 9, FontStyle.Bold),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtNom.TextChanged += (s, e) => arch.Nombre = txtNom.Text;

            TextBox txtDesc = new TextBox
            {
                Location    = new Point(56, 42),
                Size        = new Size(ancho - 108, 24),
                Font        = new Font("Segoe UI", 8.5f),
                ForeColor   = Color.Gray,
                Text        = "Descripción breve...",
                BorderStyle = BorderStyle.FixedSingle
            };
            txtDesc.Enter += (s, e) =>
            {
                if (txtDesc.ForeColor == Color.Gray)
                { txtDesc.Text = arch.Descripcion; txtDesc.ForeColor = Color.Black; }
            };
            txtDesc.Leave += (s, e) =>
            {
                arch.Descripcion = txtDesc.ForeColor == Color.Gray ? "" : txtDesc.Text;
                if (string.IsNullOrWhiteSpace(txtDesc.Text))
                { txtDesc.Text = "Descripción breve..."; txtDesc.ForeColor = Color.Gray; }
            };

            Label lblInfo = new Label
            {
                Text      = $"📦 {FormatearBytes(arch.TamanoBytes)}   •   {arch.Extension.ToUpper()}",
                Location  = new Point(56, 72),
                AutoSize  = true,
                Font      = new Font("Segoe UI", 7.5f),
                ForeColor = Color.FromArgb(150, 150, 150)
            };

            Label lblFechaAr = new Label
            {
                Text      = "📅 " + DateTime.Now.ToString("dd/MM/yyyy"),
                Location  = new Point(56, 90),
                AutoSize  = true,
                Font      = new Font("Segoe UI", 7.5f),
                ForeColor = Color.FromArgb(150, 150, 150)
            };

            Button btnDel = new Button
            {
                Text      = "✕",
                Location  = new Point(ancho - 44, 12),
                Size      = new Size(28, 26),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(255, 240, 240),
                ForeColor = Color.FromArgb(200, 50, 50),
                Font      = new Font("Segoe UI", 8, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnDel.FlatAppearance.BorderSize = 0;
            btnDel.Click += (s, e) =>
            {
                _archivosAdjuntos.Remove(arch);
                flpArchivosSlide.Controls.Remove(pnl);
            };

            Panel linea = new Panel
            {
                Location  = new Point(0, 120),
                Size      = new Size(ancho, 1),
                BackColor = Color.FromArgb(230, 230, 230)
            };

            pnl.Controls.Add(pb);
            pnl.Controls.Add(txtNom);
            pnl.Controls.Add(txtDesc);
            pnl.Controls.Add(lblInfo);
            pnl.Controls.Add(lblFechaAr);
            pnl.Controls.Add(btnDel);
            pnl.Controls.Add(linea);

            return pnl;
        }

        private string FormatearBytes(long bytes)
        {
            if (bytes < 1024L)        return $"{bytes} B";
            if (bytes < 1024L * 1024) return $"{bytes / 1024.0:F1} KB";
            return                           $"{bytes / (1024.0 * 1024):F1} MB";
        }

        // ── ANIMACIÓN DEL PANEL ───────────────────────────────────────────────────
        private void AbrirArchivos()
        {
            int anchoSlide = (int)(this.ClientSize.Width * 0.42);
            pnlArchivosSlide.Size     = new Size(anchoSlide, this.ClientSize.Height);
            pnlArchivosSlide.Location = new Point(this.ClientSize.Width, 0);
            pnlArchivosSlide.Visible  = true;
            pnlArchivosSlide.BringToFront();
            archivosAbierto   = true;
            timerArchivos.Tag = this.ClientSize.Width - anchoSlide;
            timerArchivos.Start();
        }

        private void CerrarArchivos()
        {
            archivosAbierto   = false;
            timerArchivos.Tag = this.ClientSize.Width;
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

        // ── SUBIR TAREA ───────────────────────────────────────────────────────────
        private void BtnSubir_Click(object sender, EventArgs e)
        {
            // ── Validaciones ──────────────────────────────────────────────────────
            string titulo = txtTitulo.Text.Trim();
            if (string.IsNullOrEmpty(titulo))
            {
                MessageBox.Show("Escribe un título para la tarea.",
                    "Campo requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitulo.Focus();
                return;
            }

            if (!(cmbMateria.SelectedItem is MateriaItem materiaItem))
            {
                MessageBox.Show("Selecciona la materia completa (Facultad → Licenciatura → Semestre → Materia).",
                    "Campo requerido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string descripcion = txtDescripcion.Text.Trim();
            int palabras = descripcion.Split(
                new char[] { ' ', '\n', '\r', '\t' },
                StringSplitOptions.RemoveEmptyEntries).Length;

            if (palabras > 10000)
            {
                MessageBox.Show($"La descripción tiene {palabras} palabras. El máximo es 10,000.",
                    "Descripción muy larga", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string idMateria     = materiaItem.ID;
            string tipoActividad = cmbtipotarea.SelectedItem?.ToString() ?? "Tarea";
            string idPagina      = Guid.NewGuid().ToString();
            string idUsuario     = SesionUsuario.ID_Usuario;

            btnSubir.Enabled = false;
            btnSubir.Text    = "Subiendo...";

            try
            {
                // ── PASO 1: Subir archivos a S3 ───────────────────────────────────
                var archivosSubidos = new List<(ArchivoAdjunto Arch, string UrlS3)>();
                foreach (ArchivoAdjunto arch in _archivosAdjuntos)
                {
                    string urlS3 = S3Service.SubirArchivo(arch.RutaLocal, $"tareas/{idPagina}");
                    archivosSubidos.Add((arch, urlS3));
                }

                // ── PASO 2: Transacción en BD ─────────────────────────────────────
                using (MySqlConnection conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();
                    using (MySqlTransaction tx = conn.BeginTransaction())
                    {
                        try
                        {
                            // INSERT PaginasWiki
                            string sqlPag = @"
                                INSERT INTO tbl_WikiUnach_PaginasWiki
                                    (ID_Pagina, ID_Materia, Titulo, Contenido_Actual,
                                     Tipo_Actividad, Votos_Positivos, Votos_Negativos,
                                     Fecha_Creacion, Fecha_Actualizacion, Esta_Bloqueada)
                                VALUES
                                    (@id, @idMat, @titulo, @contenido,
                                     @tipo, 0, 0, NOW(), NOW(), 0)";
                            using (MySqlCommand cmd = new MySqlCommand(sqlPag, conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@id",        idPagina);
                                cmd.Parameters.AddWithValue("@idMat",     idMateria);
                                cmd.Parameters.AddWithValue("@titulo",    titulo);
                                cmd.Parameters.AddWithValue("@contenido", descripcion);
                                cmd.Parameters.AddWithValue("@tipo",      tipoActividad);
                                cmd.ExecuteNonQuery();
                            }

                            // Etiquetas
                            foreach (string tag in _tags)
                            {
                                string nombreTag  = tag.TrimStart('#');
                                string idEtiqueta = null;

                                using (MySqlCommand cmd = new MySqlCommand(
                                    "SELECT ID_Etiqueta FROM tbl_WikiUnach_Etiquetas WHERE Nombre_Etiqueta = @n LIMIT 1",
                                    conn, tx))
                                {
                                    cmd.Parameters.AddWithValue("@n", nombreTag);
                                    object res = cmd.ExecuteScalar();
                                    idEtiqueta = (res != null && res != DBNull.Value) ? res.ToString() : null;
                                }

                                if (idEtiqueta == null)
                                {
                                    idEtiqueta = Guid.NewGuid().ToString();
                                    using (MySqlCommand cmd = new MySqlCommand(
                                        "INSERT INTO tbl_WikiUnach_Etiquetas (ID_Etiqueta, Nombre_Etiqueta) VALUES (@id, @n)",
                                        conn, tx))
                                    {
                                        cmd.Parameters.AddWithValue("@id", idEtiqueta);
                                        cmd.Parameters.AddWithValue("@n",  nombreTag);
                                        cmd.ExecuteNonQuery();
                                    }
                                }

                                using (MySqlCommand cmd = new MySqlCommand(
                                    "INSERT IGNORE INTO tbl_WikiUnach_Pagina_Etiquetas (ID_Pagina, ID_Etiqueta) VALUES (@p, @e)",
                                    conn, tx))
                                {
                                    cmd.Parameters.AddWithValue("@p", idPagina);
                                    cmd.Parameters.AddWithValue("@e", idEtiqueta);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            // Archivos
                            string idArchivoPrincipal = null;
                            for (int i = 0; i < archivosSubidos.Count; i++)
                            {
                                ArchivoAdjunto arch  = archivosSubidos[i].Arch;
                                string         urlS3 = archivosSubidos[i].UrlS3;
                                string         idArch = Guid.NewGuid().ToString();

                                using (MySqlCommand cmd = new MySqlCommand(@"
                                    INSERT INTO tbl_WikiUnach_Archivos
                                        (ID_Archivo, ID_Materia, Titulo, Descripcion, URL_Archivo,
                                         Tipo_Archivo, Tamano_Bytes, Fecha_Creacion, ID_Usuario)
                                    VALUES (@idA, @idMat, @tit, @desc, @url, @tipo, @tam, NOW(), @idU)",
                                    conn, tx))
                                {
                                    cmd.Parameters.AddWithValue("@idA",  idArch);
                                    cmd.Parameters.AddWithValue("@idMat", idMateria);
                                    cmd.Parameters.AddWithValue("@tit",  arch.Nombre);
                                    cmd.Parameters.AddWithValue("@desc", arch.Descripcion);
                                    cmd.Parameters.AddWithValue("@url",  urlS3);
                                    cmd.Parameters.AddWithValue("@tipo", arch.Extension);
                                    cmd.Parameters.AddWithValue("@tam",  arch.TamanoBytes);
                                    cmd.Parameters.AddWithValue("@idU",  idUsuario);
                                    cmd.ExecuteNonQuery();
                                }

                                if (i == 0) idArchivoPrincipal = idArch;
                            }

                            // Actualizar archivo principal
                            if (idArchivoPrincipal != null)
                                using (MySqlCommand cmd = new MySqlCommand(
                                    "UPDATE tbl_WikiUnach_PaginasWiki SET ID_Archivo_Principal = @a WHERE ID_Pagina = @p",
                                    conn, tx))
                                {
                                    cmd.Parameters.AddWithValue("@a", idArchivoPrincipal);
                                    cmd.Parameters.AddWithValue("@p", idPagina);
                                    cmd.ExecuteNonQuery();
                                }

                            // Revisión inicial
                            using (MySqlCommand cmd = new MySqlCommand(@"
                                INSERT INTO tbl_WikiUnach_RevisionesWiki
                                    (ID_Revision, ID_Pagina, ID_Usuario,
                                     Instantanea_Contenido, Mensaje_Commit, Fecha_Creacion)
                                VALUES (@idR, @idP, @idU, @cont, 'Publicación inicial', NOW())",
                                conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@idR",  Guid.NewGuid().ToString());
                                cmd.Parameters.AddWithValue("@idP",  idPagina);
                                cmd.Parameters.AddWithValue("@idU",  idUsuario);
                                cmd.Parameters.AddWithValue("@cont", descripcion);
                                cmd.ExecuteNonQuery();
                            }

                            tx.Commit();

                            MessageBox.Show("¡Tarea publicada correctamente!",
                                "Publicación exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Close();
                        }
                        catch
                        {
                            tx.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al publicar la tarea:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSubir.Enabled = true;
                btnSubir.Text    = "Subir Tarea";
            }
        }

        // ── ÍCONO POR EXTENSIÓN ───────────────────────────────────────────────────
        private Image ObtenerImagenPorExtension(string ext)
        {
            ext = ext.ToLower().TrimStart('.');
            string archivo;
            switch (ext)
            {
                case "cs":                                        archivo = "C#.png";         break;
                case "cpp": case "cc": case "h": case "hpp":     archivo = "C++.png";        break;
                case "xlsx": case "xls":                          archivo = "excel.png";      break;
                case "java":                                      archivo = "java.png";       break;
                case "mp3": case "wav": case "ogg":               archivo = "mp3.png";        break;
                case "mp4": case "avi": case "mov": case "mkv":   archivo = "mp4.png";        break;
                case "mwb":                                       archivo = "mwb.png";        break;
                case "pdf":                                       archivo = "pdf.png";        break;
                case "pptx": case "ppt":                          archivo = "powerpoint.png"; break;
                case "py":                                        archivo = "python.png";     break;
                case "rar":                                       archivo = "rar.png";        break;
                case "docx": case "doc":                          archivo = "word.png";       break;
                case "zip": case "7z":                            archivo = "zip.png";        break;
                default:                                          archivo = "txt.png";        break;
            }
            string ruta     = ResolverRutaIcono(archivo);
            string fallback = ResolverRutaIcono("txt.png");
            if (ruta != null)     return Image.FromFile(ruta);
            if (fallback != null) return Image.FromFile(fallback);
            return null;
        }

        // ── NAVEGACIÓN ────────────────────────────────────────────────────────────
        private void btnregresar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
