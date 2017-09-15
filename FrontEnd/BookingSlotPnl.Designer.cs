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
            // 
            // BookingSlotPnl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.timeLbl);
            this.Controls.Add(this.bookingLbl);
            this.Name = "BookingSlotPnl";
            this.Size = new System.Drawing.Size(210, 52);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label bookingLbl;
        private System.Windows.Forms.Label timeLbl;
    }
}
