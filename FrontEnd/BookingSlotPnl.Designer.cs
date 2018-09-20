namespace TBRBooker.FrontEnd
{
    partial class BookingSlotPnl
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
            this.bookingLbl = new System.Windows.Forms.Label();
            this.timeLbl = new System.Windows.Forms.Label();
            this.confirmBtn = new System.Windows.Forms.Button();
            this.rejectBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bookingLbl
            // 
            this.bookingLbl.AutoSize = true;
            this.bookingLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bookingLbl.Location = new System.Drawing.Point(44, 18);
            this.bookingLbl.Name = "bookingLbl";
            this.bookingLbl.Size = new System.Drawing.Size(123, 16);
            this.bookingLbl.TabIndex = 0;
            this.bookingLbl.Text = "17123 Tchakoivsky";
            this.bookingLbl.Click += new System.EventHandler(this.bookingLbl_Click);
            this.bookingLbl.MouseEnter += new System.EventHandler(this.bookingLbl_MouseEnter);
            this.bookingLbl.MouseLeave += new System.EventHandler(this.bookingLbl_MouseLeave);
            // 
            // timeLbl
            // 
            this.timeLbl.AutoSize = true;
            this.timeLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.timeLbl.Location = new System.Drawing.Point(3, 18);
            this.timeLbl.Name = "timeLbl";
            this.timeLbl.Size = new System.Drawing.Size(39, 16);
            this.timeLbl.TabIndex = 1;
            this.timeLbl.Text = "15:30";
            this.timeLbl.Click += new System.EventHandler(this.timeLbl_Click);
            this.timeLbl.MouseEnter += new System.EventHandler(this.timeLbl_MouseEnter);
            this.timeLbl.MouseLeave += new System.EventHandler(this.timeLbl_MouseLeave);
            // 
            // confirmBtn
            // 
            this.confirmBtn.BackgroundImage = global::TBRBooker.FrontEnd.Properties.Resources.greentick;
            this.confirmBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.confirmBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.confirmBtn.Location = new System.Drawing.Point(184, 26);
            this.confirmBtn.Name = "confirmBtn";
            this.confirmBtn.Size = new System.Drawing.Size(25, 25);
            this.confirmBtn.TabIndex = 3;
            this.confirmBtn.UseVisualStyleBackColor = true;
            this.confirmBtn.Visible = false;
            this.confirmBtn.Click += new System.EventHandler(this.confirmBtn_Click);
            // 
            // rejectBtn
            // 
            this.rejectBtn.BackgroundImage = global::TBRBooker.FrontEnd.Properties.Resources.redcross;
            this.rejectBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.rejectBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rejectBtn.Location = new System.Drawing.Point(151, 26);
            this.rejectBtn.Name = "rejectBtn";
            this.rejectBtn.Size = new System.Drawing.Size(25, 25);
            this.rejectBtn.TabIndex = 2;
            this.rejectBtn.UseVisualStyleBackColor = true;
            this.rejectBtn.Visible = false;
            this.rejectBtn.Click += new System.EventHandler(this.rejectBtn_Click);
            // 
            // BookingSlotPnl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.confirmBtn);
            this.Controls.Add(this.rejectBtn);
            this.Controls.Add(this.timeLbl);
            this.Controls.Add(this.bookingLbl);
            this.Name = "BookingSlotPnl";
            this.Size = new System.Drawing.Size(210, 52);
            this.Load += new System.EventHandler(this.BookingSlotPnl_Load);
            this.MouseEnter += new System.EventHandler(this.BookingSlotPnl_MouseEnter);
            this.MouseLeave += new System.EventHandler(this.BookingSlotPnl_MouseLeave);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BookingSlotPnl_MouseUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label bookingLbl;
        private System.Windows.Forms.Label timeLbl;
        private System.Windows.Forms.Button rejectBtn;
        private System.Windows.Forms.Button confirmBtn;
    }
}
