namespace WUNACH
{
    partial class FrmDetallesTarea
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmDetallesTarea));
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnVerArchivos = new System.Windows.Forms.Button();
            this.btnVerComentarios = new System.Windows.Forms.Button();
            this.pnlRating = new System.Windows.Forms.Panel();
            this.lblContadorVotos = new System.Windows.Forms.Label();
            this.btnDislike = new System.Windows.Forms.Button();
            this.btnLike = new System.Windows.Forms.Button();
            this.pnlCuerpoDetalle = new System.Windows.Forms.Panel();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.pnlHeaderDetalle = new System.Windows.Forms.Panel();
            this.lblfechaActualizacion = new System.Windows.Forms.Label();
            this.lblTipoActividad = new System.Windows.Forms.Label();
            this.lbltags = new System.Windows.Forms.Label();
            this.btnRevisiones = new System.Windows.Forms.Button();
            this.btnRegresar = new System.Windows.Forms.Button();
            this.lblMateriaTag = new System.Windows.Forms.Label();
            this.lblfechaCreacion = new System.Windows.Forms.Label();
            this.lblInfoUsuario = new System.Windows.Forms.Label();
            this.lblTituloTarea = new System.Windows.Forms.Label();
            this.btnBookmark = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.pnlRating.SuspendLayout();
            this.pnlCuerpoDetalle.SuspendLayout();
            this.pnlHeaderDetalle.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnBookmark);
            this.panel1.Controls.Add(this.btnVerArchivos);
            this.panel1.Controls.Add(this.btnVerComentarios);
            this.panel1.Controls.Add(this.pnlRating);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 138);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1277, 97);
            this.panel1.TabIndex = 5;
            // 
            // btnVerArchivos
            // 
            this.btnVerArchivos.BackColor = System.Drawing.Color.LightSlateGray;
            this.btnVerArchivos.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVerArchivos.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnVerArchivos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVerArchivos.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVerArchivos.Location = new System.Drawing.Point(10, 14);
            this.btnVerArchivos.Name = "btnVerArchivos";
            this.btnVerArchivos.Size = new System.Drawing.Size(150, 36);
            this.btnVerArchivos.TabIndex = 3;
            this.btnVerArchivos.Text = "📎 Archivos";
            this.btnVerArchivos.UseVisualStyleBackColor = false;
            this.btnVerArchivos.Click += new System.EventHandler(this.btnVerArchivos_Click);
            // 
            // btnVerComentarios
            // 
            this.btnVerComentarios.BackColor = System.Drawing.Color.LightSlateGray;
            this.btnVerComentarios.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVerComentarios.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnVerComentarios.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVerComentarios.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVerComentarios.Location = new System.Drawing.Point(1086, 14);
            this.btnVerComentarios.Name = "btnVerComentarios";
            this.btnVerComentarios.Size = new System.Drawing.Size(165, 36);
            this.btnVerComentarios.TabIndex = 2;
            this.btnVerComentarios.Text = "Comentarios 💬";
            this.btnVerComentarios.UseVisualStyleBackColor = false;
            this.btnVerComentarios.Click += new System.EventHandler(this.btnVerComentarios_Click);
            // 
            // pnlRating
            // 
            this.pnlRating.BackColor = System.Drawing.Color.Silver;
            this.pnlRating.Controls.Add(this.lblContadorVotos);
            this.pnlRating.Controls.Add(this.btnDislike);
            this.pnlRating.Controls.Add(this.btnLike);
            this.pnlRating.Location = new System.Drawing.Point(770, 7);
            this.pnlRating.Name = "pnlRating";
            this.pnlRating.Size = new System.Drawing.Size(306, 50);
            this.pnlRating.TabIndex = 1;
            // 
            // lblContadorVotos
            // 
            this.lblContadorVotos.AutoSize = true;
            this.lblContadorVotos.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContadorVotos.Location = new System.Drawing.Point(130, 26);
            this.lblContadorVotos.Name = "lblContadorVotos";
            this.lblContadorVotos.Size = new System.Drawing.Size(73, 31);
            this.lblContadorVotos.TabIndex = 2;
            this.lblContadorVotos.Text = "label1";
            // 
            // btnDislike
            // 
            this.btnDislike.BackColor = System.Drawing.Color.White;
            this.btnDislike.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnDislike.BackgroundImage")));
            this.btnDislike.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnDislike.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDislike.Location = new System.Drawing.Point(220, 3);
            this.btnDislike.Name = "btnDislike";
            this.btnDislike.Size = new System.Drawing.Size(83, 44);
            this.btnDislike.TabIndex = 1;
            this.btnDislike.UseVisualStyleBackColor = false;
            // 
            // btnLike
            // 
            this.btnLike.BackColor = System.Drawing.Color.White;
            this.btnLike.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnLike.BackgroundImage")));
            this.btnLike.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnLike.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLike.Location = new System.Drawing.Point(3, 1);
            this.btnLike.Name = "btnLike";
            this.btnLike.Size = new System.Drawing.Size(87, 47);
            this.btnLike.TabIndex = 0;
            this.btnLike.UseVisualStyleBackColor = false;
            // 
            // pnlCuerpoDetalle
            // 
            this.pnlCuerpoDetalle.Controls.Add(this.richTextBox1);
            this.pnlCuerpoDetalle.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlCuerpoDetalle.Location = new System.Drawing.Point(0, 171);
            this.pnlCuerpoDetalle.Name = "pnlCuerpoDetalle";
            this.pnlCuerpoDetalle.Size = new System.Drawing.Size(1277, 454);
            this.pnlCuerpoDetalle.TabIndex = 4;
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.Color.White;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(0, 0);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(1277, 454);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // pnlHeaderDetalle
            // 
            this.pnlHeaderDetalle.Controls.Add(this.lblfechaActualizacion);
            this.pnlHeaderDetalle.Controls.Add(this.lblTipoActividad);
            this.pnlHeaderDetalle.Controls.Add(this.lbltags);
            this.pnlHeaderDetalle.Controls.Add(this.btnRevisiones);
            this.pnlHeaderDetalle.Controls.Add(this.btnRegresar);
            this.pnlHeaderDetalle.Controls.Add(this.lblMateriaTag);
            this.pnlHeaderDetalle.Controls.Add(this.lblfechaCreacion);
            this.pnlHeaderDetalle.Controls.Add(this.lblInfoUsuario);
            this.pnlHeaderDetalle.Controls.Add(this.lblTituloTarea);
            this.pnlHeaderDetalle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeaderDetalle.Location = new System.Drawing.Point(0, 0);
            this.pnlHeaderDetalle.Name = "pnlHeaderDetalle";
            this.pnlHeaderDetalle.Size = new System.Drawing.Size(1277, 138);
            this.pnlHeaderDetalle.TabIndex = 3;
            // 
            // lblfechaActualizacion
            // 
            this.lblfechaActualizacion.AutoSize = true;
            this.lblfechaActualizacion.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblfechaActualizacion.Location = new System.Drawing.Point(549, 56);
            this.lblfechaActualizacion.Name = "lblfechaActualizacion";
            this.lblfechaActualizacion.Size = new System.Drawing.Size(77, 32);
            this.lblfechaActualizacion.TabIndex = 7;
            this.lblfechaActualizacion.Text = "Fecha";
            // 
            // lblTipoActividad
            // 
            this.lblTipoActividad.AutoSize = true;
            this.lblTipoActividad.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTipoActividad.Location = new System.Drawing.Point(744, 56);
            this.lblTipoActividad.Name = "lblTipoActividad";
            this.lblTipoActividad.Size = new System.Drawing.Size(203, 32);
            this.lblTipoActividad.TabIndex = 6;
            this.lblTipoActividad.Text = "Tipo de Actividad";
            // 
            // lbltags
            // 
            this.lbltags.AutoSize = true;
            this.lbltags.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbltags.Location = new System.Drawing.Point(1034, 56);
            this.lbltags.Name = "lbltags";
            this.lbltags.Size = new System.Drawing.Size(62, 32);
            this.lbltags.TabIndex = 5;
            this.lbltags.Text = "Tags";
            // 
            // btnRevisiones
            // 
            this.btnRevisiones.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btnRevisiones.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRevisiones.Location = new System.Drawing.Point(94, 98);
            this.btnRevisiones.Name = "btnRevisiones";
            this.btnRevisiones.Size = new System.Drawing.Size(127, 40);
            this.btnRevisiones.TabIndex = 4;
            this.btnRevisiones.Text = "Revisiones 🔍";
            this.btnRevisiones.UseVisualStyleBackColor = false;
            this.btnRevisiones.Click += new System.EventHandler(this.btnRevisiones_Click);
            // 
            // btnRegresar
            // 
            this.btnRegresar.BackColor = System.Drawing.Color.White;
            this.btnRegresar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnRegresar.BackgroundImage")));
            this.btnRegresar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnRegresar.Location = new System.Drawing.Point(3, 98);
            this.btnRegresar.Name = "btnRegresar";
            this.btnRegresar.Size = new System.Drawing.Size(85, 40);
            this.btnRegresar.TabIndex = 3;
            this.btnRegresar.UseVisualStyleBackColor = false;
            this.btnRegresar.Click += new System.EventHandler(this.btnRegresar_Click);
            // 
            // lblMateriaTag
            // 
            this.lblMateriaTag.AutoSize = true;
            this.lblMateriaTag.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMateriaTag.Location = new System.Drawing.Point(171, 56);
            this.lblMateriaTag.Name = "lblMateriaTag";
            this.lblMateriaTag.Size = new System.Drawing.Size(99, 32);
            this.lblMateriaTag.TabIndex = 1;
            this.lblMateriaTag.Text = "Materia";
            // 
            // lblfechaCreacion
            // 
            this.lblfechaCreacion.AutoSize = true;
            this.lblfechaCreacion.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblfechaCreacion.Location = new System.Drawing.Point(373, 56);
            this.lblfechaCreacion.Name = "lblfechaCreacion";
            this.lblfechaCreacion.Size = new System.Drawing.Size(77, 32);
            this.lblfechaCreacion.TabIndex = 1;
            this.lblfechaCreacion.Text = "Fecha";
            // 
            // lblInfoUsuario
            // 
            this.lblInfoUsuario.AutoSize = true;
            this.lblInfoUsuario.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInfoUsuario.Location = new System.Drawing.Point(3, 53);
            this.lblInfoUsuario.Name = "lblInfoUsuario";
            this.lblInfoUsuario.Size = new System.Drawing.Size(103, 32);
            this.lblInfoUsuario.TabIndex = 1;
            this.lblInfoUsuario.Text = "Nombre";
            // 
            // lblTituloTarea
            // 
            this.lblTituloTarea.AutoSize = true;
            this.lblTituloTarea.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTituloTarea.Location = new System.Drawing.Point(12, 8);
            this.lblTituloTarea.Name = "lblTituloTarea";
            this.lblTituloTarea.Size = new System.Drawing.Size(138, 55);
            this.lblTituloTarea.TabIndex = 1;
            this.lblTituloTarea.Text = "Título";
            // 
            // btnBookmark
            // 
            this.btnBookmark.Location = new System.Drawing.Point(661, 14);
            this.btnBookmark.Name = "btnBookmark";
            this.btnBookmark.Size = new System.Drawing.Size(83, 61);
            this.btnBookmark.TabIndex = 1;
            this.btnBookmark.Text = "Bookmark";
            this.btnBookmark.UseVisualStyleBackColor = true;
            // 
            // FrmDetallesTarea
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1277, 625);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pnlCuerpoDetalle);
            this.Controls.Add(this.pnlHeaderDetalle);
            this.Font = new System.Drawing.Font("Modern No. 20", 7.999999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FrmDetallesTarea";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmDetallesTarea";
            this.panel1.ResumeLayout(false);
            this.pnlRating.ResumeLayout(false);
            this.pnlRating.PerformLayout();
            this.pnlCuerpoDetalle.ResumeLayout(false);
            this.pnlHeaderDetalle.ResumeLayout(false);
            this.pnlHeaderDetalle.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnVerArchivos;
        private System.Windows.Forms.Button btnVerComentarios;
        private System.Windows.Forms.Panel pnlRating;
        private System.Windows.Forms.Label lblContadorVotos;
        private System.Windows.Forms.Button btnDislike;
        private System.Windows.Forms.Button btnLike;
        private System.Windows.Forms.Panel pnlCuerpoDetalle;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Panel pnlHeaderDetalle;
        private System.Windows.Forms.Label lblfechaActualizacion;
        private System.Windows.Forms.Label lblTipoActividad;
        private System.Windows.Forms.Label lbltags;
        private System.Windows.Forms.Button btnRevisiones;
        private System.Windows.Forms.Button btnRegresar;
        private System.Windows.Forms.Label lblMateriaTag;
        private System.Windows.Forms.Label lblfechaCreacion;
        private System.Windows.Forms.Label lblInfoUsuario;
        private System.Windows.Forms.Label lblTituloTarea;
        private System.Windows.Forms.Button btnBookmark;
    }
}