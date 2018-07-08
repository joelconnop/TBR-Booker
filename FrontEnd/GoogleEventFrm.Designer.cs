namespace TBRBooker.FrontEnd
{
    partial class GoogleEventFrm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GoogleEventFrm));
            this.closeBtn = new System.Windows.Forms.Button();
            this.saveBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.nameFld = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.descFld = new System.Windows.Forms.TextBox();
            this.contactGrp = new System.Windows.Forms.GroupBox();
            this.allDayChk = new System.Windows.Forms.CheckBox();
            this.endPick = new TBRBooker.FrontEnd.TimePicker();
            this.label29 = new System.Windows.Forms.Label();
            this.durationDescFld = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.durationFld = new System.Windows.Forms.TextBox();
            this.startPick = new TBRBooker.FrontEnd.TimePicker();
            this.label15 = new System.Windows.Forms.Label();
            this.dateFld = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.locationFld = new System.Windows.Forms.TextBox();
            this.contactGrp.SuspendLayout();
            this.SuspendLayout();
            // 
            // closeBtn
            // 
            this.closeBtn.Location = new System.Drawing.Point(216, 371);
            this.closeBtn.Name = "closeBtn";
            this.closeBtn.Size = new System.Drawing.Size(75, 23);
            this.closeBtn.TabIndex = 1;
            this.closeBtn.Text = "Close";
            this.closeBtn.UseVisualStyleBackColor = true;
            this.closeBtn.Click += new System.EventHandler(this.closeBtn_Click);
            // 
            // saveBtn
            // 
            this.saveBtn.Location = new System.Drawing.Point(297, 371);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(75, 23);
            this.saveBtn.TabIndex = 2;
            this.saveBtn.Text = "Save";
            this.saveBtn.UseVisualStyleBackColor = true;
            this.saveBtn.Click += new System.EventHandler(this.saveBtn_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Event Name";
            // 
            // nameFld
            // 
            this.nameFld.Location = new System.Drawing.Point(20, 38);
            this.nameFld.Name = "nameFld";
            this.nameFld.Size = new System.Drawing.Size(134, 20);
            this.nameFld.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 120);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(112, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "More Details (optional)";
            // 
            // descFld
            // 
            this.descFld.Location = new System.Drawing.Point(20, 137);
            this.descFld.Name = "descFld";
            this.descFld.Size = new System.Drawing.Size(308, 20);
            this.descFld.TabIndex = 2;
            // 
            // contactGrp
            // 
            this.contactGrp.BackColor = System.Drawing.SystemColors.Control;
            this.contactGrp.Controls.Add(this.locationFld);
            this.contactGrp.Controls.Add(this.label1);
            this.contactGrp.Controls.Add(this.dateFld);
            this.contactGrp.Controls.Add(this.allDayChk);
            this.contactGrp.Controls.Add(this.endPick);
            this.contactGrp.Controls.Add(this.label29);
            this.contactGrp.Controls.Add(this.durationDescFld);
            this.contactGrp.Controls.Add(this.label24);
            this.contactGrp.Controls.Add(this.durationFld);
            this.contactGrp.Controls.Add(this.startPick);
            this.contactGrp.Controls.Add(this.label15);
            this.contactGrp.Controls.Add(this.descFld);
            this.contactGrp.Controls.Add(this.label3);
            this.contactGrp.Controls.Add(this.nameFld);
            this.contactGrp.Controls.Add(this.label2);
            this.contactGrp.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.contactGrp.Location = new System.Drawing.Point(10, 10);
            this.contactGrp.Name = "contactGrp";
            this.contactGrp.Size = new System.Drawing.Size(362, 355);
            this.contactGrp.TabIndex = 0;
            this.contactGrp.TabStop = false;
            // 
            // allDayChk
            // 
            this.allDayChk.AutoSize = true;
            this.allDayChk.Location = new System.Drawing.Point(20, 204);
            this.allDayChk.Name = "allDayChk";
            this.allDayChk.Size = new System.Drawing.Size(59, 17);
            this.allDayChk.TabIndex = 3;
            this.allDayChk.Text = "All Day";
            this.allDayChk.UseVisualStyleBackColor = true;
            this.allDayChk.CheckedChanged += new System.EventHandler(this.allDayChk_CheckedChanged);
            // 
            // endPick
            // 
            this.endPick.Location = new System.Drawing.Point(20, 312);
            this.endPick.Name = "endPick";
            this.endPick.Size = new System.Drawing.Size(84, 27);
            this.endPick.TabIndex = 5;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(19, 296);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(52, 13);
            this.label29.TabIndex = 10;
            this.label29.Text = "End Time";
            // 
            // durationDescFld
            // 
            this.durationDescFld.AutoSize = true;
            this.durationDescFld.Location = new System.Drawing.Point(202, 318);
            this.durationDescFld.Name = "durationDescFld";
            this.durationDescFld.Size = new System.Drawing.Size(96, 13);
            this.durationDescFld.TabIndex = 7;
            this.durationDescFld.Text = "(0 hours 0 minutes)";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(116, 296);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(47, 13);
            this.label24.TabIndex = 9;
            this.label24.Text = "Duration";
            // 
            // durationFld
            // 
            this.durationFld.Location = new System.Drawing.Point(119, 315);
            this.durationFld.Name = "durationFld";
            this.durationFld.Size = new System.Drawing.Size(77, 20);
            this.durationFld.TabIndex = 6;
            // 
            // startPick
            // 
            this.startPick.Location = new System.Drawing.Point(16, 254);
            this.startPick.Name = "startPick";
            this.startPick.Size = new System.Drawing.Size(84, 27);
            this.startPick.TabIndex = 15;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(17, 240);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(55, 13);
            this.label15.TabIndex = 15;
            this.label15.Text = "Start Time";
            // 
            // dateFld
            // 
            this.dateFld.AutoSize = true;
            this.dateFld.Location = new System.Drawing.Point(20, 173);
            this.dateFld.Name = "dateFld";
            this.dateFld.Size = new System.Drawing.Size(30, 13);
            this.dateFld.TabIndex = 14;
            this.dateFld.Text = "Date";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Location (optional)";
            // 
            // locationFld
            // 
            this.locationFld.Location = new System.Drawing.Point(20, 86);
            this.locationFld.Name = "locationFld";
            this.locationFld.Size = new System.Drawing.Size(308, 20);
            this.locationFld.TabIndex = 1;
            // 
            // GoogleEventFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(168)))), ((int)(((byte)(239)))));
            this.ClientSize = new System.Drawing.Size(384, 406);
            this.Controls.Add(this.contactGrp);
            this.Controls.Add(this.saveBtn);
            this.Controls.Add(this.closeBtn);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GoogleEventFrm";
            this.Text = "Add Blockout Event";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Load += new System.EventHandler(this.GoogleEventFrm_Load);
            this.contactGrp.ResumeLayout(false);
            this.contactGrp.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button closeBtn;
        private System.Windows.Forms.Button saveBtn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox nameFld;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox descFld;
        private System.Windows.Forms.GroupBox contactGrp;
        private TimePicker startPick;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label durationDescFld;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox durationFld;
        private TimePicker endPick;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.CheckBox allDayChk;
        private System.Windows.Forms.Label dateFld;
        private System.Windows.Forms.TextBox locationFld;
        private System.Windows.Forms.Label label1;
    }
}