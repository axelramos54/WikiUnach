using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WUNACH
{
    public partial class FrmPrincipal : Form
    {
        private Button botonActivo = null;

        // ── CLASES HELPER ─────────────────────────────────────────────────────────
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

        // ── CONSTRUCTOR ───────────────────────────────────────────────────────────
        public FrmPrincipal()
        {
            InitializeComponent();
            pnlContenedor.BackColor = Color.White;

            // Estado inicial: solo Facultad habilitada
            InicializarComboboxes();

            // Eventos en cascada
            cmbFacultad.SelectedIndexChanged    += CmbFacultad_Changed;
            cmbLicenciatura.SelectedIndexChanged += CmbLicenciatura_Changed;
            cmbSemestre.SelectedIndexChanged     += CmbSemestre_Changed;

            this.Load += (s, e) =>
            {
                CargarFacultades();
                AgregarToggleAdminBloqueo();
                HacerResponsivo();
                InicializarBusqueda();
                AgregarCampanitaNotificaciones();
                LayoutHelper.AplicarTitulo(this, "Inicio");
                Tema.AplicarA(this);
            };

            this.MinimumSize = new System.Drawing.Size(1100, 600);
        }

        // ── LAYOUT RESPONSIVO ─────────────────────────────────────────────────────
        private void HacerResponsivo()
        {
            // pnlContenedor (área principal) → llena el espacio restante
            pnlContenedor.Dock = DockStyle.Fill;

            // Reorganizar Z-order para que el dock funcione correctamente
            pnlHeader.SendToBack();
            pnlTabs.SendToBack();
            pnlHeader.BringToFront();   // header arriba
            pnlTabs.BringToFront();     // tabs debajo del header
            pnlContenedor.BringToFront(); // ocupa el resto

            // Actualizar en vivo cuando se cambia el tema desde otro formulario
            Action handler = () => { if (!this.IsDisposed) Tema.AplicarA(this); };
            Tema.TemaCambiado += handler;
            this.FormClosed   += (s, e) => Tema.TemaCambiado -= handler;
        }

        // ── TOGGLE DE BLOQUEO ADMIN ──────────────────────────────────────────────
        private void AgregarToggleAdminBloqueo()
        {
            if (SesionUsuario.Rol != "ADMINISTRADOR") return;

            CheckBox chkBloqueo = new CheckBox
            {
                Text       = "🔒 Modo bloqueo (clic en tarea = bloquear/desbloquear)",
                AutoSize   = true,
                Font       = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold),
                ForeColor  = System.Drawing.Color.FromArgb(180, 60, 60),
                BackColor  = System.Drawing.Color.Transparent,
                Location   = new System.Drawing.Point(1090, 80),
                Cursor     = Cursors.Hand
            };
            chkBloqueo.CheckedChanged += (s, ev) =>
            {
                SesionUsuario.ModoBloqueoAdmin = chkBloqueo.Checked;
                chkBloqueo.ForeColor = chkBloqueo.Checked
                    ? System.Drawing.Color.FromArgb(200, 30, 30)
                    : System.Drawing.Color.FromArgb(180, 60, 60);
            };
            pnlHeader.Controls.Add(chkBloqueo);
            chkBloqueo.BringToFront();
        }

        // ── INICIALIZACIÓN DE COMBOBOXES ──────────────────────────────────────────
        private void InicializarComboboxes()
        {
            // Facultad: habilitada, placeholder al inicio
            cmbFacultad.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFacultad.Items.Clear();
            cmbFacultad.Items.Add("── Selecciona una Facultad ──");
            cmbFacultad.SelectedIndex = 0;

            // Licenciatura: deshabilitada hasta que se elija Facultad
            cmbLicenciatura.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLicenciatura.Items.Clear();
            cmbLicenciatura.Items.Add("── Elige Facultad primero ──");
            cmbLicenciatura.SelectedIndex = 0;
            cmbLicenciatura.Enabled = false;

            // Semestre: deshabilitado hasta que se elija Licenciatura
            cmbSemestre.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSemestre.Items.Clear();
            cmbSemestre.Items.Add("── Elige Licenciatura primero ──");
            cmbSemestre.SelectedIndex = 0;
            cmbSemestre.Enabled = false;

            // Limpiar área de contenido
            pnlTabs.Controls.Clear();
            pnlContenedor.Controls.Clear();
        }

        // ── CARGA DE FACULTADES ───────────────────────────────────────────────────
        private void CargarFacultades()
        {
            cmbFacultad.Items.Clear();
            cmbFacultad.Items.Add("── Selecciona una Facultad ──");

            try
            {
                using (MySqlConnection conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();
                    string sql = @"SELECT ID_Facultad, Nombre_Facultad
                                   FROM tbl_WikiUnach_Facultades
                                   ORDER BY Nombre_Facultad";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                        while (reader.Read())
                            cmbFacultad.Items.Add(new FacultadItem
                            {
                                ID     = reader["ID_Facultad"].ToString(),
                                Nombre = reader["Nombre_Facultad"].ToString()
                            });
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al cargar facultades:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Preseleccionar la facultad del usuario si existe en la lista
            for (int i = 1; i < cmbFacultad.Items.Count; i++)
            {
                if (cmbFacultad.Items[i] is FacultadItem fi &&
                    fi.ID == SesionUsuario.ID_Facultad)
                {
                    cmbFacultad.SelectedIndex = i;
                    return;
                }
            }
            cmbFacultad.SelectedIndex = 0;
        }

        // ── EVENTOS EN CASCADA ────────────────────────────────────────────────────
        private void CmbFacultad_Changed(object sender, EventArgs e)
        {
            // Resetear downstream
            ResetearLicenciatura();
            ResetearSemestre();
            pnlTabs.Controls.Clear();
            pnlContenedor.Controls.Clear();
            botonActivo = null;

            if (!(cmbFacultad.SelectedItem is FacultadItem fi)) return;

            CargarLicenciaturas(fi.ID);
        }

        private void CmbLicenciatura_Changed(object sender, EventArgs e)
        {
            ResetearSemestre();
            pnlTabs.Controls.Clear();
            pnlContenedor.Controls.Clear();
            botonActivo = null;

            if (!(cmbLicenciatura.SelectedItem is LicenciaturaItem li)) return;

            CargarSemestres(li.ID);
        }

        private void CmbSemestre_Changed(object sender, EventArgs e)
        {
            pnlTabs.Controls.Clear();
            pnlContenedor.Controls.Clear();
            botonActivo = null;

            if (!(cmbLicenciatura.SelectedItem is LicenciaturaItem li)) return;
            if (!(cmbSemestre.SelectedItem is SemestreItem si))         return;

            CargarMateriasEnTabs(li.ID, si.Valor);
        }

        // ── HELPERS DE RESET ─────────────────────────────────────────────────────
        private void ResetearLicenciatura()
        {
            cmbLicenciatura.Items.Clear();
            cmbLicenciatura.Items.Add("── Elige Facultad primero ──");
            cmbLicenciatura.SelectedIndex = 0;
            cmbLicenciatura.Enabled = false;
        }

        private void ResetearSemestre()
        {
            cmbSemestre.Items.Clear();
            cmbSemestre.Items.Add("── Elige Licenciatura primero ──");
            cmbSemestre.SelectedIndex = 0;
            cmbSemestre.Enabled = false;
        }

        // ── CARGA DE LICENCIATURAS ────────────────────────────────────────────────
        private void CargarLicenciaturas(string idFacultad)
        {
            cmbLicenciatura.Items.Clear();
            cmbLicenciatura.Items.Add("── Selecciona una Licenciatura ──");

            try
            {
                using (MySqlConnection conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();
                    string sql = @"SELECT ID_Licenciatura, Nombre_Licenciatura
                                   FROM tbl_WikiUnach_Licenciaturas
                                   WHERE ID_Facultad = @idFac
                                   ORDER BY Nombre_Licenciatura";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@idFac", idFacultad);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                            while (reader.Read())
                                cmbLicenciatura.Items.Add(new LicenciaturaItem
                                {
                                    ID     = reader["ID_Licenciatura"].ToString(),
                                    Nombre = reader["Nombre_Licenciatura"].ToString()
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

        // ── CARGA DE SEMESTRES ────────────────────────────────────────────────────
        private void CargarSemestres(string idLicenciatura)
        {
            cmbSemestre.Items.Clear();
            cmbSemestre.Items.Add("── Selecciona un Semestre ──");

            try
            {
                using (MySqlConnection conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();
                    string sql = @"SELECT DISTINCT Semestre
                                   FROM tbl_WikiUnach_Materias
                                   WHERE ID_Licenciatura = @idLic
                                   ORDER BY LENGTH(Semestre), Semestre";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@idLic", idLicenciatura);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                            while (reader.Read())
                                cmbSemestre.Items.Add(new SemestreItem
                                {
                                    Valor = reader["Semestre"].ToString()
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

        // ── CARGA DE MATERIAS COMO TABS ───────────────────────────────────────────
        private void CargarMateriasEnTabs(string idLicenciatura, string semestre)
        {
            pnlTabs.Controls.Clear();

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
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string idMateria     = reader["ID_Materia"].ToString();
                                string nombreMateria = reader["Nombre_Materia"].ToString();

                                Button btnMateria = new Button
                                {
                                    Text      = nombreMateria,
                                    Size      = new Size(160, 40),
                                    FlatStyle = FlatStyle.Flat,
                                    BackColor = Color.FromArgb(240, 240, 240),
                                    ForeColor = Color.Black,
                                    Cursor    = Cursors.Hand,
                                    Margin    = new Padding(5, 5, 0, 0)
                                };
                                btnMateria.FlatAppearance.BorderSize = 0;

                                string capId     = idMateria;
                                string capNombre = nombreMateria;
                                btnMateria.Click += (s, e) =>
                                    SeleccionarMateria((Button)s, capId, capNombre);

                                pnlTabs.Controls.Add(btnMateria);
                            }
                        }
                    }
                }

                if (pnlTabs.Controls.Count == 0)
                    pnlTabs.Controls.Add(new Label
                    {
                        Text     = "No hay materias para este semestre.",
                        AutoSize = true,
                        Margin   = new Padding(10, 10, 0, 0)
                    });
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al cargar materias:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── SELECCIONAR MATERIA ───────────────────────────────────────────────────
        private void SeleccionarMateria(Button botonPresionado, string idMateria, string nombreMateria)
        {
            if (botonActivo != null)
            {
                botonActivo.BackColor = Color.FromArgb(240, 240, 240);
                botonActivo.ForeColor = Color.Black;
            }
            botonPresionado.BackColor = Color.White;
            botonPresionado.ForeColor = Color.DarkBlue;
            botonActivo = botonPresionado;

            pnlContenedor.Controls.Clear();
            UCCurso vistaCurso = new UCCurso { Dock = DockStyle.Fill };
            vistaCurso.ConfigurarCurso(idMateria, nombreMateria);
            pnlContenedor.Controls.Add(vistaCurso);
        }

        // ── NAVEGACIÓN ────────────────────────────────────────────────────────────
        private void btncuenta_Click(object sender, EventArgs e)
        {
            if (SesionUsuario.Rol == "VISITANTE")
            {
                MessageBox.Show(
                    "Estás navegando como Visitante.\n\n" +
                    "Los visitantes no tienen cuenta en el sistema.\n" +
                    "Para acceder a tu perfil, crea una cuenta o inicia sesión.",
                    "Sin cuenta de usuario",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }
            new FrmCuenta().Show();
        }

        private void lblsubir_Click(object sender, EventArgs e)
        {
            if (SesionUsuario.Rol == "VISITANTE")
            {
                MessageBox.Show(
                    "Los visitantes solo pueden ver el contenido.\n\n" +
                    "Para subir tareas, crea una cuenta o inicia sesión.",
                    "Acción no disponible",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }
            new FrmSubirTarea().Show();
        }

        private void btnCerrarsecion_Click(object sender, EventArgs e)
        {
            SesionUsuario.CerrarSesion();
            Tema.ResetearAClaroSinUsuario();   // limpiar tema activo
            FrmAcceso frm = new FrmAcceso();
            frm.Show();
            this.Hide();
        }

        // ─────────────────────────────────────────────────────────────────────────
        //                       BÚSQUEDA GLOBAL
        // ─────────────────────────────────────────────────────────────────────────
        private Panel _pnlResultadosBusqueda;
        private FlowLayoutPanel _flpResultados;

        private void InicializarBusqueda()
        {
            // Placeholder
            txtSearch.Text      = "🔍  Buscar tareas o etiquetas...";
            txtSearch.ForeColor = Color.Gray;
            txtSearch.Enter += (s, e) =>
            {
                if (txtSearch.ForeColor == Color.Gray)
                {
                    txtSearch.Text      = "";
                    txtSearch.ForeColor = Color.Black;
                }
            };
            txtSearch.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    txtSearch.Text      = "🔍  Buscar tareas o etiquetas...";
                    txtSearch.ForeColor = Color.Gray;
                }
            };

            txtSearch.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    string q = txtSearch.ForeColor == Color.Gray ? "" : txtSearch.Text.Trim();
                    RealizarBusqueda(q);
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    CerrarPanelBusqueda();
                }
            };
        }

        private void CrearPanelResultadosBusqueda()
        {
            _pnlResultadosBusqueda = new Panel
            {
                Size        = new Size(560, 400),
                BackColor   = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Visible     = false
            };

            Panel header = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 36,
                BackColor = Color.FromArgb(35, 55, 95)
            };
            Label lblTit = new Label
            {
                Text      = "Resultados",
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize  = true,
                Location  = new Point(12, 8)
            };
            Button btnCerrar = new Button
            {
                Text      = "✕",
                ForeColor = Color.White,
                BackColor = Color.FromArgb(35, 55, 95),
                FlatStyle = FlatStyle.Flat,
                Size      = new Size(32, 32),
                Location  = new Point(_pnlResultadosBusqueda.Width - 36, 2),
                Font      = new Font("Segoe UI", 10, FontStyle.Bold),
                Anchor    = AnchorStyles.Top | AnchorStyles.Right
            };
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.Click += (s, e) => CerrarPanelBusqueda();
            header.Controls.Add(lblTit);
            header.Controls.Add(btnCerrar);

            _flpResultados = new FlowLayoutPanel
            {
                Dock          = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll    = true,
                WrapContents  = false,
                BackColor     = Color.White,
                Padding       = new Padding(8)
            };

            _pnlResultadosBusqueda.Controls.Add(_flpResultados);
            _pnlResultadosBusqueda.Controls.Add(header);
            this.Controls.Add(_pnlResultadosBusqueda);
        }

        private void RealizarBusqueda(string query)
        {
            if (string.IsNullOrEmpty(query) || query.Length < 2)
            {
                CerrarPanelBusqueda();
                return;
            }

            if (_pnlResultadosBusqueda == null) CrearPanelResultadosBusqueda();
            _flpResultados.Controls.Clear();

            // Posicionar debajo del txtSearch
            Point absSearch = txtSearch.PointToScreen(Point.Empty);
            Point relForm   = this.PointToClient(absSearch);
            _pnlResultadosBusqueda.Location = new Point(
                Math.Max(10, relForm.X + txtSearch.Width - _pnlResultadosBusqueda.Width),
                relForm.Y + txtSearch.Height + 4);

            bool esAdmin = SesionUsuario.Rol == "ADMINISTRADOR";
            int totalResultados = 0;

            try
            {
                using (MySqlConnection conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();
                    string sql = @"
                        SELECT DISTINCT pw.ID_Pagina, pw.Titulo, pw.Tipo_Actividad,
                               m.Nombre_Materia, pw.Votos_Positivos, pw.Esta_Bloqueada
                        FROM tbl_WikiUnach_PaginasWiki pw
                        INNER JOIN tbl_WikiUnach_Materias m ON pw.ID_Materia = m.ID_Materia
                        LEFT JOIN tbl_WikiUnach_Pagina_Etiquetas pe ON pe.ID_Pagina = pw.ID_Pagina
                        LEFT JOIN tbl_WikiUnach_Etiquetas e ON e.ID_Etiqueta = pe.ID_Etiqueta
                        WHERE (@isAdmin = 1 OR pw.Esta_Bloqueada = 0)
                          AND (pw.Titulo LIKE @q OR e.Nombre_Etiqueta LIKE @q)
                        ORDER BY pw.Votos_Positivos DESC, pw.Fecha_Actualizacion DESC
                        LIMIT 25";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@isAdmin", esAdmin ? 1 : 0);
                        cmd.Parameters.AddWithValue("@q", "%" + query + "%");
                        using (var r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                _flpResultados.Controls.Add(CrearItemBusqueda(
                                    r["ID_Pagina"].ToString(),
                                    r["Titulo"].ToString(),
                                    r["Tipo_Actividad"].ToString(),
                                    r["Nombre_Materia"].ToString(),
                                    Convert.ToInt32(r["Votos_Positivos"]),
                                    Convert.ToInt32(r["Esta_Bloqueada"]) == 1));
                                totalResultados++;
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error en la búsqueda:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (totalResultados == 0)
                _flpResultados.Controls.Add(new Label
                {
                    Text      = "Sin resultados para \"" + query + "\"",
                    AutoSize  = true,
                    Margin    = new Padding(15, 25, 0, 0),
                    ForeColor = Color.Gray,
                    Font      = new Font("Segoe UI", 9.5f, FontStyle.Italic)
                });

            _pnlResultadosBusqueda.Visible = true;
            _pnlResultadosBusqueda.BringToFront();
        }

        private Panel CrearItemBusqueda(string idPagina, string titulo, string tipo,
                                         string materia, int votos, bool bloqueada)
        {
            Panel item = new Panel
            {
                Size      = new Size(_flpResultados.ClientSize.Width - 20, 64),
                BackColor = Color.White,
                Cursor    = Cursors.Hand,
                Margin    = new Padding(0, 0, 0, 4)
            };

            Panel borde = new Panel
            {
                Location  = new Point(0, 0),
                Size      = new Size(4, 64),
                BackColor = bloqueada ? Color.FromArgb(200, 50, 50) : Color.FromArgb(30, 100, 210)
            };

            Label lblTitulo = new Label
            {
                Text      = (bloqueada ? "🔒  " : "") + titulo,
                Location  = new Point(14, 8),
                AutoSize  = true,
                Font      = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };

            Label lblInfo = new Label
            {
                Text      = "📚 " + materia + "   •   📌 " + tipo,
                Location  = new Point(14, 30),
                AutoSize  = true,
                Font      = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(100, 100, 100)
            };

            Label lblVotos = new Label
            {
                Text      = "👍 " + votos,
                Location  = new Point(14, 46),
                AutoSize  = true,
                Font      = new Font("Segoe UI", 8f),
                ForeColor = Color.FromArgb(140, 140, 140)
            };

            item.Controls.Add(borde);
            item.Controls.Add(lblTitulo);
            item.Controls.Add(lblInfo);
            item.Controls.Add(lblVotos);

            EventHandler enter = (s, e) => item.BackColor = Color.FromArgb(238, 244, 255);
            EventHandler leave = (s, e) => item.BackColor = Color.White;
            item.MouseEnter += enter; lblTitulo.MouseEnter += enter; lblInfo.MouseEnter += enter; lblVotos.MouseEnter += enter;
            item.MouseLeave += leave; lblTitulo.MouseLeave += leave; lblInfo.MouseLeave += leave; lblVotos.MouseLeave += leave;

            EventHandler click = (s, e) =>
            {
                CerrarPanelBusqueda();
                new FrmDetallesTarea(idPagina).Show();
            };
            item.Click += click; lblTitulo.Click += click; lblInfo.Click += click; lblVotos.Click += click; borde.Click += click;

            return item;
        }

        private void CerrarPanelBusqueda()
        {
            if (_pnlResultadosBusqueda != null) _pnlResultadosBusqueda.Visible = false;
        }

        // ─────────────────────────────────────────────────────────────────────────
        //                       NOTIFICACIONES (campanita)
        // ─────────────────────────────────────────────────────────────────────────
        private Button _btnCampanita;
        private Panel  _pnlNotificacionesDropdown;
        private FlowLayoutPanel _flpNotificaciones;

        private void AgregarCampanitaNotificaciones()
        {
            if (string.IsNullOrEmpty(SesionUsuario.ID_Usuario)) return; // visitantes no

            _btnCampanita = new Button
            {
                Text      = "🔔",
                Size      = new Size(54, 54),
                Location  = new Point(pnlHeader.Width - 70, 5),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.LightSteelBlue,
                Font      = new Font("Segoe UI", 14),
                Cursor    = Cursors.Hand,
                Anchor    = AnchorStyles.Top | AnchorStyles.Right
            };
            _btnCampanita.FlatAppearance.BorderSize = 0;
            _btnCampanita.Click += (s, e) => ToggleDropdownNotificaciones();
            pnlHeader.Controls.Add(_btnCampanita);
            _btnCampanita.BringToFront();

            ActualizarBadgeCampanita();
        }

        private void ActualizarBadgeCampanita()
        {
            if (_btnCampanita == null) return;
            int n = Notificaciones.ContarNoLeidas(SesionUsuario.ID_Usuario);
            _btnCampanita.Text = n > 0 ? $"🔔 {n}" : "🔔";
            _btnCampanita.ForeColor = n > 0 ? Color.FromArgb(180, 30, 30) : Color.Black;
        }

        private void ToggleDropdownNotificaciones()
        {
            if (_pnlNotificacionesDropdown != null && _pnlNotificacionesDropdown.Visible)
            {
                _pnlNotificacionesDropdown.Visible = false;
                return;
            }

            if (_pnlNotificacionesDropdown == null) CrearDropdownNotificaciones();
            CargarNotificacionesEnDropdown();

            // Posicionar debajo de la campanita
            Point abs = _btnCampanita.PointToScreen(Point.Empty);
            Point rel = this.PointToClient(abs);
            _pnlNotificacionesDropdown.Location = new Point(
                rel.X + _btnCampanita.Width - _pnlNotificacionesDropdown.Width,
                rel.Y + _btnCampanita.Height + 2);
            _pnlNotificacionesDropdown.Visible = true;
            _pnlNotificacionesDropdown.BringToFront();
        }

        private void CrearDropdownNotificaciones()
        {
            _pnlNotificacionesDropdown = new Panel
            {
                Size        = new Size(380, 420),
                BackColor   = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Visible     = false
            };

            Panel hdr = new Panel { Dock = DockStyle.Top, Height = 38, BackColor = Color.FromArgb(35, 55, 95) };
            hdr.Controls.Add(new Label
            {
                Text = "Notificaciones", ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true, Location = new Point(12, 9)
            });

            Button btnMarcarTodas = new Button
            {
                Text      = "Marcar todas",
                Location  = new Point(238, 6),
                Size      = new Size(110, 26),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(70, 100, 160),
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 8f),
                Cursor    = Cursors.Hand
            };
            btnMarcarTodas.FlatAppearance.BorderSize = 0;
            btnMarcarTodas.Click += (s, e) =>
            {
                Notificaciones.MarcarTodasComoLeidas(SesionUsuario.ID_Usuario);
                CargarNotificacionesEnDropdown();
                ActualizarBadgeCampanita();
            };
            hdr.Controls.Add(btnMarcarTodas);

            _flpNotificaciones = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown,
                AutoScroll = true, WrapContents = false, BackColor = Color.White,
                Padding = new Padding(6)
            };

            _pnlNotificacionesDropdown.Controls.Add(_flpNotificaciones);
            _pnlNotificacionesDropdown.Controls.Add(hdr);
            this.Controls.Add(_pnlNotificacionesDropdown);
        }

        private void CargarNotificacionesEnDropdown()
        {
            _flpNotificaciones.Controls.Clear();
            var lista = Notificaciones.Obtener(SesionUsuario.ID_Usuario, 30);

            if (lista.Count == 0)
            {
                _flpNotificaciones.Controls.Add(new Label
                {
                    Text = "No tienes notificaciones.", AutoSize = true,
                    Margin = new Padding(15, 30, 0, 0), ForeColor = Color.Gray,
                    Font = new Font("Segoe UI", 9, FontStyle.Italic)
                });
                return;
            }

            foreach (var n in lista)
                _flpNotificaciones.Controls.Add(CrearItemNotificacion(n));
        }

        private Panel CrearItemNotificacion(Notificaciones.Item n)
        {
            Panel pnl = new Panel
            {
                Size      = new Size(_flpNotificaciones.ClientSize.Width - 16, 60),
                BackColor = n.Leida ? Color.White : Color.FromArgb(238, 244, 255),
                Cursor    = Cursors.Hand,
                Margin    = new Padding(0, 0, 0, 4)
            };
            Label lblMsg = new Label
            {
                Text     = n.Mensaje, Location = new Point(10, 6),
                Size     = new Size(pnl.Width - 20, 38),
                Font     = new Font("Segoe UI", 9)
            };
            Label lblFecha = new Label
            {
                Text      = n.Fecha.ToString("dd/MM/yyyy HH:mm"),
                Location  = new Point(10, 40),
                AutoSize  = true,
                Font      = new Font("Segoe UI", 7.5f),
                ForeColor = Color.Gray
            };
            pnl.Controls.Add(lblMsg);
            pnl.Controls.Add(lblFecha);

            EventHandler click = (s, e) =>
            {
                Notificaciones.MarcarComoLeida(n.IdNotificacion);
                ActualizarBadgeCampanita();
                _pnlNotificacionesDropdown.Visible = false;
                if (!string.IsNullOrEmpty(n.IdPagina))
                    new FrmDetallesTarea(n.IdPagina).Show();
            };
            pnl.Click += click; lblMsg.Click += click; lblFecha.Click += click;
            return pnl;
        }
    }
}
