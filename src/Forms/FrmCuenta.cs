using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WUNACH
{
    public partial class FrmCuenta : Form
    {
        private bool _modoEdicion = false;

        // ── MODO "VER PERFIL DE OTRO USUARIO" ───────────────────────────────────
        // Si _idUsuarioVisto es null → modo normal (mi propia cuenta)
        // Si tiene un ID → estamos viendo el perfil de OTRO usuario (read-only)
        private readonly string _idUsuarioVisto;
        private bool _bloqueadaViewer = false;   // estado bloqueada del usuario visto
        private string _nombreViewer = "";       // nombre del usuario visto

        // Constructor por defecto: muestra el perfil del usuario en sesión
        public FrmCuenta() : this(null) { }

        // Constructor para ver el perfil de OTRO usuario por ID
        public FrmCuenta(string idUsuarioAVer)
        {
            InitializeComponent();
            _idUsuarioVisto = idUsuarioAVer;

            btneditar.Click            += BtnEditar_Click;
            btnAdministrarTareas.Click += BtnAdministrarTareas_Click;
            btnregreso.Click           += btnregreso_Click;

            // Matrícula solo se habilita cuando hay un rol seleccionado
            cmbrol.SelectedIndexChanged += CmbRol_Changed;

            this.Load += FrmCuenta_Load;
            this.Load += (s, e) =>
            {
                AgregarSelectorTema();
                AnclarControlesIngreso();
                LayoutHelper.AplicarTitulo(this, _idUsuarioVisto != null ? "Perfil" : "Mi Cuenta");
                LayoutHelper.EnterPasaAlSiguiente(txtnombre, txtcorreo, txtmatricula);
                Tema.AplicarA(this);
            };

            // Bloquear dígitos en el campo Nombre (consistencia con el registro)
            txtnombre.KeyPress += (s, e) =>
            {
                if (char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                    System.Media.SystemSounds.Beep.Play();
                }
            };

            this.MinimumSize = new System.Drawing.Size(900, 600);
        }

        // ── LAYOUT RESPONSIVO ─────────────────────────────────────────────────────
        private void AnclarControlesIngreso()
        {
            // pnlIngreso ocupa toda el área debajo del header
            pnlIngreso.Dock = DockStyle.Fill;
            pnlTitulo.SendToBack();
            pnlTitulo.BringToFront();   // header arriba
            pnlIngreso.BringToFront();  // contenido llena el resto

            // Anclar campos de texto (se estiran horizontalmente)
            txtnombre.Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtcorreo.Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtmatricula.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            cmbrol.Anchor       = AnchorStyles.Top | AnchorStyles.Left;
            textBox1.Anchor     = AnchorStyles.Top | AnchorStyles.Left;

            // Botones inferiores: anclados a la parte de abajo
            btneditar.Anchor            = AnchorStyles.Bottom | AnchorStyles.Left;
            btnregreso.Anchor           = AnchorStyles.Bottom | AnchorStyles.Left;
            btnAdministrarTareas.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        }

        // ── SELECTOR DE TEMA ─────────────────────────────────────────────────────
        private void AgregarSelectorTema()
        {
            // Solo en modo "mi cuenta" (no cuando se ve el perfil de otro)
            if (_idUsuarioVisto != null) return;

            Label lblTema = new Label
            {
                Text     = "Tema:",
                Location = new Point(540, 130),
                AutoSize = true,
                Font     = new Font("Segoe UI Semibold", 11, FontStyle.Bold)
            };

            ComboBox cmbTema = new ComboBox
            {
                Location      = new Point(540, 165),
                Size          = new Size(260, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font          = new Font("Segoe UI", 10)
            };

            // Items según el rol
            cmbTema.Items.Add(Tema.NombreLegible(Tema.Tipo.Claro));
            cmbTema.Items.Add(Tema.NombreLegible(Tema.Tipo.Oscuro));
            cmbTema.Items.Add(Tema.NombreLegible(Tema.Tipo.Forest));
            cmbTema.Items.Add(Tema.NombreLegible(Tema.Tipo.Ocean));
            if (SesionUsuario.Rol == "ADMINISTRADOR")
                cmbTema.Items.Add(Tema.NombreLegible(Tema.Tipo.TripleBaka));

            // Seleccionar el actual
            string actualNombre = Tema.NombreLegible(Tema.Actual);
            int idx = cmbTema.Items.IndexOf(actualNombre);
            cmbTema.SelectedIndex = idx >= 0 ? idx : 0;

            cmbTema.SelectedIndexChanged += (s, ev) =>
            {
                string sel = cmbTema.SelectedItem.ToString();
                if      (sel == Tema.NombreLegible(Tema.Tipo.Claro))      Tema.Actual = Tema.Tipo.Claro;
                else if (sel == Tema.NombreLegible(Tema.Tipo.Oscuro))     Tema.Actual = Tema.Tipo.Oscuro;
                else if (sel == Tema.NombreLegible(Tema.Tipo.Forest))     Tema.Actual = Tema.Tipo.Forest;
                else if (sel == Tema.NombreLegible(Tema.Tipo.Ocean))      Tema.Actual = Tema.Tipo.Ocean;
                else if (sel == Tema.NombreLegible(Tema.Tipo.TripleBaka)) Tema.Actual = Tema.Tipo.TripleBaka;

                Tema.AplicarA(this);
            };

            this.Controls.Add(lblTema);
            this.Controls.Add(cmbTema);
            lblTema.BringToFront();
            cmbTema.BringToFront();
        }

        // ── CARGA DE DATOS ────────────────────────────────────────────────────────
        private void FrmCuenta_Load(object sender, EventArgs e)
        {
            // Items del rol (siempre los mismos)
            cmbrol.Items.Clear();
            cmbrol.Items.AddRange(new object[] { "ESTUDIANTE", "DOCENTE", "VISITANTE", "ADMINISTRADOR", "MAESTRO", "ALUMNO" });

            if (_idUsuarioVisto == null)
            {
                // ── Modo "MI CUENTA" ──────────────────────────────────────────────
                txtmatricula.Text = SesionUsuario.Matricula ?? "";
                txtnombre.Text    = SesionUsuario.Nombre    ?? "";
                txtcorreo.Text    = SesionUsuario.Correo    ?? "";
                cmbrol.SelectedItem = SesionUsuario.Rol ?? "";

                CargarFechaRegistro(SesionUsuario.ID_Usuario);
                SetModoLectura();
            }
            else
            {
                // ── Modo "VER OTRO PERFIL" ────────────────────────────────────────
                CargarPerfilOtroUsuario();
            }
        }

        /// <summary>Carga los datos del usuario indicado por _idUsuarioVisto.</summary>
        private void CargarPerfilOtroUsuario()
        {
            try
            {
                using (MySqlConnection conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();
                    string sql = @"SELECT Nombre, Correo, Rol, Matricula,
                                          Fecha_Registro, Cuenta_Bloqueada
                                   FROM tbl_WikiUnach_Usuarios
                                   WHERE ID_Usuario = @id";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", _idUsuarioVisto);
                        using (MySqlDataReader r = cmd.ExecuteReader())
                        {
                            if (r.Read())
                            {
                                _nombreViewer    = r["Nombre"].ToString();
                                txtnombre.Text   = _nombreViewer;
                                txtcorreo.Text   = r["Correo"].ToString();
                                cmbrol.SelectedItem = r["Rol"].ToString();
                                txtmatricula.Text = r["Matricula"] == DBNull.Value
                                                    ? "" : r["Matricula"].ToString();
                                textBox1.Text    = r["Fecha_Registro"] == DBNull.Value
                                                    ? "—"
                                                    : Convert.ToDateTime(r["Fecha_Registro"]).ToString("dd/MM/yyyy");
                                _bloqueadaViewer = Convert.ToInt32(r["Cuenta_Bloqueada"]) == 1;
                            }
                            else
                            {
                                MessageBox.Show("Usuario no encontrado.",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                this.Close();
                                return;
                            }
                        }
                    }
                }

                // Todo en read-only
                SetModoLectura();

                // Adaptar UI según quién está viendo
                lbldatosp.Text = "Perfil de " + _nombreViewer + " 👤";
                this.Text      = "Perfil — " + _nombreViewer;

                bool soyAdmin   = SesionUsuario.Rol == "ADMINISTRADOR";
                bool esMiPerfil = _idUsuarioVisto == SesionUsuario.ID_Usuario;

                if (soyAdmin && !esMiPerfil)
                {
                    // Repurpose: btneditar → Bloquear/Desbloquear; btnAdministrarTareas → Eliminar
                    btneditar.Text       = _bloqueadaViewer ? "🔓 DESBLOQUEAR" : "🔒 BLOQUEAR";
                    btneditar.BackColor  = Color.FromArgb(255, 235, 200);
                    btneditar.ForeColor  = Color.FromArgb(150, 80, 0);

                    btnAdministrarTareas.Text      = "🗑 ELIMINAR CUENTA";
                    btnAdministrarTareas.BackColor = Color.FromArgb(255, 230, 230);
                    btnAdministrarTareas.ForeColor = Color.FromArgb(180, 30, 30);
                }
                else
                {
                    // Sin permisos de admin: ocultar botones de acción
                    btneditar.Visible            = false;
                    btnAdministrarTareas.Visible = false;
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al cargar perfil:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void CargarFechaRegistro(string idUsuario)
        {
            try
            {
                using (MySqlConnection conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();
                    string sql = @"SELECT Fecha_Registro FROM tbl_WikiUnach_Usuarios
                                   WHERE ID_Usuario = @id";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", idUsuario);
                        object res = cmd.ExecuteScalar();
                        textBox1.Text = (res != null && res != DBNull.Value)
                                        ? Convert.ToDateTime(res).ToString("dd/MM/yyyy")
                                        : "—";
                    }
                }
            }
            catch { textBox1.Text = "—"; }
        }

        // ── EVENTO ROL → habilitar matrícula ─────────────────────────────────────
        private void CmbRol_Changed(object sender, EventArgs e)
        {
            if (!_modoEdicion) return;

            bool rolSeleccionado = cmbrol.SelectedIndex >= 0;
            txtmatricula.ReadOnly  = !rolSeleccionado;
            txtmatricula.BackColor = rolSeleccionado ? Color.White : SystemColors.Control;
        }

        // ── MODO LECTURA ──────────────────────────────────────────────────────────
        private void SetModoLectura()
        {
            txtnombre.ReadOnly    = true;
            txtcorreo.ReadOnly    = true;
            txtmatricula.ReadOnly = true;
            textBox1.ReadOnly     = true;

            cmbrol.Enabled        = false;
            cmbrol.DropDownStyle  = ComboBoxStyle.DropDownList;

            Color bg = SystemColors.Control;
            txtnombre.BackColor    = bg;
            txtcorreo.BackColor    = bg;
            txtmatricula.BackColor = bg;
            textBox1.BackColor     = bg;

            if (_idUsuarioVisto == null) btneditar.Text = "EDITAR";
            _modoEdicion = false;
        }

        // ── MODO EDICIÓN ──────────────────────────────────────────────────────────
        private void SetModoEdicion()
        {
            txtnombre.ReadOnly = false;
            txtcorreo.ReadOnly = false;
            txtmatricula.ReadOnly  = true;
            txtmatricula.BackColor = SystemColors.Control;

            cmbrol.Enabled       = true;
            cmbrol.DropDownStyle = ComboBoxStyle.DropDownList;

            txtnombre.BackColor = Color.White;
            txtcorreo.BackColor = Color.White;

            btneditar.Text = "GUARDAR";
            _modoEdicion   = true;
            txtnombre.Focus();
        }

        // ── BOTÓN EDITAR / GUARDAR (o BLOQUEAR/DESBLOQUEAR si admin viendo otro) ─
        private void BtnEditar_Click(object sender, EventArgs e)
        {
            // Modo "ver otro perfil" + admin → este botón es Bloquear/Desbloquear
            if (_idUsuarioVisto != null)
            {
                BloquearODesbloquearViewer();
                return;
            }

            // Modo normal "mi cuenta"
            if (!_modoEdicion)
            {
                if (!ConfirmarContrasena()) return;
                SetModoEdicion();
                return;
            }

            // ── Validaciones ──────────────────────────────────────────────────────
            string nuevoNombre   = txtnombre.Text.Trim();
            string nuevoCorreo   = txtcorreo.Text.Trim().ToLower();
            string nuevoRol      = cmbrol.SelectedItem?.ToString() ?? "";
            string nuevaMatricula = txtmatricula.Text.Trim().ToUpper();

            if (string.IsNullOrWhiteSpace(nuevoNombre))
            { MostrarError("El nombre no puede estar vacío."); txtnombre.Focus(); return; }

            if (nuevoNombre.IndexOfAny("0123456789".ToCharArray()) >= 0)
            { MostrarError("El nombre no debe contener números."); txtnombre.Focus(); txtnombre.SelectAll(); return; }

            if (string.IsNullOrWhiteSpace(nuevoCorreo) ||
                !nuevoCorreo.Contains("@") || !nuevoCorreo.Contains("."))
            { MostrarError("Ingresa un correo electrónico válido (debe contener @ y .)."); txtcorreo.Focus(); return; }

            if (string.IsNullOrWhiteSpace(nuevoRol))
            { MostrarError("Debes seleccionar un rol."); cmbrol.Focus(); return; }

            // ── Guardar en BD ─────────────────────────────────────────────────────
            try
            {
                using (MySqlConnection conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();

                    string sqlCheck = @"SELECT COUNT(*) FROM tbl_WikiUnach_Usuarios
                                        WHERE Correo = @correo AND ID_Usuario != @id";
                    using (MySqlCommand cmd = new MySqlCommand(sqlCheck, conn))
                    {
                        cmd.Parameters.AddWithValue("@correo", nuevoCorreo);
                        cmd.Parameters.AddWithValue("@id",     SesionUsuario.ID_Usuario);
                        if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                        { MostrarError("Ese correo ya está en uso por otra cuenta."); txtcorreo.Focus(); return; }
                    }

                    if (!string.IsNullOrWhiteSpace(nuevaMatricula) &&
                        nuevaMatricula != (SesionUsuario.Matricula ?? "").ToUpper())
                    {
                        string sqlMat = @"SELECT COUNT(*) FROM tbl_WikiUnach_Usuarios
                                          WHERE Matricula = @mat AND ID_Usuario != @id";
                        using (MySqlCommand cmd = new MySqlCommand(sqlMat, conn))
                        {
                            cmd.Parameters.AddWithValue("@mat", nuevaMatricula);
                            cmd.Parameters.AddWithValue("@id",  SesionUsuario.ID_Usuario);
                            if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                            { MostrarError("Esa matrícula ya está registrada en otra cuenta."); txtmatricula.Focus(); return; }
                        }
                    }

                    string sqlUpdate = @"
                        UPDATE tbl_WikiUnach_Usuarios
                        SET    Nombre    = @nombre,
                               Correo   = @correo,
                               Rol      = @rol,
                               Matricula = @matricula
                        WHERE  ID_Usuario = @id";

                    using (MySqlCommand cmd = new MySqlCommand(sqlUpdate, conn))
                    {
                        cmd.Parameters.AddWithValue("@nombre", nuevoNombre);
                        cmd.Parameters.AddWithValue("@correo", nuevoCorreo);
                        cmd.Parameters.AddWithValue("@rol",    nuevoRol);
                        cmd.Parameters.AddWithValue("@id",     SesionUsuario.ID_Usuario);

                        if (string.IsNullOrWhiteSpace(nuevaMatricula))
                            cmd.Parameters.Add("@matricula", MySqlDbType.VarChar).Value = DBNull.Value;
                        else
                            cmd.Parameters.AddWithValue("@matricula", nuevaMatricula);

                        cmd.ExecuteNonQuery();
                    }
                }

                SesionUsuario.Nombre    = nuevoNombre;
                SesionUsuario.Correo    = nuevoCorreo;
                SesionUsuario.Rol       = nuevoRol;
                SesionUsuario.Matricula = string.IsNullOrWhiteSpace(nuevaMatricula)
                                         ? null : nuevaMatricula;

                MessageBox.Show("Datos actualizados correctamente.",
                    "Guardado", MessageBoxButtons.OK, MessageBoxIcon.Information);

                SetModoLectura();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al guardar los cambios:\n" + ex.Message,
                    "Error de BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── BOTÓN ADMINISTRAR TAREAS (o ELIMINAR CUENTA si admin viendo otro) ────
        private void BtnAdministrarTareas_Click(object sender, EventArgs e)
        {
            if (_idUsuarioVisto != null)
            {
                EliminarViewer();
                return;
            }

            new FrmAdministrarTareas().Show();
        }

        // ── ACCIONES DE ADMIN SOBRE OTRO USUARIO ─────────────────────────────────
        private void BloquearODesbloquearViewer()
        {
            string accion = _bloqueadaViewer ? "DESBLOQUEAR" : "BLOQUEAR";
            if (MessageBox.Show($"¿{accion} la cuenta de {_nombreViewer}?",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                != DialogResult.Yes) return;

            try
            {
                using (MySqlConnection conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(
                        "UPDATE tbl_WikiUnach_Usuarios SET Cuenta_Bloqueada=@b " +
                        "WHERE ID_Usuario=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@b",  _bloqueadaViewer ? 0 : 1);
                        cmd.Parameters.AddWithValue("@id", _idUsuarioVisto);
                        cmd.ExecuteNonQuery();
                    }
                }
                _bloqueadaViewer = !_bloqueadaViewer;
                btneditar.Text = _bloqueadaViewer ? "🔓 DESBLOQUEAR" : "🔒 BLOQUEAR";

                // Notificar al usuario afectado
                Notificaciones.Crear(
                    _idUsuarioVisto,
                    _bloqueadaViewer ? "BLOQUEO" : "DESBLOQUEO",
                    _bloqueadaViewer
                        ? "🔒 Un administrador bloqueó tu cuenta. Contacta al administrador para más info."
                        : "🔓 Un administrador desbloqueó tu cuenta. Ya puedes usar WUNACH normalmente.");

                MessageBox.Show("Estado actualizado correctamente.",
                    "Listo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EliminarViewer()
        {
            if (MessageBox.Show(
                $"¿ELIMINAR PERMANENTEMENTE la cuenta de {_nombreViewer}?\n\n" +
                "Esto borrará también todas sus tareas, comentarios y revisiones.\n" +
                "Esta acción NO se puede deshacer.",
                "⚠ Eliminación permanente",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            try
            {
                using (MySqlConnection conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();
                    using (MySqlCommand cmd = new MySqlCommand(
                        "DELETE FROM tbl_WikiUnach_Usuarios WHERE ID_Usuario=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", _idUsuarioVisto);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Cuenta eliminada correctamente.",
                    "Listo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al eliminar:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ── CONFIRMACIÓN DE CONTRASEÑA (antes de activar edición) ─────────────────
        private bool ConfirmarContrasena()
        {
            Form dlg = new Form
            {
                Text            = "Confirmar identidad",
                Size            = new Size(380, 180),
                StartPosition   = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox     = false,
                MinimizeBox     = false
            };

            Label lbl = new Label
            {
                Text     = "Ingresa tu contraseña para continuar:",
                Location = new Point(20, 20),
                Size     = new Size(330, 22),
                Font     = new Font("Segoe UI", 9.5f)
            };

            TextBox txtPass = new TextBox
            {
                Location     = new Point(20, 50),
                Size         = new Size(330, 26),
                PasswordChar = '●',
                Font         = new Font("Segoe UI", 10f)
            };

            Button btnOk = new Button
            {
                Text         = "Confirmar",
                DialogResult = DialogResult.OK,
                Location     = new Point(140, 90),
                Size         = new Size(100, 32),
                BackColor    = Color.FromArgb(30, 100, 210),
                ForeColor    = Color.White,
                FlatStyle    = FlatStyle.Flat,
                Font         = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnOk.FlatAppearance.BorderSize = 0;

            Button btnCancelar = new Button
            {
                Text         = "Cancelar",
                DialogResult = DialogResult.Cancel,
                Location     = new Point(250, 90),
                Size         = new Size(100, 32),
                FlatStyle    = FlatStyle.Flat,
                Font         = new Font("Segoe UI", 9)
            };

            dlg.Controls.AddRange(new Control[] { lbl, txtPass, btnOk, btnCancelar });
            dlg.AcceptButton = btnOk;
            dlg.CancelButton = btnCancelar;

            txtPass.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) dlg.DialogResult = DialogResult.OK;
            };

            if (dlg.ShowDialog(this) != DialogResult.OK) return false;

            string contrasenaIngresada = txtPass.Text;
            if (string.IsNullOrEmpty(contrasenaIngresada))
            { MostrarError("Debes ingresar tu contraseña."); return false; }

            try
            {
                string hashIngresado = HashearContrasena(contrasenaIngresada);
                using (MySqlConnection conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();
                    string sql = @"SELECT COUNT(*) FROM tbl_WikiUnach_Usuarios
                                   WHERE ID_Usuario      = @id
                                     AND Contrasena_Hash = @hash";
                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@id",   SesionUsuario.ID_Usuario);
                        cmd.Parameters.AddWithValue("@hash", hashIngresado);
                        bool ok = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                        if (!ok) MostrarError("Contraseña incorrecta.");
                        return ok;
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al verificar la contraseña:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        // ── AUXILIARES ────────────────────────────────────────────────────────────
        private string HashearContrasena(string contrasena)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(contrasena);
                byte[] hash  = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        private void MostrarError(string mensaje) =>
            MessageBox.Show(mensaje, "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        // ── NAVEGACIÓN ────────────────────────────────────────────────────────────
        private void btnregreso_Click(object sender, EventArgs e) => this.Close();
    }
}
