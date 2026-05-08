namespace WUNACH
{
    partial class FrmAdministrarTareas
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmAdministrarTareas));
            this.pnlContenedor = new System.Windows.Forms.Panel();
            this.pnlTabs = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.cmbSemestre = new System.Windows.Forms.ComboBox();
            this.cmbLicenciatura = new System.Windows.Forms.ComboBox();
            this.cmbFacultad = new System.Windows.Forms.ComboBox();
            this.btnRegresar = new System.Windows.Forms.Button();
            this.cmbGuardados = new System.Windows.Forms.ComboBox();
            this.pnlHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlContenedor
            // 
            this.pnlContenedor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContenedor.Location = new System.Drawing.Point(0, 200);
            this.pnlContenedor.Name = "pnlContenedor";
            this.pnlContenedor.Size = new System.Drawing.Size(1374, 467);
            this.pnlContenedor.TabIndex = 5;
            // 
            // pnlTabs
            // 
            this.pnlTabs.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTabs.Location = new System.Drawing.Point(0, 100);
            this.pnlTabs.Name = "pnlTabs";
            this.pnlTabs.Size = new System.Drawing.Size(1374, 100);
            this.pnlTabs.TabIndex = 4;
            // 
            // pnlHeader
            // 
            this.pnlHeader.Controls.Add(this.cmbGuardados);
            this.pnlHeader.Controls.Add(this.cmbSemestre);
            this.pnlHeader.Controls.Add(this.cmbLicenciatura);
            this.pnlHeader.Controls.Add(this.cmbFacultad);
            this.pnlHeader.Controls.Add(this.btnRegresar);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1374, 100);
            this.pnlHeader.TabIndex = 3;
            // 
            // cmbSemestre
            // 
            this.cmbSemestre.FormattingEnabled = true;
            this.cmbSemestre.Location = new System.Drawing.Point(333, 15);
            this.cmbSemestre.Name = "cmbSemestre";
            this.cmbSemestre.Size = new System.Drawing.Size(314, 28);
            this.cmbSemestre.TabIndex = 9;
            // 
            // cmbLicenciatura
            // 
            this.cmbLicenciatura.FormattingEnabled = true;
            this.cmbLicenciatura.Location = new System.Drawing.Point(12, 49);
            this.cmbLicenciatura.Name = "cmbLicenciatura";
            this.cmbLicenciatura.Size = new System.Drawing.Size(314, 28);
            this.cmbLicenciatura.TabIndex = 8;
            // 
            // cmbFacultad
            // 
            this.cmbFacultad.FormattingEnabled = true;
            this.cmbFacultad.Location = new System.Drawing.Point(10, 15);
            this.cmbFacultad.Name = "cmbFacultad";
            this.cmbFacultad.Size = new System.Drawing.Size(316, 28);
            this.cmbFacultad.TabIndex = 7;
            // 
            // btnRegresar
            // 
            this.btnRegresar.BackColor = System.Drawing.Color.White;
            this.btnRegresar.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnRegresar.BackgroundImage")));
            this.btnRegresar.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnRegresar.Location = new System.Drawing.Point(1176, 25);
            this.btnRegresar.Name = "btnRegresar";
            this.btnRegresar.Size = new System.Drawing.Size(126, 57);
            this.btnRegresar.TabIndex = 2;
            this.btnRegresar.UseVisualStyleBackColor = false;
            // 
            // cmbGuardados
            // 
            this.cmbGuardados.FormattingEnabled = true;
            this.cmbGuardados.Items.AddRange(new object[] {
            "Mis Tareas",
            "BookMarked",
            "Liked"});
            this.cmbGuardados.Location = new System.Drawing.Point(711, 15);
            this.cmbGuardados.Name = "cmbGuardados";
            this.cmbGuardados.Size = new System.Drawing.Size(274, 28);
            this.cmbGuardados.TabIndex = 10;
            // 
            // FrmAdministrarTareas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1374, 667);
            this.Controls.Add(this.pnlContenedor);
            this.Controls.Add(this.pnlTabs);
            this.Controls.Add(this.pnlHeader);
            this.Name = "FrmAdministrarTareas";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmAdministrarTareas";
            this.pnlHeader.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlContenedor;
        private System.Windows.Forms.FlowLayoutPanel pnlTabs;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.ComboBox cmbSemestre;
        private System.Windows.Forms.ComboBox cmbLicenciatura;
        private System.Windows.Forms.ComboBox cmbFacultad;
        private System.Windows.Forms.Button btnRegresar;
        private System.Windows.Forms.ComboBox cmbGuardados;
    }
}