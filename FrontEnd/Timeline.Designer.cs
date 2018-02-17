namespace TBRBooker.FrontEnd
{
    partial class Timeline
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
            this.Box = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.setTimeMnu = new System.Windows.Forms.ToolStripMenuItem();
            this.openBookingMnu = new System.Windows.Forms.ToolStripMenuItem();
            this.timeTmr = new System.Windows.Forms.Timer(this.components);
            this.selectedLbl = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.Box)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Box
            // 
            this.Box.BackColor = System.Drawing.Color.Transparent;
            this.Box.ContextMenuStrip = this.contextMenuStrip1;
            this.Box.Location = new System.Drawing.Point(3, 3);
            this.Box.Name = "Box";
            this.Box.Size = new System.Drawing.Size(891, 88);
            this.Box.TabIndex = 0;
            this.Box.TabStop = false;
            this.Box.Click += new System.EventHandler(this.Box_Click);
            this.Box.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.Box_MouseDoubleClick);
            this.Box.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Box_MouseDown);
            this.Box.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Box_MouseMove);
            this.Box.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Box_MouseUp);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSeparator1,
            this.setTimeMnu,
            this.openBookingMnu});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(151, 54);
            this.contextMenuStrip1.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.contextMenuStrip1_Closed);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(147, 6);
            // 
            // setTimeMnu
            // 
            this.setTimeMnu.Name = "setTimeMnu";
            this.setTimeMnu.Size = new System.Drawing.Size(150, 22);
            this.setTimeMnu.Text = "Set Time";
            this.setTimeMnu.Click += new System.EventHandler(this.setTimeMnu_Click);
            // 
            // openBookingMnu
            // 
            this.openBookingMnu.Name = "openBookingMnu";
            this.openBookingMnu.Size = new System.Drawing.Size(150, 22);
            this.openBookingMnu.Text = "Open Booking";
            this.openBookingMnu.Click += new System.EventHandler(this.openBookingMnu_Click);
            // 
            // timeTmr
            // 
            this.timeTmr.Interval = 500;
            this.timeTmr.Tick += new System.EventHandler(this.timeTmr_Tick);
            // 
            // selectedLbl
            // 
            this.selectedLbl.AutoSize = true;
            this.selectedLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectedLbl.Location = new System.Drawing.Point(15, 38);
            this.selectedLbl.Name = "selectedLbl";
            this.selectedLbl.Size = new System.Drawing.Size(131, 16);
            this.selectedLbl.TabIndex = 1;
            this.selectedLbl.Text = "Selected Booking";
            this.selectedLbl.Visible = false;
            // 
            // Timeline
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.selectedLbl);
            this.Controls.Add(this.Box);
            this.Name = "Timeline";
            this.Size = new System.Drawing.Size(900, 94);
            this.Load += new System.EventHandler(this.Timeline_Load);
            ((System.ComponentModel.ISupportInitialize)(this.Box)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox Box;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem setTimeMnu;
        private System.Windows.Forms.ToolStripMenuItem openBookingMnu;
        private System.Windows.Forms.Timer timeTmr;
        private System.Windows.Forms.Label selectedLbl;
    }
}
