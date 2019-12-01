namespace TBRBooker.FrontEnd
{
    partial class InputDialog
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.descFld = new System.Windows.Forms.Label();
            this.inputFld = new System.Windows.Forms.TextBox();
            this.saveBtn = new System.Windows.Forms.Button();
            this.closeBtn = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.descFld);
            this.panel1.Controls.Add(this.inputFld);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(463, 97);
            this.panel1.TabIndex = 0;
            // 
            // descFld
            // 
            this.descFld.AutoSize = true;
            this.descFld.Location = new System.Drawing.Point(7, 8);
            this.descFld.Name = "descFld";
            this.descFld.Size = new System.Drawing.Size(35, 13);
            this.descFld.TabIndex = 1;
            this.descFld.Text = "label1";
            // 
            // inputFld
            // 
            this.inputFld.Location = new System.Drawing.Point(10, 26);
            this.inputFld.Multiline = true;
            this.inputFld.Name = "inputFld";
            this.inputFld.Size = new System.Drawing.Size(442, 61);
            this.inputFld.TabIndex = 0;
            this.inputFld.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.inputFld_KeyPress);
            // 
            // saveBtn
            // 
            this.saveBtn.Location = new System.Drawing.Point(400, 115);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(75, 23);
            this.saveBtn.TabIndex = 4;
            this.saveBtn.Text = "Ok";
            this.saveBtn.UseVisualStyleBackColor = true;
            this.saveBtn.Click += new System.EventHandler(this.saveBtn_Click);
            // 
            // closeBtn
            // 
            this.closeBtn.Location = new System.Drawing.Point(319, 115);
            this.closeBtn.Name = "closeBtn";
            this.closeBtn.Size = new System.Drawing.Size(75, 23);
            this.closeBtn.TabIndex = 3;
            this.closeBtn.Text = "Cancel";
            this.closeBtn.UseVisualStyleBackColor = true;
            this.closeBtn.Click += new System.EventHandler(this.closeBtn_Click);
            // 
            // InputDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(168)))), ((int)(((byte)(239)))));
            this.ClientSize = new System.Drawing.Size(491, 150);
            this.Controls.Add(this.saveBtn);
            this.Controls.Add(this.closeBtn);
            this.Controls.Add(this.panel1);
            this.Name = "InputDialog";
            this.Text = "Input Required";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox inputFld;
        private System.Windows.Forms.Label descFld;
        private System.Windows.Forms.Button saveBtn;
        private System.Windows.Forms.Button closeBtn;
    }
}