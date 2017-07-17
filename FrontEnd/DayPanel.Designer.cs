namespace FrontEnd
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
            this.amJobLbl = new System.Windows.Forms.LinkLabel();
            this.midJobLbl = new System.Windows.Forms.LinkLabel();
            this.pmJobLbl = new System.Windows.Forms.LinkLabel();
            this.otherJob1Lbl = new System.Windows.Forms.LinkLabel();
            this.linkLabel4 = new System.Windows.Forms.LinkLabel();
            this.addLeadBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // amJobLbl
            // 
            this.amJobLbl.AutoSize = true;
            this.amJobLbl.BackColor = System.Drawing.Color.Yellow;
            this.amJobLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.amJobLbl.Location = new System.Drawing.Point(5, 15);
            this.amJobLbl.Name = "amJobLbl";
            this.amJobLbl.Size = new System.Drawing.Size(112, 20);
            this.amJobLbl.TabIndex = 0;
            this.amJobLbl.TabStop = true;
            this.amJobLbl.Text = "17123 AM Job";
            // 
            // midJobLbl
            // 
            this.midJobLbl.ActiveLinkColor = System.Drawing.Color.Red;
            this.midJobLbl.AutoSize = true;
            this.midJobLbl.BackColor = System.Drawing.Color.Orange;
            this.midJobLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.midJobLbl.Location = new System.Drawing.Point(5, 55);
            this.midJobLbl.Name = "midJobLbl";
            this.midJobLbl.Size = new System.Drawing.Size(113, 20);
            this.midJobLbl.TabIndex = 1;
            this.midJobLbl.TabStop = true;
            this.midJobLbl.Text = "17124 Mid Job";
            // 
            // pmJobLbl
            // 
            this.pmJobLbl.AutoSize = true;
            this.pmJobLbl.BackColor = System.Drawing.Color.Orchid;
            this.pmJobLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pmJobLbl.Location = new System.Drawing.Point(5, 96);
            this.pmJobLbl.Name = "pmJobLbl";
            this.pmJobLbl.Size = new System.Drawing.Size(111, 20);
            this.pmJobLbl.TabIndex = 2;
            this.pmJobLbl.TabStop = true;
            this.pmJobLbl.Text = "17125 PM Job";
            // 
            // otherJob1Lbl
            // 
            this.otherJob1Lbl.AutoSize = true;
            this.otherJob1Lbl.BackColor = System.Drawing.Color.Silver;
            this.otherJob1Lbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.otherJob1Lbl.Location = new System.Drawing.Point(3, 141);
            this.otherJob1Lbl.Name = "otherJob1Lbl";
            this.otherJob1Lbl.Size = new System.Drawing.Size(107, 20);
            this.otherJob1Lbl.TabIndex = 3;
            this.otherJob1Lbl.TabStop = true;
            this.otherJob1Lbl.Text = "17126 Other1";
            // 
            // linkLabel4
            // 
            this.linkLabel4.AutoSize = true;
            this.linkLabel4.BackColor = System.Drawing.Color.Silver;
            this.linkLabel4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel4.Location = new System.Drawing.Point(3, 169);
            this.linkLabel4.Name = "linkLabel4";
            this.linkLabel4.Size = new System.Drawing.Size(107, 20);
            this.linkLabel4.TabIndex = 4;
            this.linkLabel4.TabStop = true;
            this.linkLabel4.Text = "17127 Other2";
            // 
            // addLeadBtn
            // 
            this.addLeadBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addLeadBtn.Location = new System.Drawing.Point(158, 156);
            this.addLeadBtn.Name = "addLeadBtn";
            this.addLeadBtn.Size = new System.Drawing.Size(30, 32);
            this.addLeadBtn.TabIndex = 5;
            this.addLeadBtn.Text = "+";
            this.addLeadBtn.UseVisualStyleBackColor = true;
            // 
            // DayPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(168)))), ((int)(((byte)(239)))));
            this.Controls.Add(this.addLeadBtn);
            this.Controls.Add(this.linkLabel4);
            this.Controls.Add(this.otherJob1Lbl);
            this.Controls.Add(this.pmJobLbl);
            this.Controls.Add(this.midJobLbl);
            this.Controls.Add(this.amJobLbl);
            this.Name = "DayPanel";
            this.Size = new System.Drawing.Size(210, 210);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel amJobLbl;
        private System.Windows.Forms.LinkLabel midJobLbl;
        private System.Windows.Forms.LinkLabel pmJobLbl;
        private System.Windows.Forms.LinkLabel otherJob1Lbl;
        private System.Windows.Forms.LinkLabel linkLabel4;
        private System.Windows.Forms.Button addLeadBtn;
    }
}
