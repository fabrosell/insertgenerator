namespace Suru.InsertGenerator.GeneradorUI
{
    partial class frmConnectServer
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
            this.lblServer = new System.Windows.Forms.Label();
            this.lblLogin = new System.Windows.Forms.Label();
            this.cmbLogin = new System.Windows.Forms.ComboBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.cmbServerName = new System.Windows.Forms.ComboBox();
            this.cmbAuthentication = new System.Windows.Forms.ComboBox();
            this.lblAuthentication = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkRememberPassword = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lblServer
            // 
            this.lblServer.AutoSize = true;
            this.lblServer.Location = new System.Drawing.Point(12, 15);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(72, 13);
            this.lblServer.TabIndex = 0;
            this.lblServer.Text = "&Server Name:";
            // 
            // lblLogin
            // 
            this.lblLogin.AutoSize = true;
            this.lblLogin.Location = new System.Drawing.Point(31, 69);
            this.lblLogin.Name = "lblLogin";
            this.lblLogin.Size = new System.Drawing.Size(36, 13);
            this.lblLogin.TabIndex = 4;
            this.lblLogin.Text = "&Login:";
            // 
            // cmbLogin
            // 
            this.cmbLogin.FormattingEnabled = true;
            this.cmbLogin.Location = new System.Drawing.Point(140, 66);
            this.cmbLogin.Name = "cmbLogin";
            this.cmbLogin.Size = new System.Drawing.Size(235, 21);
            this.cmbLogin.TabIndex = 5;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(92, 153);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(98, 23);
            this.btnConnect.TabIndex = 9;
            this.btnConnect.Text = "&Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // cmbServerName
            // 
            this.cmbServerName.FormattingEnabled = true;
            this.cmbServerName.Location = new System.Drawing.Point(122, 12);
            this.cmbServerName.Name = "cmbServerName";
            this.cmbServerName.Size = new System.Drawing.Size(253, 21);
            this.cmbServerName.TabIndex = 1;
            this.cmbServerName.SelectedIndexChanged += new System.EventHandler(this.cmbServerName_SelectedIndexChanged);
            // 
            // cmbAuthentication
            // 
            this.cmbAuthentication.FormattingEnabled = true;
            this.cmbAuthentication.Location = new System.Drawing.Point(122, 39);
            this.cmbAuthentication.Name = "cmbAuthentication";
            this.cmbAuthentication.Size = new System.Drawing.Size(253, 21);
            this.cmbAuthentication.TabIndex = 3;
            this.cmbAuthentication.SelectedIndexChanged += new System.EventHandler(this.cmbAuthentication_SelectedIndexChanged);
            // 
            // lblAuthentication
            // 
            this.lblAuthentication.AutoSize = true;
            this.lblAuthentication.Location = new System.Drawing.Point(12, 42);
            this.lblAuthentication.Name = "lblAuthentication";
            this.lblAuthentication.Size = new System.Drawing.Size(78, 13);
            this.lblAuthentication.TabIndex = 2;
            this.lblAuthentication.Text = "&Authentication:";
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(31, 93);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(56, 13);
            this.lblPassword.TabIndex = 6;
            this.lblPassword.Text = "&Password:";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(140, 93);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '●';
            this.txtPassword.Size = new System.Drawing.Size(235, 20);
            this.txtPassword.TabIndex = 7;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(196, 153);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(98, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // chkRememberPassword
            // 
            this.chkRememberPassword.AutoSize = true;
            this.chkRememberPassword.Location = new System.Drawing.Point(140, 119);
            this.chkRememberPassword.Name = "chkRememberPassword";
            this.chkRememberPassword.Size = new System.Drawing.Size(126, 17);
            this.chkRememberPassword.TabIndex = 8;
            this.chkRememberPassword.Text = "Remember Password";
            this.chkRememberPassword.UseVisualStyleBackColor = true;
            // 
            // frmConnectServer
            // 
            this.AcceptButton = this.btnConnect;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(387, 187);
            this.Controls.Add(this.chkRememberPassword);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.cmbAuthentication);
            this.Controls.Add(this.lblAuthentication);
            this.Controls.Add(this.cmbServerName);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.cmbLogin);
            this.Controls.Add(this.lblLogin);
            this.Controls.Add(this.lblServer);
            this.MaximizeBox = false;
            this.Name = "frmConnectServer";
            this.Text = "Connect to Server";
            this.Load += new System.EventHandler(this.frmConnectServer_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.Label lblLogin;
        private System.Windows.Forms.ComboBox cmbLogin;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.ComboBox cmbServerName;
        private System.Windows.Forms.ComboBox cmbAuthentication;
        private System.Windows.Forms.Label lblAuthentication;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chkRememberPassword;
    }
}

