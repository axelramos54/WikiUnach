using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace WUNACH
{
    public partial class FrmAcceso : Form
    {
        // ── P/INVOKE — mismo DLL que FrmCrearcuenta ───────────────────────────────
        [DllImport("VerificadorAdmin.dll",
                   CallingConvention = CallingConvention.Cdecl,
                   CharSet = CharSet.Ansi)]
        private static extern bool VerificarCodigoAdmin(string codigo);

        // ── ESTADO DE LA SECUENCIA SECRETA ────────────────────────────────────────
        private int  _clicksButton1      = 0;
        private bool _esperandoSecuencia = false;
        private bool _xPresionado        = false;
        private bool _modoAdmin          = false;

        public FrmAcceso()
        {
            InitializeComponent();
            txtcontraseña.PasswordChar = '*';

            // Conectar evento de cambio de rol
            cmbrol.SelectedIndexChanged += CmbRol_SelectedIndexChanged;

            // ── Secuencia secreta de administrador ────────────────────────────────
            button1.Click  += Button1_SecretoClick;
            this.KeyPreview = true;
            this.KeyDown   += FrmAcceso_KeyDown;

            this.Load += (s, e) =>
            {
                this.Text = "WUNACH  •  Iniciar sesión";
                LayoutHelper.AplicarIcono(this);
                LayoutHelper.EnterPasaAlSiguiente(txtcorreo, txtcontraseña);
                AgregarLinkOlvidoContrasena();
                Tema.AplicarA(this);
            };

            this.MinimumSize = new System.Drawing.Size(800, 480);
        }

        // ── SECUENCIA SECRETA: button1 × 3 + Ctrl+Alt+Shift+X → D ───────────────
        private void Button1_SecretoClick(object sender, EventArgs e)
        {
            if (_modoAdmin) return;

            _clicksButton1++;
            if (_clicksButton1 >= 3)
            {
                _clicksButton1      = 0;
                _esperandoSecuencia = true;
            }
        }

        private void FrmAcceso_KeyDown(object sender, KeyEventArgs e)
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

            if (!modificadores)
                _xPresionado = false;
        }

        // ── DIÁLOGO FLOTANTE DE CÓDIGO ADMIN ─────────────────────────────────────
        private void MostrarDialogoCodigoAdmin()
        {
            Form dialogo = new Form
            {
                Text            = "Verificación de Administrador",
                Size            = new Size(340, 175),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition   = FormStartPosition.CenterParent,
                MaximizeBox     = false,
                MinimizeBox     = false,
                ShowInTaskbar   = false,
                TopMost         = true,
                BackColor       = Color.White
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
                Location  = new Point(20, 50),
                Size      = new Size(290, 28),
                Font      = new Font("Segoe UI", 12),
                PasswordChar = '●',
                MaxLength = 10,
                TextAlign = HorizontalAlignment.Center
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
                string codigo    = txtCodigo.Text.Trim();
                bool   correcto  = false;

                try   { correcto = VerificarCodigoAdmin(codigo); }
                catch (DllNotFoundException)
                {
                    MessageBox.Show(
                        "No se encontró VerificadorAdmin.dll.\n" +
                        "Asegúrate de que esté en la misma carpeta que el ejecutable.",
                        "DLL no encontrada", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            txtCodigo.KeyDown += (s, ev) =>
            {
                if (ev.KeyCode == Keys.Enter)
                { btnVerificar.PerformClick(); ev.SuppressKeyPress = true; }
            };

            dialogo.Controls.Add(lblTitulo);
            dialogo.Controls.Add(txtCodigo);
            dialogo.Controls.Add(lblError);
            dialogo.Controls.Add(btnVerificar);
            dialogo.Controls.Add(btnCancelar);
            dialogo.AcceptButton = btnVerificar;
            dialogo.ShowDialog(this);
        }

        // ── ACTIVAR MODO ADMINISTRADOR EN LOGIN ───────────────────────────────────
        private void ActivarModoAdmin()
        {
            _modoAdmin = true;

            // Ocultar selector de rol
            lblrol.Visible  = false;
            cmbrol.Visible  = false;

            // Asegurarse de que correo y contraseña estén visibles
            // (por si venía seleccionado VISITANTE antes)
            txtcorreo.Visible     = true;
            txtcontraseña.Visible  = true;
            lblcorreo.Visible     = true;
            lblcontra.Visible     = true;

            // Cambiar texto del botón
            btningreso.Text = "Ingresar como Admin";

            // Etiqueta visual
            Label lblModoAdmin = new Label
            {
                Text      = "🔐  Acceso de Administrador",
                Location  = new Point(lblrol.Left, lblrol.Top),
                AutoSize  = true,
                Font      = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 80, 160)
            };
            this.Controls.Add(lblModoAdmin);
            lblModoAdmin.BringToFront();
        }

        // ── CAMBIO DE ROL: mostrar/ocultar campos según selección ────────────────
        private void CmbRol_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool esVisitante = cmbrol.SelectedItem?.ToString() == "VISITANTE";

            // Ocultar/mostrar campos de credenciales
            txtcorreo.Visible    = !esVisitante;
            txtcontraseña.Visible = !esVisitante;
            lblcorreo.Visible    = !esVisitante;
            lblcontra.Visible    = !esVisitante;

            // Cambiar texto del botón de ingreso
            btningreso.Text = esVisitante ? "Entrar como Visitante" : "Ingresar";

            // Ocultar botón de crear cuenta para visitantes (no aplica)
            btnCcuenta.Visible = !esVisitante;
        }

        private void btningreso_Click(object sender, EventArgs e)
        {
            // ── VISITANTE: acceso directo sin credenciales ───────────────────────
            if (cmbrol.SelectedItem?.ToString() == "VISITANTE")
            {
                SesionUsuario.ID_Usuario  = null;
                SesionUsuario.Nombre      = "Visitante";
                SesionUsuario.Correo      = null;
                SesionUsuario.Rol         = "VISITANTE";
                SesionUsuario.Matricula   = null;
                SesionUsuario.ID_Facultad = null;

                // Visitantes siempre tema Claro (no tienen cuenta para guardar preferencia)
                Tema.ResetearAClaroSinUsuario();

                FrmPrincipal frm = new FrmPrincipal();
                this.Hide();
                frm.Show();
                return;
            }

            // ── 1. VALIDAR CAMPOS ────────────────────────────────────────────────
            if (string.IsNullOrWhiteSpace(txtcorreo.Text))
            {
                MostrarError("Ingresa tu correo electrónico.");
                txtcorreo.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtcontraseña.Text))
            {
                MostrarError("Ingresa tu contraseña.");
                txtcontraseña.Focus();
                return;
            }

            if (!_modoAdmin && cmbrol.SelectedIndex == -1)
            {
                MostrarError("Selecciona tu rol.");
                cmbrol.Focus();
                return;
            }

            // ── 2. PREPARAR CREDENCIALES ─────────────────────────────────────────
            string correo = txtcorreo.Text.Trim().ToLower();
            string hash   = HashearContrasena(txtcontraseña.Text);
            string rol    = _modoAdmin ? "ADMINISTRADOR" : cmbrol.SelectedItem.ToString();

            // ── 3. CONSULTAR LA BD ───────────────────────────────────────────────
            try
            {
                using (MySqlConnection conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();

                    string sql = @"
                        SELECT ID_Usuario, Nombre, Correo, Rol, Matricula,
                               ID_Facultad, ID_Licenciatura, Semestre,
                               Cuenta_Bloqueada
                        FROM tbl_WikiUnach_Usuarios
                        WHERE Correo           = @correo
                          AND Contrasena_Hash  = @hash
                          AND UPPER(Rol)       = UPPER(@rol)";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@correo", correo);
                        cmd.Parameters.AddWithValue("@hash",   hash);
                        cmd.Parameters.AddWithValue("@rol",    rol);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // ── Verificar si la cuenta está bloqueada ───────────
                                bool bloqueada = Convert.ToInt32(reader["Cuenta_Bloqueada"]) == 1;
                                if (bloqueada)
                                {
                                    MostrarError("Tu cuenta ha sido bloqueada por un administrador.\nContacta al soporte para más información.");
                                    txtcontraseña.Clear();
                                    return;
                                }

                                // ── 4. LOGIN EXITOSO: GUARDAR SESIÓN ────────────────
                                SesionUsuario.ID_Usuario      = reader["ID_Usuario"].ToString();
                                SesionUsuario.Nombre          = reader["Nombre"].ToString();
                                SesionUsuario.Correo          = reader["Correo"].ToString();
                                SesionUsuario.Rol             = reader["Rol"].ToString();
                                SesionUsuario.Matricula       = reader["Matricula"] == DBNull.Value
                                                                ? null
                                                                : reader["Matricula"].ToString();
                                SesionUsuario.ID_Facultad     = reader["ID_Facultad"].ToString();
                                SesionUsuario.ID_Licenciatura = reader["ID_Licenciatura"] == DBNull.Value
                                                                ? null
                                                                : reader["ID_Licenciatura"].ToString();
                                SesionUsuario.Semestre        = reader["Semestre"] == DBNull.Value
                                                                ? null
                                                                : reader["Semestre"].ToString();

                                // Cargar tema preferido del usuario (por cuenta)
                                Tema.CargarParaUsuario(SesionUsuario.ID_Usuario);

                                // ── 5. NAVEGAR AL FORMULARIO PRINCIPAL ──────────────
                                FrmPrincipal frm = new FrmPrincipal();
                                this.Hide();
                                frm.Show();
                            }
                            else
                            {
                                MostrarError("Correo, contraseña o rol incorrectos.\nVerifica tus datos e intenta de nuevo.");
                                txtcontraseña.Clear();
                                txtcontraseña.Focus();
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(
                    "Error al conectar con la base de datos:\n" + ex.Message,
                    "Error de conexión",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error inesperado:\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnCcuenta_Click(object sender, EventArgs e)
        {
            FrmCrearcuenta frm = new FrmCrearcuenta();
            this.Hide();
            frm.Show();
        }

        // ── MÉTODOS AUXILIARES ───────────────────────────────────────────────────

        /// <summary>
        /// Genera hash SHA-256 idéntico al usado en FrmCrearcuenta.
        /// </summary>
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

        // ─────────────────────────────────────────────────────────────────────────
        //                  RECUPERACIÓN DE CONTRASEÑA POR CORREO
        // ─────────────────────────────────────────────────────────────────────────
        private void AgregarLinkOlvidoContrasena()
        {
            LinkLabel lnk = new LinkLabel
            {
                Text      = "¿Olvidaste tu contraseña?",
                AutoSize  = true,
                Font      = new Font("Segoe UI", 9, FontStyle.Underline),
                LinkColor = Color.FromArgb(40, 90, 180),
                Location  = new Point(txtcontraseña.Left, txtcontraseña.Bottom + 6)
            };
            lnk.Click += (s, e) => IniciarFlujoRecuperacion();
            this.Controls.Add(lnk);
            lnk.BringToFront();
        }

        private void IniciarFlujoRecuperacion()
        {
            // Paso 1: pedir el correo
            string correo = PedirTextoEnDialogo(
                "Recuperar contraseña",
                "Ingresa el correo de tu cuenta:",
                false);
            if (string.IsNullOrWhiteSpace(correo)) return;
            correo = correo.Trim().ToLower();

            // Paso 2: verificar que exista en la BD
            string idUsuario;
            try
            {
                using (var conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "SELECT ID_Usuario FROM tbl_WikiUnach_Usuarios WHERE Correo=@c", conn))
                    {
                        cmd.Parameters.AddWithValue("@c", correo);
                        object r = cmd.ExecuteScalar();
                        if (r == null)
                        {
                            MostrarError("No existe ninguna cuenta con ese correo.");
                            return;
                        }
                        idUsuario = r.ToString();
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al consultar la BD:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Paso 3: enviar código
            string codigoEsperado = EmailService.GenerarCodigo();
            try
            {
                Cursor = Cursors.WaitCursor;
                EmailService.EnviarCodigoVerificacion(correo, codigoEsperado);
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show("No se pudo enviar el correo:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Cursor = Cursors.Default;

            // Paso 4: pedir el código
            string codigoIngresado = PedirTextoEnDialogo(
                "Verificación",
                "Te enviamos un código a " + correo + ".\nIngrésalo aquí:",
                false);
            if (string.IsNullOrEmpty(codigoIngresado)) return;
            if (codigoIngresado.Trim() != codigoEsperado)
            {
                MostrarError("Código incorrecto.");
                return;
            }

            // Paso 5: pedir nueva contraseña
            string nueva = PedirTextoEnDialogo(
                "Nueva contraseña",
                "Ingresa tu nueva contraseña (mínimo 6 caracteres):",
                true);
            if (string.IsNullOrEmpty(nueva)) return;
            if (nueva.Length < 6)
            {
                MostrarError("La contraseña debe tener al menos 6 caracteres.");
                return;
            }
            string confirmacion = PedirTextoEnDialogo(
                "Confirmar contraseña", "Repite la nueva contraseña:", true);
            if (nueva != confirmacion)
            {
                MostrarError("Las contraseñas no coinciden.");
                return;
            }

            // Paso 6: actualizar BD
            try
            {
                string nuevoHash = HashearContrasena(nueva);
                using (var conn = DBConexion.ObtenerConexion())
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(
                        "UPDATE tbl_WikiUnach_Usuarios SET Contrasena_Hash=@h " +
                        "WHERE ID_Usuario=@id", conn))
                    {
                        cmd.Parameters.AddWithValue("@h",  nuevoHash);
                        cmd.Parameters.AddWithValue("@id", idUsuario);
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show("Contraseña actualizada correctamente.\nYa puedes iniciar sesión.",
                    "Listo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error al actualizar contraseña:\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>Diálogo simple para pedir un string al usuario.</summary>
        private string PedirTextoEnDialogo(string titulo, string mensaje, bool esContrasena)
        {
            Form dlg = new Form
            {
                Text            = titulo,
                Size            = new Size(420, 200),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition   = FormStartPosition.CenterParent,
                MaximizeBox     = false,
                MinimizeBox     = false,
                ShowInTaskbar   = false,
                BackColor       = Color.White
            };
            Label lbl = new Label
            {
                Text     = mensaje,
                Location = new Point(20, 18),
                Size     = new Size(370, 50),
                Font     = new Font("Segoe UI", 9.5f)
            };
            TextBox txt = new TextBox
            {
                Location     = new Point(20, 75),
                Size         = new Size(370, 28),
                Font         = new Font("Segoe UI", 11),
                PasswordChar = esContrasena ? '●' : '\0'
            };
            Button btnOk = new Button
            {
                Text         = "Aceptar",
                DialogResult = DialogResult.OK,
                Location     = new Point(200, 115),
                Size         = new Size(90, 32),
                BackColor    = Color.FromArgb(30, 100, 210),
                ForeColor    = Color.White,
                FlatStyle    = FlatStyle.Flat,
                Font         = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnOk.FlatAppearance.BorderSize = 0;
            Button btnCancel = new Button
            {
                Text         = "Cancelar",
                DialogResult = DialogResult.Cancel,
                Location     = new Point(300, 115),
                Size         = new Size(90, 32),
                FlatStyle    = FlatStyle.Flat
            };
            dlg.Controls.AddRange(new Control[] { lbl, txt, btnOk, btnCancel });
            dlg.AcceptButton = btnOk;
            dlg.CancelButton = btnCancel;
            txt.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) { dlg.DialogResult = DialogResult.OK; e.SuppressKeyPress = true; }
            };
            return dlg.ShowDialog(this) == DialogResult.OK ? txt.Text : null;
        }
    }
}
