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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BookingsFrm));
            this.leftTabs = new System.Windows.Forms.TabControl();
            this.rightTabs = new System.Windows.Forms.TabControl();
            this.SuspendLayout();
            // 
            // leftTabs
            // 
            this.leftTabs.Location = new System.Drawing.Point(5, 5);
            this.leftTabs.Name = "leftTabs";
            this.leftTabs.SelectedIndex = 0;
            this.leftTabs.Size = new System.Drawing.Size(945, 990);
            this.leftTabs.TabIndex = 0;
            // 
            // rightTabs
            // 
            this.rightTabs.Location = new System.Drawing.Point(954, 5);
            this.rightTabs.Name = "rightTabs";
            this.rightTabs.SelectedIndex = 0;
            this.rightTabs.Size = new System.Drawing.Size(945, 990);
            this.rightTabs.TabIndex = 1;
            // 
            // BookingsFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(168)))), ((int)(((byte)(239)))));
            this.ClientSize = new System.Drawing.Size(1904, 1001);
            this.Controls.Add(this.rightTabs);
            this.Controls.Add(this.leftTabs);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BookingsFrm";
            this.Text = "BookingsFrm";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BookingsFrm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl leftTabs;
        private System.Windows.Forms.TabControl rightTabs;
    }
}