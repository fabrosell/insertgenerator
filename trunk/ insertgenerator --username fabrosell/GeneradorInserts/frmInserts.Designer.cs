namespace Suru.InsertGenerator.GeneradorUI
{
    partial class frmInserts
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmInserts));
            this.cmbDataBase = new System.Windows.Forms.ComboBox();
            this.lblDatabase = new System.Windows.Forms.Label();
            this.dgvTables = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.btnOptions = new System.Windows.Forms.Button();
            this.ssStateBar = new System.Windows.Forms.StatusStrip();
            this.tssServerName = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssUserName = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnChangeConnection = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTables)).BeginInit();
            this.ssStateBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbDataBase
            // 
            this.cmbDataBase.FormattingEnabled = true;
            this.cmbDataBase.Location = new System.Drawing.Point(75, 12);
            this.cmbDataBase.Name = "cmbDataBase";
            this.cmbDataBase.Size = new System.Drawing.Size(219, 21);
            this.cmbDataBase.TabIndex = 0;
            // 
            // lblDatabase
            // 
            this.lblDatabase.AutoSize = true;
            this.lblDatabase.Location = new System.Drawing.Point(10, 15);
            this.lblDatabase.Name = "lblDatabase";
            this.lblDatabase.Size = new System.Drawing.Size(54, 13);
            this.lblDatabase.TabIndex = 1;
            this.lblDatabase.Text = "DataBase";
            // 
            // dgvTables
            // 
            this.dgvTables.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvTables.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTables.Location = new System.Drawing.Point(0, 87);
            this.dgvTables.Name = "dgvTables";
            this.dgvTables.Size = new System.Drawing.Size(346, 313);
            this.dgvTables.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(90, 45);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(105, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Generate Inserts";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(12, 49);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(70, 17);
            this.chkSelectAll.TabIndex = 4;
            this.chkSelectAll.Text = "Select All";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            // 
            // btnOptions
            // 
            this.btnOptions.Location = new System.Drawing.Point(201, 45);
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(108, 23);
            this.btnOptions.TabIndex = 5;
            this.btnOptions.Text = "Options >>";
            this.btnOptions.UseVisualStyleBackColor = true;
            // 
            // ssStateBar
            // 
            this.ssStateBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssServerName,
            this.tssUserName});
            this.ssStateBar.Location = new System.Drawing.Point(0, 403);
            this.ssStateBar.Name = "ssStateBar";
            this.ssStateBar.Size = new System.Drawing.Size(346, 22);
            this.ssStateBar.TabIndex = 6;
            // 
            // tssServerName
            // 
            this.tssServerName.Name = "tssServerName";
            this.tssServerName.Size = new System.Drawing.Size(77, 17);
            this.tssServerName.Text = "[Server Name]";
            // 
            // tssUserName
            // 
            this.tssUserName.Name = "tssUserName";
            this.tssUserName.Size = new System.Drawing.Size(67, 17);
            this.tssUserName.Text = "[User Name]";
            // 
            // btnChangeConnection
            // 
            this.btnChangeConnection.Image = ((System.Drawing.Image)(resources.GetObject("btnChangeConnection.Image")));
            this.btnChangeConnection.Location = new System.Drawing.Point(300, 12);
            this.btnChangeConnection.Name = "btnChangeConnection";
            this.btnChangeConnection.Size = new System.Drawing.Size(34, 23);
            this.btnChangeConnection.TabIndex = 7;
            this.btnChangeConnection.UseVisualStyleBackColor = true;
            // 
            // frmInserts
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(346, 425);
            this.Controls.Add(this.btnChangeConnection);
            this.Controls.Add(this.ssStateBar);
            this.Controls.Add(this.btnOptions);
            this.Controls.Add(this.chkSelectAll);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.dgvTables);
            this.Controls.Add(this.lblDatabase);
            this.Controls.Add(this.cmbDataBase);
            this.MaximizeBox = false;
            this.Name = "frmInserts";
            this.Text = "Suru Insert Generator";
            this.Load += new System.EventHandler(this.frmInserts_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTables)).EndInit();
            this.ssStateBar.ResumeLayout(false);
            this.ssStateBar.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbDataBase;
        private System.Windows.Forms.Label lblDatabase;
        private System.Windows.Forms.DataGridView dgvTables;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox chkSelectAll;
        private System.Windows.Forms.Button btnOptions;
        private System.Windows.Forms.StatusStrip ssStateBar;
        private System.Windows.Forms.ToolStripStatusLabel tssServerName;
        private System.Windows.Forms.ToolStripStatusLabel tssUserName;
        private System.Windows.Forms.Button btnChangeConnection;
    }
}