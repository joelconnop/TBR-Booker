namespace TBRBooker.FrontEnd
{
    partial class SettingsManagementFrm
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
            this.SettingsGrid = new System.Windows.Forms.PropertyGrid();
            this.saveBtn = new System.Windows.Forms.Button();
            this.closeBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SettingsGrid
            // 
            this.SettingsGrid.Location = new System.Drawing.Point(2, 2);
            this.SettingsGrid.Name = "SettingsGrid";
            this.SettingsGrid.Size = new System.Drawing.Size(692, 436);
            this.SettingsGrid.TabIndex = 0;
            // 
            // saveBtn
            // 
            this.saveBtn.Location = new System.Drawing.Point(619, 451);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(75, 23);
            this.saveBtn.TabIndex = 4;
            this.saveBtn.Text = "Save";
            this.saveBtn.UseVisualStyleBackColor = true;
            this.saveBtn.Click += new System.EventHandler(this.saveBtn_Click);
            // 
            // closeBtn
            // 
            this.closeBtn.Location = new System.Drawing.Point(538, 451);
            this.closeBtn.Name = "closeBtn";
            this.closeBtn.Size = new System.Drawing.Size(75, 23);
            this.closeBtn.TabIndex = 3;
            this.closeBtn.Text = "Close";
            this.closeBtn.UseVisualStyleBackColor = true;
            this.closeBtn.Click += new System.EventHandler(this.closeBtn_Click);
            // 
            // SettingsManagementFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(168)))), ((int)(((byte)(239)))));
            this.ClientSize = new System.Drawing.Size(699, 486);
            this.Controls.Add(this.saveBtn);
            this.Controls.Add(this.closeBtn);
            this.Controls.Add(this.SettingsGrid);
            this.Name = "SettingsManagementFrm";
            this.Text = "SettingsManagementFrm";
            this.Load += new System.EventHandler(this.SettingsManagementFrm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PropertyGrid SettingsGrid;
        private System.Windows.Forms.Button saveBtn;
        private System.Windows.Forms.Button closeBtn;
    }
}