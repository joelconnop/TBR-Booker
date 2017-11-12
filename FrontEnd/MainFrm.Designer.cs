namespace TBRBooker.FrontEnd
{
    partial class MainFrm
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
            this.mainMnu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.databaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.databaseReadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.datePicker = new System.Windows.Forms.DateTimePicker();
            this.nextBtn = new System.Windows.Forms.Button();
            this.prevBtn = new System.Windows.Forms.Button();
            this.monthsLbl = new System.Windows.Forms.Label();
            this.daysPanel = new System.Windows.Forms.Panel();
            this.mainMnu.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMnu
            // 
            this.mainMnu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.testingToolStripMenuItem});
            this.mainMnu.Location = new System.Drawing.Point(0, 0);
            this.mainMnu.Name = "mainMnu";
            this.mainMnu.Size = new System.Drawing.Size(1904, 24);
            this.mainMnu.TabIndex = 0;
            this.mainMnu.Text = "Main Menu Strip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // testingToolStripMenuItem
            // 
            this.testingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.databaseToolStripMenuItem,
            this.databaseReadToolStripMenuItem});
            this.testingToolStripMenuItem.Name = "testingToolStripMenuItem";
            this.testingToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.testingToolStripMenuItem.Text = "Testing";
            // 
            // databaseToolStripMenuItem
            // 
            this.databaseToolStripMenuItem.Name = "databaseToolStripMenuItem";
            this.databaseToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.databaseToolStripMenuItem.Text = "Database write";
            this.databaseToolStripMenuItem.Click += new System.EventHandler(this.databaseToolStripMenuItem_Click);
            // 
            // databaseReadToolStripMenuItem
            // 
            this.databaseReadToolStripMenuItem.Name = "databaseReadToolStripMenuItem";
            this.databaseReadToolStripMenuItem.Size = new System.Drawing.Size(151, 22);
            this.databaseReadToolStripMenuItem.Text = "Database read";
            this.databaseReadToolStripMenuItem.Click += new System.EventHandler(this.databaseReadToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.datePicker);
            this.panel1.Controls.Add(this.nextBtn);
            this.panel1.Controls.Add(this.prevBtn);
            this.panel1.Controls.Add(this.monthsLbl);
            this.panel1.Location = new System.Drawing.Point(0, 27);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1904, 100);
            this.panel1.TabIndex = 1;
            // 
            // datePicker
            // 
            this.datePicker.Location = new System.Drawing.Point(864, 9);
            this.datePicker.Name = "datePicker";
            this.datePicker.Size = new System.Drawing.Size(200, 20);
            this.datePicker.TabIndex = 3;
            this.datePicker.ValueChanged += new System.EventHandler(this.datePicker_ValueChanged);
            // 
            // nextBtn
            // 
            this.nextBtn.BackgroundImage = global::TBRBooker.FrontEnd.Properties.Resources.next;
            this.nextBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.nextBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.nextBtn.Location = new System.Drawing.Point(1260, 27);
            this.nextBtn.Name = "nextBtn";
            this.nextBtn.Size = new System.Drawing.Size(60, 60);
            this.nextBtn.TabIndex = 2;
            this.nextBtn.UseVisualStyleBackColor = true;
            this.nextBtn.Click += new System.EventHandler(this.nextBtn_Click);
            // 
            // prevBtn
            // 
            this.prevBtn.BackgroundImage = global::TBRBooker.FrontEnd.Properties.Resources.back;
            this.prevBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.prevBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.prevBtn.Location = new System.Drawing.Point(615, 27);
            this.prevBtn.Name = "prevBtn";
            this.prevBtn.Size = new System.Drawing.Size(60, 60);
            this.prevBtn.TabIndex = 1;
            this.prevBtn.UseVisualStyleBackColor = true;
            this.prevBtn.Click += new System.EventHandler(this.prevBtn_Click);
            // 
            // monthsLbl
            // 
            this.monthsLbl.AutoSize = true;
            this.monthsLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.monthsLbl.Location = new System.Drawing.Point(760, 32);
            this.monthsLbl.Name = "monthsLbl";
            this.monthsLbl.Size = new System.Drawing.Size(422, 55);
            this.monthsLbl.TabIndex = 0;
            this.monthsLbl.Text = "July 17- August 17";
            this.monthsLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // daysPanel
            // 
            this.daysPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(168)))), ((int)(((byte)(239)))));
            this.daysPanel.Location = new System.Drawing.Point(0, 134);
            this.daysPanel.Name = "daysPanel";
            this.daysPanel.Size = new System.Drawing.Size(1904, 866);
            this.daysPanel.TabIndex = 2;
            // 
            // MainFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1904, 1001);
            this.Controls.Add(this.daysPanel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.mainMnu);
            this.MainMenuStrip = this.mainMnu;
            this.Name = "MainFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "TBR Booker";
            this.Load += new System.EventHandler(this.MainFrm_Load);
            this.mainMnu.ResumeLayout(false);
            this.mainMnu.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mainMnu;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label monthsLbl;
        private System.Windows.Forms.Button prevBtn;
        private System.Windows.Forms.DateTimePicker datePicker;
        private System.Windows.Forms.Button nextBtn;
        private System.Windows.Forms.Panel daysPanel;
        private System.Windows.Forms.ToolStripMenuItem testingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem databaseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem databaseReadToolStripMenuItem;
    }
}

