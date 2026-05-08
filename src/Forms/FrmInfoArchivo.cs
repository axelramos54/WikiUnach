using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace WUNACH
{
    /// <summary>
    /// Ventana flotante que muestra el detalle de un archivo adjunto.
    /// Se crea completamente en código, no requiere Designer.
    /// Se cierra con el botón ✕, haciendo clic fuera o presionando Escape.
    /// </summary>
    internal class FrmInfoArchivo : Form
    {
        // URL pre-firmada recibida desde FrmDetallesTarea
        private readonly string _urlFirmada;

        public FrmInfoArchivo(string titulo, string descripcion, long tamanoBytes,
                              DateTime fecha, Image icono, string urlFirmada)
        {
            _urlFirmada = urlFirmada;

            // ── CONFIGURACIÓN DEL FORM ───────────────────────────────────────────
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition   = FormStartPosition.CenterParent;
            this.Size            = new Size(460, 290);
            this.BackColor       = Color.White;
            this.KeyPreview      = true;

            // Borde simulado con un panel exterior
            Panel pnlBorde = new Panel
            {
                Dock        = DockStyle.Fill,
                BackColor   = Color.FromArgb(210, 210, 210),
                Padding     = new Padding(1)
            };

            Panel pnlFondo = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.White
            };

            // ── BOTÓN CERRAR ─────────────────────────────────────────────────────
            Button btnCerrar = new Button
            {
                Text      = "✕",
                Location  = new Point(416, 8),
                Size      = new Size(34, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.Gray,
                Font      = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnCerrar.FlatAppearance.BorderSize         = 0;
            btnCerrar.FlatAppearance.MouseOverBackColor = Color.FromArgb(255, 210, 210);
            btnCerrar.Click += (s, e) => this.Close();

            // ── ÍCONO DEL TIPO DE ARCHIVO ────────────────────────────────────────
            PictureBox pbIcono = new PictureBox
            {
                Location  = new Point(20, 20),
                Size      = new Size(64, 64),
                SizeMode  = PictureBoxSizeMode.Zoom,
                Image     = icono,
                BackColor = Color.FromArgb(245, 245, 245)
            };

            // ── TÍTULO ───────────────────────────────────────────────────────────
            Label lblTitulo = new Label
            {
                Text      = titulo,
                Location  = new Point(100, 18),
                Size      = new Size(340, 34),
                Font      = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(25, 25, 25)
            };

            // ── DESCRIPCIÓN (máximo 25 palabras) ─────────────────────────────────
            string[] palabras  = descripcion.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string   descCorta = palabras.Length > 25
                                 ? string.Join(" ", palabras.Take(25)) + "..."
                                 : descripcion;

            Label lblDescripcion = new Label
            {
                Text      = descCorta,
                Location  = new Point(100, 56),
                Size      = new Size(340, 60),
                Font      = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(120, 120, 120)
            };

            // ── SEPARADOR ────────────────────────────────────────────────────────
            Panel separador = new Panel
            {
                Location  = new Point(20, 148),
                Size      = new Size(418, 1),
                BackColor = Color.FromArgb(220, 220, 220)
            };

            // ── TAMAÑO DEL ARCHIVO ───────────────────────────────────────────────
            Label lblTamaño = new Label
            {
                Text      = "📦  " + FormatearTamaño(tamanoBytes),
                Location  = new Point(20, 160),
                AutoSize  = true,
                Font      = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(140, 140, 140)
            };

            // ── FECHA DE PUBLICACIÓN ─────────────────────────────────────────────
            Label lblFecha = new Label
            {
                Text      = "📅  Publicado: " + fecha.ToString("dd/MM/yyyy"),
                Location  = new Point(20, 185),
                AutoSize  = true,
                Font      = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(140, 140, 140)
            };

            // ── BOTONES ABRIR / DESCARGAR ────────────────────────────────────────
            Button btnAbrir = new Button
            {
                Text      = "🌐  Abrir",
                Location  = new Point(20, 218),
                Size      = new Size(120, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(30, 100, 210),
                ForeColor = Color.White,
                Font      = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnAbrir.FlatAppearance.BorderSize = 0;
            btnAbrir.FlatAppearance.MouseOverBackColor = Color.FromArgb(20, 80, 180);
            btnAbrir.Click += (s, e) =>
            {
                try { Process.Start(_urlFirmada); }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudo abrir el archivo:\n" + ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            Button btnDescargar = new Button
            {
                Text      = "📥  Descargar",
                Location  = new Point(154, 218),
                Size      = new Size(140, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(230, 244, 255),
                ForeColor = Color.FromArgb(30, 100, 210),
                Font      = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnDescargar.FlatAppearance.BorderColor = Color.FromArgb(180, 210, 245);
            btnDescargar.FlatAppearance.BorderSize  = 1;
            btnDescargar.Click += (s, e) =>
            {
                try
                {
                    string carpeta   = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                    string descargas = Path.Combine(carpeta, "Downloads");
                    if (!Directory.Exists(descargas)) descargas = carpeta;

                    string ext          = Path.GetExtension(new Uri(_urlFirmada).AbsolutePath);
                    string nombreLimpio = string.Concat(titulo.Split(Path.GetInvalidFileNameChars()));
                    string destino      = Path.Combine(descargas, nombreLimpio + ext);

                    int n = 1;
                    while (File.Exists(destino))
                        destino = Path.Combine(descargas, $"{nombreLimpio}_{n++}{ext}");

                    using (WebClient wc = new WebClient())
                        wc.DownloadFile(_urlFirmada, destino);

                    MessageBox.Show("Archivo guardado en:\n" + destino,
                        "Descarga completada", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al descargar:\n" + ex.Message,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            // ── AGREGAR CONTROLES ────────────────────────────────────────────────
            this.Controls.Add(btnCerrar);
            this.Controls.Add(pbIcono);
            this.Controls.Add(lblTitulo);
            this.Controls.Add(lblDescripcion);
            this.Controls.Add(separador);
            this.Controls.Add(lblTamaño);
            this.Controls.Add(lblFecha);
            this.Controls.Add(btnAbrir);
            this.Controls.Add(btnDescargar);
            this.Controls.Add(pnlBorde);

            // ── ARRASTRABLE (MouseDown/Move/Up sobre el form) ────────────────────
            bool   arrastrando = false;
            Point  origenArrastre = Point.Empty;

            this.MouseDown += (s, e) => { arrastrando = true;  origenArrastre = e.Location; };
            this.MouseMove += (s, e) =>
            {
                if (arrastrando)
                    this.Location = new Point(
                        this.Location.X + e.X - origenArrastre.X,
                        this.Location.Y + e.Y - origenArrastre.Y);
            };
            this.MouseUp += (s, e) => arrastrando = false;

            // ── CERRAR CON ESCAPE ────────────────────────────────────────────────
            this.KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) this.Close(); };
        }

        // ── FORMATEO DE TAMAÑO ───────────────────────────────────────────────────
        private string FormatearTamaño(long bytes)
        {
            if (bytes >= 1024L * 1024 * 1024) return $"{bytes / (1024.0 * 1024 * 1024):F1} GB";
            if (bytes >= 1024 * 1024)         return $"{bytes / (1024.0 * 1024):F1} MB";
            if (bytes >= 1024)                return $"{bytes / 1024.0:F1} KB";
            return $"{bytes} B";
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // FrmInfoArchivo
            // 
            this.ClientSize = new System.Drawing.Size(460, 290);
            this.Name = "FrmInfoArchivo";
            this.ResumeLayout(false);

        }
    }
}
