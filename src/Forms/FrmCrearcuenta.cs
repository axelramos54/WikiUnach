using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WUNACH
{
    public partial class FrmCrearcuenta : Form
    {
        private const string ID_FACULTAD_VISITANTE = "FAC_GENERAL";

        // ── P/INVOKE — DLL verificadora en C++/ASM ────────────────────────────────
        // El archivo VerificadorAdmin.dll debe estar junto al .exe
        // Compilar VerificadorAdmin.cpp como DLL en Release|Win32 (x86)
        [DllImport("VerificadorAdmin.dll",
                   CallingConvention = CallingConvention.Cdecl,
                   CharSet = CharSet.Ansi)]
        private static extern bool VerificarCodigoAdmin(string codigo);

        // ── ESTADO DE LA SECUENCIA SECRETA ────────────────────────────────────────
        private int  _clicksButton1           = 0;
        private bool _esperandoSecuencia      = false;
        private bool _xPresionado             = false;
        private bool _modoAdmin               = false;

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
            public string Valor { get; set; }
            public override string ToString() => "Semestre " + Valor;
        }

        public FrmCrearcuenta()
        {
            InitializeComponent();

            // Evento de cambio de rol
            cmbrol.SelectedIndexChanged += CmbRol_Changed;

            // Cascada Facultad → Licenciatura → Semestre
            cmbFacultad.SelectedIndexChanged    += CmbFacultad_Changed;
            cmbLicenciatura.SelectedIndexChanged += CmbLicenciatura_Changed;

            // ── Secuencia secreta de administrador ────────────────────────────────
            // button1 ya existe en el Designer (el logo/ícono en el header)
            button1.Click += Button1_SecretoClick;
            this.KeyPreview = true;
            this.KeyDown   += FrmCrearcuenta_KeyDown;

            this.Load += FrmCrearcuenta_Load;
            this.Load += (s, e) =>
            {
                this.Text = "WUNACH  •  Crear Cuenta";
                LayoutHelper.AplicarIcono(this);
                LayoutHelper.EnterPasaAlSiguiente(txtnombre, txtmatricula, txtcorreo, txtcontraseña);
                Tema.AplicarA(this);
            };

            // Bloquear dígitos en el campo Nombre en tiempo real
            txtnombre.KeyPress += (s, e) =>
            {
                if (char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;   // ignorar la tecla
                    System.Media.SystemSounds.Beep.Play();
                }
            };

            this.MinimumSize = new System.Drawing.Size(870, 640);
        }

        // ── SECUENCIA SECRETA: button1 × 3 + Ctrl+Alt+Shift+X → D ───────────────
        private void Button1_SecretoClick(object sender, EventArgs e)
        {
            if (_modoAdmin) return;  // ya está activado

            _clicksButton1++;
            if (_clicksButton1 >= 3)
            {
                _clicksButton1    = 0;
                _esperandoSecuencia = true;
            }
        }

        private void FrmCrearcuenta_KeyDown(object sender, KeyEventArgs e)
        {
            if (!_esperandoSecuencia || _modoAdmin) return;

            bool modificadores = e.Control && e.Alt && e.Shift;

            if (modificadores && e.KeyCode == Keys.X)
            {
                _xPresionado = true;
                e.Handled    = true;
                return;
            }

            if (modificadores && e.KeyCode == Keys.D && _xPresionado)
            {
                _xPresionado        = false;
                _esperandoSecuencia = false;
                e.Handled           = true;
                MostrarDialogoCodigoAdmin();
                return;
            }

            // Cualquier otra tecla que no sea un modificador solo resetea la X
            if (!modificadores)
                _xPresionado = false;
        }

        // ── DIÁLOGO FLOTANTE DE CÓDIGO ADMIN ─────────────────────────────────────
        private void MostrarDialogoCodigoAdmin()
        {
            Form dialogo = new Form
            {
                Text              = "Verificación de Administrador",
                Size              = new Size(340, 175),
                FormBorderStyle   = FormBorderStyle.FixedDialog,
                StartPosition     = FormStartPosition.CenterParent,
                MaximizeBox       = false,
                MinimizeBox       = false,
                ShowInTaskbar     = false,
                TopMost           = true,
                BackColor         = Color.White
            };

            Label lblTitulo = new Label
            {
                Text      = "🔐  Ingresa el código de acceso:",
                Location  = new Point(20, 18),
                AutoSize  = true,
                Font      = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 80)
            };

            TextBox txtCodigo = new TextBox
            {
                Location     = new Point(20, 50),
                Size         = new Size(290, 28),
                Font         = new Font("Segoe UI", 12),
                PasswordChar = '●',
                MaxLength    = 10,
                TextAlign    = HorizontalAlignment.Center
            };

            Label lblError = new Label
            {
                Text      = "",
                Location  = new Point(20, 84),
                AutoSize  = true,
                Font      = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(200, 50, 50),
                Visible   = false
            };

            Button btnVerificar = new Button
            {
                Text      = "Verificar",
                Location  = new Point(130, 105),
                Size      = new Size(90, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(30, 80, 160),
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnVerificar.FlatAppearance.BorderSize = 0;

            Button btnCancelar = new Button
            {
                Text      = "Cancelar",
                Location  = new Point(228, 105),
                Size      = new Size(80, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(240, 240, 240),
                ForeColor = Color.FromArgb(80, 80, 80),
                Font      = new Font("Segoe UI", 9),
                Cursor    = Cursors.Hand
            };
            btnCancelar.FlatAppearance.BorderSize = 1;
            btnCancelar.Click += (s, ev) => dialogo.Close();

            btnVerificar.Click += (s, ev) =>
            {
                string codigo = txtCodigo.Text.Trim();

                bool correcto = false;
                try
                {
                    correcto = VerificarCodigoAdmin(codigo);
                }
                catch (DllNotFoundException)
                {
                    MessageBox.Show(
                        "No se encontró VerificadorAdmin.dll.\n" +
                        "Asegúrate de que esté en la misma carpeta que el ejecutable.",
                        "DLL no encontrada",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    dialogo.Close();
                    return;
                }

                if (correcto)
                {
                    dialogo.Close();
                    ActivarModoAdmin();
                }
                else
                {
                    lblError.Text    = "Código incorrecto. Intenta de nuevo.";
                    lblError.Visible = true;
                    txtCodigo.Clear();
                    txtCodigo.Focus();
                }
            };

            // Enter en el TextBox = clic en Verificar
            txtCodigo.KeyDown += (s, ev) =>
            {
                if (ev.KeyCode == Keys.Enter) { btnVerificar.PerformClick(); ev.SuppressKeyPress = true; }
            };

            dialogo.Controls.Add(lblTitulo);
            dialogo.Controls.Add(txtCodigo);
            dialogo.Controls.Add(lblError);
            dialogo.Controls.Add(btnVerificar);
            dialogo.Controls.Add(btnCancelar);
            dialogo.AcceptButton = btnVerificar;

            dialogo.ShowDialog(this);
        }

        // ── ACTIVAR MODO ADMINISTRADOR ────────────────────────────────────────────
        private void ActivarModoAdmin()
        {
            _modoAdmin = true;

            // Ocultar selector de rol (se asignará ADMINISTRADOR automáticamente)
            lblrol.Visible  = false;
            cmbrol.Visible  = false;

            // Ocultar matrícula (admin no la usa)
            lblmatricula.Visible = false;
            txtmatricula.Visible = false;

            // Ocultar campos de ubicación
            lblFacultad.Visible     = false;
            cmbFacultad.Visible     = false;
            OcultarLicenciaturaYSemestre();

            // Etiqueta visual de modo admin
            Label lblModoAdmin = new Label
            {
                Text      = "🔐  Creando cuenta de Administrador",
                Location  = new Point(lblrol.Left, lblrol.Top),
                AutoSize  = true,
                Font      = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 80, 160)
            };
            this.Controls.Add(lblModoAdmin);
            lblModoAdmin.BringToFront();
        }

        private void FrmCrearcuenta_Load(object sender, EventArgs e)
        {
            // Estado inicial: deshabilitar todo hasta que se elija rol
            cmbFacultad.DropDownStyle    = ComboBoxStyle.DropDownList;
            cmbLicenciatura.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSemestre.DropDownStyle    = ComboBoxStyle.DropDownList;

            cmbFacultad.Enabled    = false;
            cmbLicenciatura.Enabled = false;
            cmbSemestre.Enabled    = false;

            // Ocultar licenciatura y semestre hasta que se elija ALUMNO
            OcultarLicenciaturaYSemestre();

            CargarFacultades();
        }

        // ── VISIBILIDAD DE CONTROLES ──────────────────────────────────────────────
        private void OcultarLicenciaturaYSemestre()
        {
            lblLicenciatura.Visible  = false;
            cmbLicenciatura.Visible  = false;
            lblSemestre.Visible      = false;
            cmbSemestre.Visible      = false;
        }

        private void MostrarLicenciaturaYSemestre()
        {
            lblLicenciatura.Visible  = true;
            cmbLicenciatura.Visible  = true;
            lblSemestre.Visible      = true;
            cmbSemestre.Visible      = true;
        }

        // ── CARGA DE FACULTADES ───────────────────────────────────────────────────
        private void CargarFacultades()
        {
            cmbFacultad.Items.Clear();
            cmbFacultad.Items.Add("── Selecciona una Facultad ──");
            cmbFacultad.SelectedIndex = 0;

            try
            {
                using (MySqlConnection conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();
                    string sql = @"SELECT ID_Facultad, Nombre_Facultad
                                   FROM tbl_WikiUnach_Facultades
                                   WHERE ID_Facultad != @general
                                   ORDER BY Nombre_Facultad";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@general", ID_FACULTAD_VISITANTE);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                            while (reader.Read())
                                cmbFacultad.Items.Add(new FacultadItem
                                {
                                    ID     = reader["ID_Facultad"].ToString(),
                                    Nombre = reader["Nombre_Facultad"].ToString()
                                });
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al cargar facultades:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── CARGA DE LICENCIATURAS ────────────────────────────────────────────────
        private void CargarLicenciaturas(string idFacultad)
        {
            cmbLicenciatura.Items.Clear();
            cmbLicenciatura.Items.Add("── Selecciona una Licenciatura ──");
            cmbLicenciatura.SelectedIndex = 0;

            // Resetear semestre
            cmbSemestre.Items.Clear();
            cmbSemestre.Items.Add("── Elige Licenciatura primero ──");
            cmbSemestre.SelectedIndex = 0;
            cmbSemestre.Enabled = false;

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

            cmbLicenciatura.Enabled = true;
        }

        // ── CARGA DE SEMESTRES ────────────────────────────────────────────────────
        private void CargarSemestres(string idLicenciatura)
        {
            cmbSemestre.Items.Clear();
            cmbSemestre.Items.Add("── Selecciona un Semestre ──");
            cmbSemestre.SelectedIndex = 0;

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

            cmbSemestre.Enabled = true;
        }

        // ── EVENTOS EN CASCADA ────────────────────────────────────────────────────
        private void CmbFacultad_Changed(object sender, EventArgs e)
        {
            // Solo carga licenciatura si el rol es ALUMNO
            if (cmbrol.SelectedItem?.ToString() != "ALUMNO") return;

            if (cmbFacultad.SelectedItem is FacultadItem fi)
                CargarLicenciaturas(fi.ID);
            else
            {
                cmbLicenciatura.Items.Clear();
                cmbLicenciatura.Items.Add("── Selecciona una Facultad primero ──");
                cmbLicenciatura.SelectedIndex = 0;
                cmbLicenciatura.Enabled = false;
                cmbSemestre.Items.Clear();
                cmbSemestre.Items.Add("── Elige Licenciatura primero ──");
                cmbSemestre.SelectedIndex = 0;
                cmbSemestre.Enabled = false;
            }
        }

        private void CmbLicenciatura_Changed(object sender, EventArgs e)
        {
            if (cmbLicenciatura.SelectedItem is LicenciaturaItem li)
                CargarSemestres(li.ID);
            else
            {
                cmbSemestre.Items.Clear();
                cmbSemestre.Items.Add("── Elige Licenciatura primero ──");
                cmbSemestre.SelectedIndex = 0;
                cmbSemestre.Enabled = false;
            }
        }

        // ── CAMBIO DE ROL ─────────────────────────────────────────────────────────
        private void CmbRol_Changed(object sender, EventArgs e)
        {
            string rol = cmbrol.SelectedItem?.ToString() ?? "";

            switch (rol)
            {
                case "ALUMNO":
                    // Facultad + Licenciatura + Semestre habilitados
                    cmbFacultad.Enabled = true;
                    MostrarLicenciaturaYSemestre();
                    // Resetear downstream
                    cmbLicenciatura.Items.Clear();
                    cmbLicenciatura.Items.Add("── Selecciona una Facultad primero ──");
                    cmbLicenciatura.SelectedIndex = 0;
                    cmbLicenciatura.Enabled = false;
                    cmbSemestre.Items.Clear();
                    cmbSemestre.Items.Add("── Elige Licenciatura primero ──");
                    cmbSemestre.SelectedIndex = 0;
                    cmbSemestre.Enabled = false;
                    // Resetear selección de Facultad para forzar elección explícita
                    cmbFacultad.SelectedIndex = 0;
                    break;

                case "MAESTRO":
                case "ADMINISTRADOR":
                    // Solo Facultad
                    cmbFacultad.Enabled = true;
                    OcultarLicenciaturaYSemestre();
                    cmbFacultad.SelectedIndex = 0;
                    break;

                case "VISITANTE":
                    // Sin campos de ubicación (FAC_GENERAL se asigna automáticamente)
                    cmbFacultad.Enabled = false;
                    cmbFacultad.SelectedIndex = 0;
                    OcultarLicenciaturaYSemestre();
                    break;
            }
        }

        // ── CREAR CUENTA ──────────────────────────────────────────────────────────
        private void btncrear_Click(object sender, EventArgs e)
        {
            // ── 1. VALIDAR CAMPOS OBLIGATORIOS ────────────────────────────────────
            if (string.IsNullOrWhiteSpace(txtnombre.Text))
            { MostrarError("El nombre de usuario es obligatorio."); txtnombre.Focus(); return; }

            // El nombre no debe contener números
            if (txtnombre.Text.IndexOfAny("0123456789".ToCharArray()) >= 0)
            {
                MostrarError("El nombre no debe contener números.");
                txtnombre.Focus();
                txtnombre.SelectAll();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtcorreo.Text) ||
                !txtcorreo.Text.Contains("@") || !txtcorreo.Text.Contains("."))
            { MostrarError("Ingresa un correo electrónico válido."); txtcorreo.Focus(); return; }

            if (string.IsNullOrWhiteSpace(txtcontraseña.Text) || txtcontraseña.Text.Length < 6)
            { MostrarError("La contraseña debe tener al menos 6 caracteres."); txtcontraseña.Focus(); return; }

            // La validación de rol se hace dentro del bloque siguiente
            // ── Determinar rol ────────────────────────────────────────────────────
            string rol;
            if (_modoAdmin)
            {
                rol = "ADMINISTRADOR";
            }
            else
            {
                if (cmbrol.SelectedIndex == -1 || string.IsNullOrEmpty(cmbrol.SelectedItem?.ToString()))
                { MostrarError("Debes seleccionar un rol."); cmbrol.Focus(); return; }
                rol = cmbrol.SelectedItem.ToString();
            }

            bool esAlumno    = rol.Equals("ALUMNO",         StringComparison.OrdinalIgnoreCase);
            bool esVisitante = rol.Equals("VISITANTE",      StringComparison.OrdinalIgnoreCase);
            bool esAdmin     = rol.Equals("ADMINISTRADOR",  StringComparison.OrdinalIgnoreCase);

            // Validar facultad (solo para MAESTRO y roles que lo requieran)
            if (!esVisitante && !esAdmin && !(cmbFacultad.SelectedItem is FacultadItem))
            { MostrarError("Debes seleccionar una facultad."); cmbFacultad.Focus(); return; }

            // Validar licenciatura y semestre solo para ALUMNO
            if (esAlumno)
            {
                if (!(cmbLicenciatura.SelectedItem is LicenciaturaItem))
                { MostrarError("Debes seleccionar una licenciatura."); cmbLicenciatura.Focus(); return; }

                if (!(cmbSemestre.SelectedItem is SemestreItem))
                { MostrarError("Debes seleccionar tu semestre."); cmbSemestre.Focus(); return; }
            }

            // ── 2. PREPARAR DATOS ─────────────────────────────────────────────────
            string nuevoId    = Guid.NewGuid().ToString();
            string nombre     = txtnombre.Text.Trim();
            string correo     = txtcorreo.Text.Trim().ToLower();
            string hash       = HashearContrasena(txtcontraseña.Text);
            // Admin y Visitante usan FAC_GENERAL; otros usan la facultad elegida
            string idFacultad = (esVisitante || esAdmin)
                                ? ID_FACULTAD_VISITANTE
                                : ((FacultadItem)cmbFacultad.SelectedItem).ID;

            string idLicenciatura = esAlumno
                                    ? ((LicenciaturaItem)cmbLicenciatura.SelectedItem).ID
                                    : null;
            string semestre       = esAlumno
                                    ? ((SemestreItem)cmbSemestre.SelectedItem).Valor
                                    : null;
            string matricula      = string.IsNullOrWhiteSpace(txtmatricula.Text)
                                    ? null
                                    : txtmatricula.Text.Trim().ToUpper();

            // ── 3. BD ─────────────────────────────────────────────────────────────
            try
            {
                using (MySqlConnection conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();

                    if (ExisteValor(conn, "Correo", correo))
                    { MostrarError("Este correo ya está registrado."); txtcorreo.Focus(); return; }

                    if (matricula != null && ExisteValor(conn, "Matricula", matricula))
                    { MostrarError("Esta matrícula ya está registrada."); txtmatricula.Focus(); return; }
                }

                // ── 3.5 VERIFICAR CORREO POR CÓDIGO ────────────────────────────────
                if (!VerificarCorreoConCodigo(correo))
                    return;   // el usuario canceló o falló la verificación

                using (MySqlConnection conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();

                    string sql = @"
                        INSERT INTO tbl_WikiUnach_Usuarios
                            (ID_Usuario, ID_Facultad, ID_Licenciatura, Semestre,
                             Matricula, Nombre, Correo, Contrasena_Hash, Rol)
                        VALUES
                            (@id, @facultad, @licenciatura, @semestre,
                             @matricula, @nombre, @correo, @hash, @rol)";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id",      nuevoId);
                        cmd.Parameters.AddWithValue("@facultad", idFacultad);
                        cmd.Parameters.AddWithValue("@nombre",  nombre);
                        cmd.Parameters.AddWithValue("@correo",  correo);
                        cmd.Parameters.AddWithValue("@hash",    hash);
                        cmd.Parameters.AddWithValue("@rol",     rol);

                        // ID_Licenciatura (null para no-alumnos)
                        if (idLicenciatura != null)
                            cmd.Parameters.AddWithValue("@licenciatura", idLicenciatura);
                        else
                            cmd.Parameters.Add("@licenciatura", MySqlDbType.VarChar).Value = DBNull.Value;

                        // Semestre (null para no-alumnos)
                        if (semestre != null)
                            cmd.Parameters.AddWithValue("@semestre", semestre);
                        else
                            cmd.Parameters.Add("@semestre", MySqlDbType.VarChar).Value = DBNull.Value;

                        // Matricula (opcional)
                        if (matricula != null)
                            cmd.Parameters.AddWithValue("@matricula", matricula);
                        else
                            cmd.Parameters.Add("@matricula", MySqlDbType.VarChar).Value = DBNull.Value;

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("¡Cuenta creada exitosamente!\nYa puedes iniciar sesión.",
                    "Registro exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                FrmAcceso frm = new FrmAcceso();
                this.Hide();
                frm.Show();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al conectar con la base de datos:\n" + ex.Message,
                    "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error inesperado:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── VERIFICACIÓN DE CORREO POR CÓDIGO ─────────────────────────────────────
        /// <summary>
        /// Genera un código, lo envía al correo y abre un diálogo flotante para
        /// que el usuario lo introduzca. Devuelve true si el código es correcto.
        /// </summary>
        private bool VerificarCorreoConCodigo(string correo)
        {
            string codigoEsperado = EmailService.GenerarCodigo();
            DateTime expira       = DateTime.Now.AddMinutes(10);

            // 1) Enviar el correo
            try
            {
                Cursor = Cursors.WaitCursor;
                EmailService.EnviarCodigoVerificacion(correo, codigoEsperado);
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show(
                    "No se pudo enviar el correo de verificación:\n" + ex.Message +
                    "\n\nVerifica la configuración SMTP en App.config y tu conexión a Internet.",
                    "Error de envío", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            Cursor = Cursors.Default;

            // 2) Mostrar diálogo
            Form dlg = new Form
            {
                Text            = "Verificación de correo",
                Size            = new Size(420, 230),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition   = FormStartPosition.CenterParent,
                MaximizeBox     = false,
                MinimizeBox     = false,
                ShowInTaskbar   = false,
                BackColor       = Color.White
            };

            Label lblTitulo = new Label
            {
                Text     = "📧  Te enviamos un código a:",
                Location = new Point(20, 18),
                AutoSize = true,
                Font     = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            Label lblCorreo = new Label
            {
                Text      = correo,
                Location  = new Point(20, 42),
                AutoSize  = true,
                Font      = new Font("Segoe UI", 9.5f, FontStyle.Italic),
                ForeColor = Color.FromArgb(80, 80, 130)
            };

            TextBox txtCodigo = new TextBox
            {
                Location  = new Point(20, 75),
                Size      = new Size(370, 30),
                Font      = new Font("Consolas", 16, FontStyle.Bold),
                MaxLength = 6,
                TextAlign = HorizontalAlignment.Center
            };

            Label lblError = new Label
            {
                Text      = "",
                Location  = new Point(20, 110),
                AutoSize  = true,
                Font      = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(200, 50, 50),
                Visible   = false
            };

            Button btnVerificar = new Button
            {
                Text      = "Verificar",
                Location  = new Point(180, 145),
                Size      = new Size(100, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(30, 100, 210),
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnVerificar.FlatAppearance.BorderSize = 0;

            Button btnReenviar = new Button
            {
                Text      = "Reenviar",
                Location  = new Point(20, 145),
                Size      = new Size(90, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(240, 240, 240),
                Font      = new Font("Segoe UI", 8.5f),
                Cursor    = Cursors.Hand
            };

            Button btnCancelar = new Button
            {
                Text      = "Cancelar",
                Location  = new Point(290, 145),
                Size      = new Size(100, 32),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(240, 240, 240),
                Font      = new Font("Segoe UI", 9),
                Cursor    = Cursors.Hand
            };

            int intentos = 0;
            bool aprobado = false;

            btnVerificar.Click += (s, ev) =>
            {
                string ingresado = txtCodigo.Text.Trim();
                if (DateTime.Now > expira)
                {
                    lblError.Text    = "El código expiró. Solicita uno nuevo.";
                    lblError.Visible = true;
                    return;
                }
                if (ingresado == codigoEsperado)
                {
                    aprobado = true;
                    dlg.Close();
                    return;
                }
                intentos++;
                lblError.Text    = $"Código incorrecto ({intentos}/5).";
                lblError.Visible = true;
                txtCodigo.Clear();
                txtCodigo.Focus();
                if (intentos >= 5)
                {
                    MessageBox.Show("Demasiados intentos fallidos.", "Cancelado",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dlg.Close();
                }
            };

            btnReenviar.Click += (s, ev) =>
            {
                try
                {
                    btnReenviar.Enabled = false;
                    Cursor = Cursors.WaitCursor;
                    codigoEsperado = EmailService.GenerarCodigo();
                    expira         = DateTime.Now.AddMinutes(10);
                    EmailService.EnviarCodigoVerificacion(correo, codigoEsperado);
                    lblError.Text      = "Nuevo código enviado.";
                    lblError.ForeColor = Color.FromArgb(40, 130, 60);
                    lblError.Visible   = true;
                    txtCodigo.Clear();
                    txtCodigo.Focus();
                }
                catch (Exception ex)
                {
                    lblError.Text    = "Error al reenviar: " + ex.Message;
                    lblError.ForeColor = Color.FromArgb(200, 50, 50);
                    lblError.Visible = true;
                }
                finally
                {
                    Cursor = Cursors.Default;
                    btnReenviar.Enabled = true;
                }
            };

            btnCancelar.Click += (s, ev) => dlg.Close();
            txtCodigo.KeyDown += (s, ev) =>
            {
                if (ev.KeyCode == Keys.Enter) { btnVerificar.PerformClick(); ev.SuppressKeyPress = true; }
            };

            dlg.Controls.Add(lblTitulo);
            dlg.Controls.Add(lblCorreo);
            dlg.Controls.Add(txtCodigo);
            dlg.Controls.Add(lblError);
            dlg.Controls.Add(btnVerificar);
            dlg.Controls.Add(btnReenviar);
            dlg.Controls.Add(btnCancelar);
            dlg.AcceptButton = btnVerificar;
            dlg.ShowDialog(this);

            return aprobado;
        }

        // ── AUXILIARES ────────────────────────────────────────────────────────────
        private bool ExisteValor(MySqlConnection conn, string columna, string valor)
        {
            string sql = $"SELECT COUNT(*) FROM tbl_WikiUnach_Usuarios WHERE {columna} = @valor";
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@valor", valor);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private string HashearContrasena(string contrasena)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(contrasena);
                byte[] hash  = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        private void MostrarError(string mensaje)
        {
            MessageBox.Show(mensaje, "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
