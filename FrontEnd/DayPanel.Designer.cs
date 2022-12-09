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
            this.components = new System.ComponentModel.Container();
            this.addEnquiryBtn = new System.Windows.Forms.Button();
            this.dateLbl = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.createGoogleEventToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nextBtn = new System.Windows.Forms.Button();
            this.prevBtn = new System.Windows.Forms.Button();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // addEnquiryBtn
            // 
            this.addEnquiryBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addEnquiryBtn.Location = new System.Drawing.Point(177, 175);
            this.addEnquiryBtn.Name = "addEnquiryBtn";
            this.addEnquiryBtn.Size = new System.Drawing.Size(30, 32);
            this.addEnquiryBtn.TabIndex = 5;
            this.addEnquiryBtn.Text = "+";
            this.addEnquiryBtn.UseVisualStyleBackColor = true;
            this.addEnquiryBtn.Click += new System.EventHandler(this.addLeadBtn_Click);
            // 
            // dateLbl
            // 
            this.dateLbl.AutoSize = true;
            this.dateLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateLbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(168)))), ((int)(((byte)(239)))));
            this.dateLbl.Location = new System.Drawing.Point(4, 4);
            this.dateLbl.Name = "dateLbl";
            this.dateLbl.Size = new System.Drawing.Size(15, 16);
            this.dateLbl.TabIndex = 6;
            this.dateLbl.Text = "1";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createGoogleEventToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(182, 26);
            // 
            // createGoogleEventToolStripMenuItem
            // 
            this.createGoogleEventToolStripMenuItem.Name = "createGoogleEventToolStripMenuItem";
            this.createGoogleEventToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
            this.createGoogleEventToolStripMenuItem.Text = "Create Google Event";
            this.createGoogleEventToolStripMenuItem.Click += new System.EventHandler(this.createGoogleEventToolStripMenuItem_Click);
            // 
            // nextBtn
            // 
            this.nextBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nextBtn.Location = new System.Drawing.Point(89, 175);
            this.nextBtn.Name = "nextBtn";
            this.nextBtn.Size = new System.Drawing.Size(60, 32);
            this.nextBtn.TabIndex = 7;
            this.nextBtn.Text = ">>>";
            this.nextBtn.UseVisualStyleBackColor = true;
            this.nextBtn.Visible = false;
            this.nextBtn.Click += new System.EventHandler(this.nextBtn_Click);
            // 
            // prevBtn
            // 
            this.prevBtn.Enabled = false;
            this.prevBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.prevBtn.Location = new System.Drawing.Point(23, 175);
            this.prevBtn.Name = "prevBtn";
            this.prevBtn.Size = new System.Drawing.Size(60, 32);
            this.prevBtn.TabIndex = 8;
            this.prevBtn.Text = "<<<";
            this.prevBtn.UseVisualStyleBackColor = true;
            this.prevBtn.Visible = false;
            this.prevBtn.Click += new System.EventHandler(this.prevBtn_Click);
            // 
            // DayPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.Controls.Add(this.prevBtn);
            this.Controls.Add(this.nextBtn);
            this.Controls.Add(this.dateLbl);
            this.Controls.Add(this.addEnquiryBtn);
            this.Name = "DayPanel";
            this.Size = new System.Drawing.Size(210, 210);
            this.Load += new System.EventHandler(this.DayPanel_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button addEnquiryBtn;
        private System.Windows.Forms.Label dateLbl;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem createGoogleEventToolStripMenuItem;
        private System.Windows.Forms.Button nextBtn;
        private System.Windows.Forms.Button prevBtn;
    }
}
