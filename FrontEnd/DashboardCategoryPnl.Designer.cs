namespace TBRBooker.FrontEnd
{
    partial class DashboardCategoryPnl
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
            this.itemsLst = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.titleLbl = new System.Windows.Forms.Label();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // itemsLst
            // 
            this.itemsLst.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader4,
            this.columnHeader2,
            this.columnHeader3});
            this.itemsLst.FullRowSelect = true;
            this.itemsLst.Location = new System.Drawing.Point(9, 30);
            this.itemsLst.MultiSelect = false;
            this.itemsLst.Name = "itemsLst";
            this.itemsLst.Size = new System.Drawing.Size(350, 240);
            this.itemsLst.TabIndex = 0;
            this.itemsLst.UseCompatibleStateImageBehavior = false;
            this.itemsLst.View = System.Windows.Forms.View.Details;
            this.itemsLst.ItemActivate += new System.EventHandler(this.itemsLst_ItemActivate);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Booking";
            this.columnHeader1.Width = 104;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Followup Date";
            this.columnHeader2.Width = 90;
            // 
            // titleLbl
            // 
            this.titleLbl.AutoSize = true;
            this.titleLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLbl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(168)))), ((int)(((byte)(239)))));
            this.titleLbl.Location = new System.Drawing.Point(6, 8);
            this.titleLbl.Name = "titleLbl";
            this.titleLbl.Size = new System.Drawing.Size(91, 16);
            this.titleLbl.TabIndex = 14;
            this.titleLbl.Text = "CATEGORY";
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Overdue";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Service Date";
            this.columnHeader4.Width = 90;
            // 
            // DashboardCategoryPnl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.Controls.Add(this.titleLbl);
            this.Controls.Add(this.itemsLst);
            this.Name = "DashboardCategoryPnl";
            this.Size = new System.Drawing.Size(370, 275);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView itemsLst;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Label titleLbl;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader3;
    }
}
