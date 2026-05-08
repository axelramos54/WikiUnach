using System;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WUNACH
{
    public partial class FrmAdministrarTareas : Form
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
        public FrmAdministrarTareas()
        {
            InitializeComponent();
            pnlContenedor.BackColor = Color.White;

            InicializarComboboxes();

            cmbFacultad.SelectedIndexChanged    += CmbFacultad_Changed;
            cmbLicenciatura.SelectedIndexChanged += CmbLicenciatura_Changed;
            cmbSemestre.SelectedIndexChanged     += CmbSemestre_Changed;

            // Filtro por categoría guardada
            cmbGuardados.DropDownStyle = ComboBoxStyle.DropDownList;
            if (cmbGuardados.Items.Count > 0) cmbGuardados.SelectedIndex = 0;
            cmbGuardados.SelectedIndexChanged += (s, e) =>
            {
                // Recargar la materia activa con el nuevo filtro
                if (botonActivo != null)
                {
                    string idMat = botonActivo.Tag as string;
                    if (!string.IsNullOrEmpty(idMat)) CargarTareasDelUsuario(idMat);
                }
            };

            btnRegresar.Click += (s, e) => this.Close();

            this.Load += (s, e) =>
            {
                CargarFacultades();
                AplicarRestriccionesPorRol();
                LayoutHelper.AplicarTitulo(this, "Administrar Tareas");
                Tema.AplicarA(this);
            };

            this.MinimumSize = new System.Drawing.Size(1000, 600);
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
                            cmbSemestre.SelectedIndex = i; // dispara CmbSemestre_Changed → carga tabs de materias
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

        // ── INICIALIZACIÓN ────────────────────────────────────────────────────────
        private void InicializarComboboxes()
        {
            cmbFacultad.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFacultad.Items.Clear();
            cmbFacultad.Items.Add("── Selecciona una Facultad ──");
            cmbFacultad.SelectedIndex = 0;

            cmbLicenciatura.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLicenciatura.Items.Clear();
            cmbLicenciatura.Items.Add("── Elige Facultad primero ──");
            cmbLicenciatura.SelectedIndex = 0;
            cmbLicenciatura.Enabled = false;

            cmbSemestre.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSemestre.Items.Clear();
            cmbSemestre.Items.Add("── Elige Licenciatura primero ──");
            cmbSemestre.SelectedIndex = 0;
            cmbSemestre.Enabled = false;

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

            // Preseleccionar facultad del usuario
            for (int i = 1; i < cmbFacultad.Items.Count; i++)
                if (cmbFacultad.Items[i] is FacultadItem fi &&
                    fi.ID == SesionUsuario.ID_Facultad)
                { cmbFacultad.SelectedIndex = i; return; }

            cmbFacultad.SelectedIndex = 0;
        }

        // ── EVENTOS EN CASCADA ────────────────────────────────────────────────────
        private void CmbFacultad_Changed(object sender, EventArgs e)
        {
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

        // ── HELPERS DE RESET ──────────────────────────────────────────────────────
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

        // ── TABS DE MATERIAS ──────────────────────────────────────────────────────
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

                                Button btn = new Button
                                {
                                    Text      = nombreMateria,
                                    Tag       = idMateria,
                                    Size      = new Size(170, 48),
                                    FlatStyle = FlatStyle.Flat,
                                    BackColor = Color.FromArgb(240, 240, 240),
                                    ForeColor = Color.Black,
                                    Cursor    = Cursors.Hand,
                                    Margin    = new Padding(6, 6, 0, 0)
                                };
                                btn.FlatAppearance.BorderSize = 0;

                                string capId     = idMateria;
                                string capNombre = nombreMateria;
                                btn.Click += (s, e) =>
                                    SeleccionarMateria((Button)s, capId, capNombre);

                                pnlTabs.Controls.Add(btn);
                            }
                        }
                    }
                }

                if (pnlTabs.Controls.Count == 0)
                    pnlTabs.Controls.Add(new Label
                    {
                        Text      = "No hay materias para este semestre.",
                        AutoSize  = true,
                        Margin    = new Padding(10, 15, 0, 0),
                        ForeColor = Color.Gray
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

            CargarTareasDelUsuario(idMateria);
        }

        // ── TAREAS DEL USUARIO EN LA MATERIA SELECCIONADA ────────────────────────
        private void CargarTareasDelUsuario(string idMateria)
        {
            pnlContenedor.Controls.Clear();

            FlowLayoutPanel flp = new FlowLayoutPanel
            {
                Dock          = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll    = true,
                WrapContents  = false,
                BackColor     = Color.White,
                Padding       = new Padding(20, 16, 20, 16)
            };
            pnlContenedor.Controls.Add(flp);

            // ── Determinar filtro según cmbGuardados ──────────────────────────────
            string filtro = cmbGuardados.SelectedItem?.ToString() ?? "Mis Tareas";
            string sqlBase;

            switch (filtro)
            {
                case "BookMarked":
                    sqlBase = @"
                        SELECT pw.ID_Pagina, pw.Titulo, pw.Tipo_Actividad,
                               pw.Contenido_Actual, pw.Fecha_Creacion,
                               pw.Fecha_Actualizacion, pw.Esta_Bloqueada
                        FROM tbl_WikiUnach_PaginasWiki pw
                        INNER JOIN tbl_WikiUnach_Bookmarks b
                                ON b.ID_Pagina = pw.ID_Pagina
                        WHERE pw.ID_Materia = @idMat
                          AND b.ID_Usuario  = @idUser
                        ORDER BY pw.Fecha_Actualizacion DESC";
                    break;

                case "Liked":
                    sqlBase = @"
                        SELECT pw.ID_Pagina, pw.Titulo, pw.Tipo_Actividad,
                               pw.Contenido_Actual, pw.Fecha_Creacion,
                               pw.Fecha_Actualizacion, pw.Esta_Bloqueada
                        FROM tbl_WikiUnach_PaginasWiki pw
                        INNER JOIN tbl_WikiUnach_VotosUsuario v
                                ON v.ID_Pagina = pw.ID_Pagina
                        WHERE pw.ID_Materia = @idMat
                          AND v.ID_Usuario  = @idUser
                          AND v.Tipo_Voto   = 'LIKE'
                        ORDER BY pw.Fecha_Actualizacion DESC";
                    break;

                default: // "Mis Tareas"
                    sqlBase = @"
                        SELECT pw.ID_Pagina, pw.Titulo, pw.Tipo_Actividad,
                               pw.Contenido_Actual, pw.Fecha_Creacion,
                               pw.Fecha_Actualizacion, pw.Esta_Bloqueada
                        FROM tbl_WikiUnach_PaginasWiki pw
                        WHERE pw.ID_Materia = @idMat
                          AND EXISTS (
                              SELECT 1 FROM tbl_WikiUnach_RevisionesWiki r
                              WHERE r.ID_Pagina      = pw.ID_Pagina
                                AND r.ID_Usuario     = @idUser
                                AND r.Mensaje_Commit = 'Publicación inicial'
                          )
                        ORDER BY pw.Fecha_Actualizacion DESC";
                    break;
            }

            // ¿Las tareas mostradas son del usuario? (afecta a botones edit/delete)
            bool esMisTareas = filtro == "Mis Tareas";
            bool esAdmin     = SesionUsuario.Rol == "ADMINISTRADOR";

            try
            {
                using (MySqlConnection conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(sqlBase, conn))
                    {
                        cmd.Parameters.AddWithValue("@idMat",  idMateria);
                        cmd.Parameters.AddWithValue("@idUser", SesionUsuario.ID_Usuario);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string   idPagina  = reader["ID_Pagina"].ToString();
                                string   titulo    = reader["Titulo"].ToString();
                                string   tipo      = reader["Tipo_Actividad"].ToString();
                                string   contenido = reader["Contenido_Actual"].ToString();
                                DateTime fechaCrea = Convert.ToDateTime(reader["Fecha_Creacion"]);
                                DateTime fechaAct  = Convert.ToDateTime(reader["Fecha_Actualizacion"]);
                                bool     bloqueada = Convert.ToBoolean(reader["Esta_Bloqueada"]);

                                int ancho = flp.ClientSize.Width - 44;
                                if (ancho < 300) ancho = 500;

                                bool puedeEditar = esMisTareas || esAdmin;

                                flp.Controls.Add(
                                    CrearItemTareaAdmin(idPagina, titulo, tipo,
                                                        contenido, fechaCrea, fechaAct,
                                                        bloqueada, ancho, flp, puedeEditar));
                            }
                        }
                    }
                }

                if (flp.Controls.Count == 0)
                    flp.Controls.Add(new Label
                    {
                        Text      = "No has subido ninguna tarea en esta materia.",
                        AutoSize  = true,
                        Margin    = new Padding(0, 20, 0, 0),
                        ForeColor = Color.Gray,
                        Font      = new Font("Segoe UI", 10)
                    });

                // Re-aplicar tema a los items dinámicos recién creados
                Tema.AplicarA(flp);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al cargar tus tareas:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── PANEL DE TAREA ADMINISTRABLE ──────────────────────────────────────────
        private Panel CrearItemTareaAdmin(string idPagina, string titulo, string tipo,
                                          string contenido, DateTime fechaCrea,
                                          DateTime fechaAct, bool bloqueada,
                                          int ancho, FlowLayoutPanel contenedor,
                                          bool puedeEditar)
        {
            Panel pnl = new Panel
            {
                Size      = new Size(ancho, 110),
                BackColor = Color.White,
                Margin    = new Padding(0, 0, 0, 10)
            };

            // Borde izquierdo (azul = activa, rojo = bloqueada)
            Panel borde = new Panel
            {
                Location  = new Point(0, 0),
                Size      = new Size(5, 110),
                BackColor = bloqueada
                            ? Color.FromArgb(200, 50, 50)
                            : Color.FromArgb(30, 100, 210)
            };

            Label lblTitulo = new Label
            {
                Text      = titulo,
                Location  = new Point(18, 10),
                Size      = new Size(ancho - 260, 24),
                Font      = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 20, 20)
            };

            Label lblTipo = new Label
            {
                Text      = "📌 " + tipo + (bloqueada ? "   🔒 Bloqueada" : ""),
                Location  = new Point(18, 36),
                AutoSize  = true,
                Font      = new Font("Segoe UI", 8.5f),
                ForeColor = bloqueada
                            ? Color.FromArgb(200, 50, 50)
                            : Color.FromArgb(80, 120, 200)
            };

            string preview = contenido.Length > 130
                             ? contenido.Substring(0, 130) + "…"
                             : contenido;
            Label lblDesc = new Label
            {
                Text      = preview,
                Location  = new Point(18, 58),
                Size      = new Size(ancho - 260, 30),
                Font      = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(120, 120, 120)
            };

            Label lblFechas = new Label
            {
                Text      = $"Creado: {fechaCrea:dd/MM/yyyy}   •   Modificado: {fechaAct:dd/MM/yyyy}",
                Location  = new Point(18, 90),
                AutoSize  = true,
                Font      = new Font("Segoe UI", 7.5f),
                ForeColor = Color.FromArgb(150, 150, 150)
            };

            // Botón Ver/Editar — texto y comportamiento dependen de los permisos
            Button btnEditar = new Button
            {
                Text      = puedeEditar ? "✏️  Ver / Editar" : "👁  Ver",
                Location  = new Point(ancho - 238, 28),
                Size      = new Size(118, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = puedeEditar ? Color.FromArgb(30, 100, 210) : Color.FromArgb(80, 110, 160),
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnEditar.FlatAppearance.BorderSize = 0;
            btnEditar.Click += (s, e) =>
                new FrmDetallesTarea(idPagina, puedeEditar).Show();

            // Botón Eliminar — solo si puede editar
            Button btnEliminar = null;
            if (puedeEditar)
            {
                btnEliminar = new Button
                {
                    Text      = "🗑️  Eliminar",
                    Location  = new Point(ancho - 112, 28),
                    Size      = new Size(104, 34),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(255, 240, 240),
                    ForeColor = Color.FromArgb(200, 50, 50),
                    Font      = new Font("Segoe UI", 9, FontStyle.Bold),
                    Cursor    = Cursors.Hand
                };
                btnEliminar.FlatAppearance.BorderColor = Color.FromArgb(220, 150, 150);
                btnEliminar.FlatAppearance.BorderSize  = 1;
                btnEliminar.Click += (s, e) =>
                    EliminarTarea(idPagina, titulo, pnl, contenedor);
            }

            Panel linea = new Panel
            {
                Location  = new Point(0, 109),
                Size      = new Size(ancho, 1),
                BackColor = Color.FromArgb(230, 230, 230)
            };

            pnl.Controls.Add(borde);
            pnl.Controls.Add(lblTitulo);
            pnl.Controls.Add(lblTipo);
            pnl.Controls.Add(lblDesc);
            pnl.Controls.Add(lblFechas);
            pnl.Controls.Add(btnEditar);
            if (btnEliminar != null) pnl.Controls.Add(btnEliminar);
            pnl.Controls.Add(linea);

            EventHandler enter = (s2, e2) => pnl.BackColor = Color.FromArgb(248, 251, 255);
            EventHandler leave = (s2, e2) => pnl.BackColor = Color.White;
            pnl.MouseEnter += enter; lblTitulo.MouseEnter += enter; lblDesc.MouseEnter += enter;
            pnl.MouseLeave += leave; lblTitulo.MouseLeave += leave; lblDesc.MouseLeave += leave;

            return pnl;
        }

        // ── ELIMINAR TAREA ────────────────────────────────────────────────────────
        private void EliminarTarea(string idPagina, string titulo,
                                   Panel panelItem, FlowLayoutPanel contenedor)
        {
            if (MessageBox.Show(
                $"¿Eliminar permanentemente:\n\"{titulo}\"?\n\nEsta acción no se puede deshacer.",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) != DialogResult.Yes) return;

            try
            {
                using (MySqlConnection conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();

                    // Obtener URL del archivo principal (para limpiar S3)
                    string urlArchivo = null;
                    string sqlUrl = @"SELECT a.URL_Archivo
                                      FROM tbl_WikiUnach_Archivos a
                                      INNER JOIN tbl_WikiUnach_PaginasWiki pw
                                              ON pw.ID_Archivo_Principal = a.ID_Archivo
                                      WHERE pw.ID_Pagina = @id";
                    using (MySqlCommand cmd = new MySqlCommand(sqlUrl, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idPagina);
                        object r = cmd.ExecuteScalar();
                        if (r != null && r != DBNull.Value) urlArchivo = r.ToString();
                    }

                    // Eliminar de BD (las FK CASCADE se encargan del resto)
                    using (MySqlCommand cmd = new MySqlCommand(
                        "DELETE FROM tbl_WikiUnach_PaginasWiki WHERE ID_Pagina = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idPagina);
                        cmd.ExecuteNonQuery();
                    }

                    // Limpiar S3 (no crítico)
                    if (!string.IsNullOrEmpty(urlArchivo))
                        try { S3Service.EliminarArchivo(new Uri(urlArchivo).AbsolutePath.TrimStart('/')); }
                        catch { }
                }

                contenedor.Controls.Remove(panelItem);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al eliminar:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
