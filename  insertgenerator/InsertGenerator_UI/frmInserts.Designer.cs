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
            this.btnGenerateInserts = new System.Windows.Forms.Button();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.btnOptions = new System.Windows.Forms.Button();
            this.ssStateBar = new System.Windows.Forms.StatusStrip();
            this.tssServerName = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssArrowKey = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssUserName = new System.Windows.Forms.ToolStripStatusLabel();
            this.tssErrorMessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnChangeConnection = new System.Windows.Forms.Button();
            this.lblOutputPathLabel = new System.Windows.Forms.Label();
            this.lblOutputPath = new System.Windows.Forms.Label();
            this.btnChangePath = new System.Windows.Forms.Button();
            this.fbdBrowseDirectory = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTables)).BeginInit();
            this.ssStateBar.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbDataBase
            // 
            this.cmbDataBase.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDataBase.FormattingEnabled = true;
            this.cmbDataBase.Location = new System.Drawing.Point(89, 12);
            this.cmbDataBase.Name = "cmbDataBase";
            this.cmbDataBase.Size = new System.Drawing.Size(219, 21);
            this.cmbDataBase.TabIndex = 0;
            this.cmbDataBase.SelectedIndexChanged += new System.EventHandler(this.cmbDataBase_SelectedIndexChanged);
            // 
            // lblDatabase
            // 
            this.lblDatabase.AutoSize = true;
            this.lblDatabase.Location = new System.Drawing.Point(10, 15);
            this.lblDatabase.Name = "lblDatabase";
            this.lblDatabase.Size = new System.Drawing.Size(54, 13);
            this.lblDatabase.TabIndex = 1;
            this.lblDatabase.Text = "&DataBase";
            // 
            // dgvTables
            // 
            this.dgvTables.AllowUserToAddRows = false;
            this.dgvTables.AllowUserToDeleteRows = false;
            this.dgvTables.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvTables.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTables.Location = new System.Drawing.Point(0, 129);
            this.dgvTables.Name = "dgvTables";
            this.dgvTables.RowHeadersVisible = false;
            this.dgvTables.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTables.Size = new System.Drawing.Size(360, 313);
            this.dgvTables.TabIndex = 2;
            this.dgvTables.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvTables_KeyDown);
            // 
            // btnGenerateInserts
            // 
            this.btnGenerateInserts.Location = new System.Drawing.Point(90, 102);
            this.btnGenerateInserts.Name = "btnGenerateInserts";
            this.btnGenerateInserts.Size = new System.Drawing.Size(105, 23);
            this.btnGenerateInserts.TabIndex = 3;
            this.btnGenerateInserts.Text = "&Generate Inserts";
            this.btnGenerateInserts.UseVisualStyleBackColor = true;
            this.btnGenerateInserts.Click += new System.EventHandler(this.btnGenerateInserts_Click);
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Location = new System.Drawing.Point(12, 106);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(70, 17);
            this.chkSelectAll.TabIndex = 4;
            this.chkSelectAll.Text = "Select &All";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // btnOptions
            // 
            this.btnOptions.Location = new System.Drawing.Point(201, 102);
            this.btnOptions.Name = "btnOptions";
            this.btnOptions.Size = new System.Drawing.Size(108, 23);
            this.btnOptions.TabIndex = 5;
            this.btnOptions.Text = "&Options >>";
            this.btnOptions.UseVisualStyleBackColor = true;
            this.btnOptions.Click += new System.EventHandler(this.btnOptions_Click);
            // 
            // ssStateBar
            // 
            this.ssStateBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssServerName,
            this.tssArrowKey,
            this.tssUserName,
            this.tssErrorMessage});
            this.ssStateBar.Location = new System.Drawing.Point(0, 445);
            this.ssStateBar.Name = "ssStateBar";
            this.ssStateBar.Size = new System.Drawing.Size(360, 22);
            this.ssStateBar.TabIndex = 6;
            // 
            // tssServerName
            // 
            this.tssServerName.Name = "tssServerName";
            this.tssServerName.Size = new System.Drawing.Size(77, 17);
            this.tssServerName.Text = "[Server Name]";
            // 
            // tssArrowKey
            // 
            this.tssArrowKey.Name = "tssArrowKey";
            this.tssArrowKey.Size = new System.Drawing.Size(15, 17);
            this.tssArrowKey.Text = "[]";
            // 
            // tssUserName
            // 
            this.tssUserName.Name = "tssUserName";
            this.tssUserName.Size = new System.Drawing.Size(67, 17);
            this.tssUserName.Text = "[User Name]";
            // 
            // tssErrorMessage
            // 
            this.tssErrorMessage.Name = "tssErrorMessage";
            this.tssErrorMessage.Size = new System.Drawing.Size(119, 17);
            this.tssErrorMessage.Text = "[ErrorMessage (if any)]";
            // 
            // btnChangeConnection
            // 
            this.btnChangeConnection.FlatAppearance.BorderSize = 0;
            this.btnChangeConnection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChangeConnection.ForeColor = System.Drawing.Color.Transparent;
            this.btnChangeConnection.Image = ((System.Drawing.Image)(resources.GetObject("btnChangeConnection.Image")));
            this.btnChangeConnection.Location = new System.Drawing.Point(314, 10);
            this.btnChangeConnection.Name = "btnChangeConnection";
            this.btnChangeConnection.Size = new System.Drawing.Size(22, 23);
            this.btnChangeConnection.TabIndex = 7;
            this.btnChangeConnection.UseVisualStyleBackColor = true;
            this.btnChangeConnection.Click += new System.EventHandler(this.btnChangeConnection_Click);
            // 
            // lblOutputPathLabel
            // 
            this.lblOutputPathLabel.AutoSize = true;
            this.lblOutputPathLabel.Location = new System.Drawing.Point(12, 47);
            this.lblOutputPathLabel.Name = "lblOutputPathLabel";
            this.lblOutputPathLabel.Size = new System.Drawing.Size(64, 13);
            this.lblOutputPathLabel.TabIndex = 8;
            this.lblOutputPathLabel.Text = "Output Path";
            // 
            // lblOutputPath
            // 
            this.lblOutputPath.AutoSize = true;
            this.lblOutputPath.Location = new System.Drawing.Point(12, 73);
            this.lblOutputPath.Name = "lblOutputPath";
            this.lblOutputPath.Size = new System.Drawing.Size(34, 13);
            this.lblOutputPath.TabIndex = 9;
            this.lblOutputPath.Text = "[path]";
            // 
            // btnChangePath
            // 
            this.btnChangePath.FlatAppearance.BorderSize = 0;
            this.btnChangePath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChangePath.Image = ((System.Drawing.Image)(resources.GetObject("btnChangePath.Image")));
            this.btnChangePath.Location = new System.Drawing.Point(90, 42);
            this.btnChangePath.Name = "btnChangePath";
            this.btnChangePath.Size = new System.Drawing.Size(21, 23);
            this.btnChangePath.TabIndex = 10;
            this.btnChangePath.UseVisualStyleBackColor = true;
            this.btnChangePath.Click += new System.EventHandler(this.btnChangePath_Click);
            // 
            // frmInserts
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(360, 467);
            this.Controls.Add(this.btnChangePath);
            this.Controls.Add(this.lblOutputPath);
            this.Controls.Add(this.lblOutputPathLabel);
            this.Controls.Add(this.btnChangeConnection);
            this.Controls.Add(this.ssStateBar);
            this.Controls.Add(this.btnOptions);
            this.Controls.Add(this.chkSelectAll);
            this.Controls.Add(this.btnGenerateInserts);
            this.Controls.Add(this.dgvTables);
            this.Controls.Add(this.lblDatabase);
            this.Controls.Add(this.cmbDataBase);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frmInserts";
            this.Text = "Suru Insert Generator";
            this.Load += new System.EventHandler(this.frmInserts_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmInserts_FormClosing);
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
        private System.Windows.Forms.Button btnGenerateInserts;
        private System.Windows.Forms.CheckBox chkSelectAll;
        private System.Windows.Forms.Button btnOptions;
        private System.Windows.Forms.StatusStrip ssStateBar;
        private System.Windows.Forms.ToolStripStatusLabel tssServerName;
        private System.Windows.Forms.ToolStripStatusLabel tssUserName;
        private System.Windows.Forms.Button btnChangeConnection;
        private System.Windows.Forms.ToolStripStatusLabel tssErrorMessage;
        private System.Windows.Forms.ToolStripStatusLabel tssArrowKey;
        private System.Windows.Forms.Label lblOutputPathLabel;
        private System.Windows.Forms.Label lblOutputPath;
        private System.Windows.Forms.Button btnChangePath;
        private System.Windows.Forms.FolderBrowserDialog fbdBrowseDirectory;
    }
}