namespace WUNACH
{
    partial class FrmPrincipal
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
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.cmbSemestre = new System.Windows.Forms.ComboBox();
            this.cmbLicenciatura = new System.Windows.Forms.ComboBox();
            this.cmbFacultad = new System.Windows.Forms.ComboBox();
            this.btncuenta = new System.Windows.Forms.Button();
            this.btnCerrarsecion = new System.Windows.Forms.Button();
            this.lblsubir = new System.Windows.Forms.Button();
            this.pnlContenedor = new System.Windows.Forms.Panel();
            this.pnlTabs = new System.Windows.Forms.FlowLayoutPanel();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.pnlHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.Controls.Add(this.txtSearch);
            this.pnlHeader.Controls.Add(this.cmbSemestre);
            this.pnlHeader.Controls.Add(this.cmbLicenciatura);
            this.pnlHeader.Controls.Add(this.cmbFacultad);
            this.pnlHeader.Controls.Add(this.btncuenta);
            this.pnlHeader.Controls.Add(this.btnCerrarsecion);
            this.pnlHeader.Controls.Add(this.lblsubir);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 49);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1732, 125);
            this.pnlHeader.TabIndex = 8;
            // 
            // cmbSemestre
            // 
            this.cmbSemestre.FormattingEnabled = true;
            this.cmbSemestre.Location = new System.Drawing.Point(765, 3);
            this.cmbSemestre.Name = "cmbSemestre";
            this.cmbSemestre.Size = new System.Drawing.Size(314, 28);
            this.cmbSemestre.TabIndex = 6;
            // 
            // cmbLicenciatura
            // 
            this.cmbLicenciatura.FormattingEnabled = true;
            this.cmbLicenciatura.Location = new System.Drawing.Point(442, 42);
            this.cmbLicenciatura.Name = "cmbLicenciatura";
            this.cmbLicenciatura.Size = new System.Drawing.Size(314, 28);
            this.cmbLicenciatura.TabIndex = 5;
            // 
            // cmbFacultad
            // 
            this.cmbFacultad.FormattingEnabled = true;
            this.cmbFacultad.Location = new System.Drawing.Point(441, 3);
            this.cmbFacultad.Name = "cmbFacultad";
            this.cmbFacultad.Size = new System.Drawing.Size(316, 28);
            this.cmbFacultad.TabIndex = 4;
            // 
            // btncuenta
            // 
            this.btncuenta.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btncuenta.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btncuenta.Location = new System.Drawing.Point(0, 0);
            this.btncuenta.Name = "btncuenta";
            this.btncuenta.Size = new System.Drawing.Size(114, 54);
            this.btncuenta.TabIndex = 3;
            this.btncuenta.Text = "Cuenta";
            this.btncuenta.UseVisualStyleBackColor = false;
            this.btncuenta.Click += new System.EventHandler(this.btncuenta_Click);
            // 
            // btnCerrarsecion
            // 
            this.btnCerrarsecion.BackColor = System.Drawing.Color.LightSteelBlue;
            this.btnCerrarsecion.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCerrarsecion.Location = new System.Drawing.Point(120, 0);
            this.btnCerrarsecion.Name = "btnCerrarsecion";
            this.btnCerrarsecion.Size = new System.Drawing.Size(122, 54);
            this.btnCerrarsecion.TabIndex = 1;
            this.btnCerrarsecion.Text = "Cerrar sesion";
            this.btnCerrarsecion.UseVisualStyleBackColor = false;
            this.btnCerrarsecion.Click += new System.EventHandler(this.btnCerrarsecion_Click);
            // 
            // lblsubir
            // 
            this.lblsubir.BackColor = System.Drawing.Color.LightSteelBlue;
            this.lblsubir.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblsubir.Location = new System.Drawing.Point(248, 0);
            this.lblsubir.Name = "lblsubir";
            this.lblsubir.Size = new System.Drawing.Size(159, 54);
            this.lblsubir.TabIndex = 2;
            this.lblsubir.Text = "Subir 📤";
            this.lblsubir.UseVisualStyleBackColor = false;
            this.lblsubir.Click += new System.EventHandler(this.lblsubir_Click);
            // 
            // pnlContenedor
            // 
            this.pnlContenedor.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlContenedor.Location = new System.Drawing.Point(0, 134);
            this.pnlContenedor.Name = "pnlContenedor";
            this.pnlContenedor.Size = new System.Drawing.Size(1732, 537);
            this.pnlContenedor.TabIndex = 7;
            // 
            // pnlTabs
            // 
            this.pnlTabs.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTabs.Location = new System.Drawing.Point(0, 0);
            this.pnlTabs.Name = "pnlTabs";
            this.pnlTabs.Size = new System.Drawing.Size(1732, 49);
            this.pnlTabs.TabIndex = 6;
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(1123, 7);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(280, 26);
            this.txtSearch.TabIndex = 7;
            // 
            // FrmPrincipal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1732, 671);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlContenedor);
            this.Controls.Add(this.pnlTabs);
            this.Name = "FrmPrincipal";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmPrincipal";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.ComboBox cmbSemestre;
        private System.Windows.Forms.ComboBox cmbLicenciatura;
        private System.Windows.Forms.ComboBox cmbFacultad;
        private System.Windows.Forms.Button btncuenta;
        private System.Windows.Forms.Button btnCerrarsecion;
        private System.Windows.Forms.Button lblsubir;
        private System.Windows.Forms.Panel pnlContenedor;
        private System.Windows.Forms.FlowLayoutPanel pnlTabs;
        private System.Windows.Forms.TextBox txtSearch;
    }
}