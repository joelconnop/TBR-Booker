namespace TBRBooker.FrontEnd
{
    partial class DayPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.addLeadBtn = new System.Windows.Forms.Button();
            this.dateLbl = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // addLeadBtn
            // 
            this.addLeadBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addLeadBtn.Location = new System.Drawing.Point(177, 175);
            this.addLeadBtn.Name = "addLeadBtn";
            this.addLeadBtn.Size = new System.Drawing.Size(30, 32);
            this.addLeadBtn.TabIndex = 5;
            this.addLeadBtn.Text = "+";
            this.addLeadBtn.UseVisualStyleBackColor = true;
            // 
            // dateLbl
            // 
            this.dateLbl.AutoSize = true;
            this.dateLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateLbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(168)))), ((int)(((byte)(239)))));
            this.dateLbl.Location = new System.Drawing.Point(4, 4);
            this.dateLbl.Name = "dateLbl";
            this.dateLbl.Size = new System.Drawing.Size(16, 16);
            this.dateLbl.TabIndex = 6;
            this.dateLbl.Text = "1";
            // 
            // DayPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.Controls.Add(this.dateLbl);
            this.Controls.Add(this.addLeadBtn);
            this.Name = "DayPanel";
            this.Size = new System.Drawing.Size(210, 210);
            this.Load += new System.EventHandler(this.DayPanel_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button addLeadBtn;
        private System.Windows.Forms.Label dateLbl;
    }
}
