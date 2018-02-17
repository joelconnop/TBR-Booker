namespace TBRBooker.FrontEnd
{
    partial class BookingsFrm
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
            this.tabs1 = new System.Windows.Forms.TabControl();
            this.tabs2 = new System.Windows.Forms.TabControl();
            this.SuspendLayout();
            // 
            // tabs1
            // 
            this.tabs1.Location = new System.Drawing.Point(5, 5);
            this.tabs1.Name = "tabs1";
            this.tabs1.SelectedIndex = 0;
            this.tabs1.Size = new System.Drawing.Size(945, 990);
            this.tabs1.TabIndex = 0;
            // 
            // tabs2
            // 
            this.tabs2.Location = new System.Drawing.Point(954, 5);
            this.tabs2.Name = "tabs2";
            this.tabs2.SelectedIndex = 0;
            this.tabs2.Size = new System.Drawing.Size(945, 990);
            this.tabs2.TabIndex = 1;
            // 
            // BookingsFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(168)))), ((int)(((byte)(239)))));
            this.ClientSize = new System.Drawing.Size(1904, 1001);
            this.Controls.Add(this.tabs2);
            this.Controls.Add(this.tabs1);
            this.Name = "BookingsFrm";
            this.Text = "BookingsFrm";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BookingsFrm_FormClosing);
            this.Load += new System.EventHandler(this.BookingsFrm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabs1;
        private System.Windows.Forms.TabControl tabs2;
    }
}