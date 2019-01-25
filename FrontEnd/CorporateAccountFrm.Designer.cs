namespace TBRBooker.FrontEnd
{
    partial class CorporateAccountFrm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CorporateAccountFrm));
            this.contactGrp = new System.Windows.Forms.GroupBox();
            this.copyAddressBtn = new System.Windows.Forms.Button();
            this.copyContactBtn = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.addressFld = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.contactEmailFld = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.contactSecondaryNumFld = new System.Windows.Forms.TextBox();
            this.contactPrimaryNumFld = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.contactNameFld = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tradingNameFld = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.businessNameFld = new System.Windows.Forms.TextBox();
            this.abnFld = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.notesFld = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.specialFld = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.bookingsFld = new System.Windows.Forms.Label();
            this.pastBookingsLst = new System.Windows.Forms.ListView();
            this.bookingsContextMnu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openBookingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useAsDefaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveBtn = new System.Windows.Forms.Button();
            this.closeBtn = new System.Windows.Forms.Button();
            this.savedFld = new System.Windows.Forms.Label();
            this.savedTmr = new System.Windows.Forms.Timer(this.components);
            this.ledgerBtn = new System.Windows.Forms.Button();
            this.contactGrp.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.bookingsContextMnu.SuspendLayout();
            this.SuspendLayout();
            // 
            // contactGrp
            // 
            this.contactGrp.BackColor = System.Drawing.SystemColors.Control;
            this.contactGrp.Controls.Add(this.copyAddressBtn);
            this.contactGrp.Controls.Add(this.copyContactBtn);
            this.contactGrp.Controls.Add(this.label4);
            this.contactGrp.Controls.Add(this.addressFld);
            this.contactGrp.Controls.Add(this.label1);
            this.contactGrp.Controls.Add(this.contactEmailFld);
            this.contactGrp.Controls.Add(this.label18);
            this.contactGrp.Controls.Add(this.contactSecondaryNumFld);
            this.contactGrp.Controls.Add(this.contactPrimaryNumFld);
            this.contactGrp.Controls.Add(this.label6);
            this.contactGrp.Controls.Add(this.contactNameFld);
            this.contactGrp.Controls.Add(this.label2);
            this.contactGrp.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.contactGrp.Location = new System.Drawing.Point(6, 122);
            this.contactGrp.Name = "contactGrp";
            this.contactGrp.Size = new System.Drawing.Size(782, 172);
            this.contactGrp.TabIndex = 1;
            this.contactGrp.TabStop = false;
            this.contactGrp.Text = "Billing";
            // 
            // copyAddressBtn
            // 
            this.copyAddressBtn.Location = new System.Drawing.Point(569, 47);
            this.copyAddressBtn.Name = "copyAddressBtn";
            this.copyAddressBtn.Size = new System.Drawing.Size(160, 23);
            this.copyAddressBtn.TabIndex = 25;
            this.copyAddressBtn.Text = "Copy from Booking Address";
            this.copyAddressBtn.UseVisualStyleBackColor = true;
            this.copyAddressBtn.Click += new System.EventHandler(this.copyAddressBtn_Click);
            // 
            // copyContactBtn
            // 
            this.copyContactBtn.Location = new System.Drawing.Point(569, 15);
            this.copyContactBtn.Name = "copyContactBtn";
            this.copyContactBtn.Size = new System.Drawing.Size(160, 23);
            this.copyContactBtn.TabIndex = 24;
            this.copyContactBtn.Text = "Copy from Primary Contact";
            this.copyContactBtn.UseVisualStyleBackColor = true;
            this.copyContactBtn.Click += new System.EventHandler(this.copyContactBtn_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(473, 56);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 22;
            this.label4.Text = "Billing Address";
            // 
            // addressFld
            // 
            this.addressFld.Location = new System.Drawing.Point(476, 73);
            this.addressFld.Name = "addressFld";
            this.addressFld.Size = new System.Drawing.Size(288, 20);
            this.addressFld.TabIndex = 21;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(539, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "The primary contact is the customer as setup on the booking form. The billing con" +
    "tact could be someone different";
            // 
            // contactEmailFld
            // 
            this.contactEmailFld.Location = new System.Drawing.Point(193, 114);
            this.contactEmailFld.Name = "contactEmailFld";
            this.contactEmailFld.Size = new System.Drawing.Size(218, 20);
            this.contactEmailFld.TabIndex = 5;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(190, 97);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(34, 13);
            this.label18.TabIndex = 17;
            this.label18.Text = "e-mail";
            // 
            // contactSecondaryNumFld
            // 
            this.contactSecondaryNumFld.Location = new System.Drawing.Point(18, 114);
            this.contactSecondaryNumFld.Name = "contactSecondaryNumFld";
            this.contactSecondaryNumFld.Size = new System.Drawing.Size(134, 20);
            this.contactSecondaryNumFld.TabIndex = 4;
            // 
            // contactPrimaryNumFld
            // 
            this.contactPrimaryNumFld.Location = new System.Drawing.Point(18, 136);
            this.contactPrimaryNumFld.Name = "contactPrimaryNumFld";
            this.contactPrimaryNumFld.Size = new System.Drawing.Size(134, 20);
            this.contactPrimaryNumFld.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(18, 98);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Contact Numbers";
            // 
            // contactNameFld
            // 
            this.contactNameFld.Location = new System.Drawing.Point(20, 69);
            this.contactNameFld.Name = "contactNameFld";
            this.contactNameFld.Size = new System.Drawing.Size(288, 20);
            this.contactNameFld.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Billing Contact Name";
            // 
            // tradingNameFld
            // 
            this.tradingNameFld.Location = new System.Drawing.Point(18, 63);
            this.tradingNameFld.Name = "tradingNameFld";
            this.tradingNameFld.Size = new System.Drawing.Size(168, 20);
            this.tradingNameFld.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 47);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Trading Name";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.businessNameFld);
            this.groupBox1.Controls.Add(this.abnFld);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.tradingNameFld);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.groupBox1.Location = new System.Drawing.Point(6, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(782, 104);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Company";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(15, 24);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(349, 13);
            this.label9.TabIndex = 21;
            this.label9.Text = "Address and contact details are as per the customer on the booking form";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(224, 47);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(138, 13);
            this.label10.TabIndex = 20;
            this.label10.Text = "Business Name (for invoice)";
            // 
            // businessNameFld
            // 
            this.businessNameFld.Location = new System.Drawing.Point(226, 63);
            this.businessNameFld.Name = "businessNameFld";
            this.businessNameFld.Size = new System.Drawing.Size(168, 20);
            this.businessNameFld.TabIndex = 19;
            // 
            // abnFld
            // 
            this.abnFld.Location = new System.Drawing.Point(440, 63);
            this.abnFld.Name = "abnFld";
            this.abnFld.Size = new System.Drawing.Size(168, 20);
            this.abnFld.TabIndex = 5;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(437, 46);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(29, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "ABN";
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox2.Controls.Add(this.notesFld);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.specialFld);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.bookingsFld);
            this.groupBox2.Controls.Add(this.pastBookingsLst);
            this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.groupBox2.Location = new System.Drawing.Point(6, 300);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(782, 226);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Bookings";
            // 
            // notesFld
            // 
            this.notesFld.Location = new System.Drawing.Point(226, 175);
            this.notesFld.Multiline = true;
            this.notesFld.Name = "notesFld";
            this.notesFld.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.notesFld.Size = new System.Drawing.Size(540, 44);
            this.notesFld.TabIndex = 5;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(223, 159);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(62, 13);
            this.label14.TabIndex = 4;
            this.label14.Text = "Other notes";
            // 
            // specialFld
            // 
            this.specialFld.Location = new System.Drawing.Point(227, 68);
            this.specialFld.Multiline = true;
            this.specialFld.Name = "specialFld";
            this.specialFld.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.specialFld.Size = new System.Drawing.Size(536, 71);
            this.specialFld.TabIndex = 3;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(224, 52);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(161, 13);
            this.label13.TabIndex = 2;
            this.label13.Text = "Special prices and arrangements";
            // 
            // bookingsFld
            // 
            this.bookingsFld.AutoSize = true;
            this.bookingsFld.Location = new System.Drawing.Point(223, 20);
            this.bookingsFld.Name = "bookingsFld";
            this.bookingsFld.Size = new System.Drawing.Size(296, 13);
            this.bookingsFld.TabIndex = 1;
            this.bookingsFld.Text = "TBR Booker acknowledges <X> bookings with this company.";
            // 
            // pastBookingsLst
            // 
            this.pastBookingsLst.CheckBoxes = true;
            this.pastBookingsLst.ContextMenuStrip = this.bookingsContextMnu;
            this.pastBookingsLst.HoverSelection = true;
            this.pastBookingsLst.Location = new System.Drawing.Point(18, 20);
            this.pastBookingsLst.Name = "pastBookingsLst";
            this.pastBookingsLst.Size = new System.Drawing.Size(186, 200);
            this.pastBookingsLst.TabIndex = 0;
            this.pastBookingsLst.UseCompatibleStateImageBehavior = false;
            this.pastBookingsLst.View = System.Windows.Forms.View.List;
            this.pastBookingsLst.ItemActivate += new System.EventHandler(this.pastBookingsLst_ItemActivate);
            this.pastBookingsLst.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.pastBookingsLst_ItemChecked);
            // 
            // bookingsContextMnu
            // 
            this.bookingsContextMnu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openBookingToolStripMenuItem,
            this.useAsDefaultToolStripMenuItem});
            this.bookingsContextMnu.Name = "bookingsContextMnu";
            this.bookingsContextMnu.Size = new System.Drawing.Size(347, 48);
            // 
            // openBookingToolStripMenuItem
            // 
            this.openBookingToolStripMenuItem.Name = "openBookingToolStripMenuItem";
            this.openBookingToolStripMenuItem.Size = new System.Drawing.Size(346, 22);
            this.openBookingToolStripMenuItem.Text = "Open Booking";
            this.openBookingToolStripMenuItem.Click += new System.EventHandler(this.openBookingToolStripMenuItem_Click);
            // 
            // useAsDefaultToolStripMenuItem
            // 
            this.useAsDefaultToolStripMenuItem.Name = "useAsDefaultToolStripMenuItem";
            this.useAsDefaultToolStripMenuItem.Size = new System.Drawing.Size(346, 22);
            this.useAsDefaultToolStripMenuItem.Text = "Set as Default (copy details/prices to new bookings)";
            this.useAsDefaultToolStripMenuItem.Click += new System.EventHandler(this.useAsDefaultToolStripMenuItem_Click);
            // 
            // saveBtn
            // 
            this.saveBtn.Location = new System.Drawing.Point(713, 544);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(75, 23);
            this.saveBtn.TabIndex = 14;
            this.saveBtn.Text = "Save";
            this.saveBtn.UseVisualStyleBackColor = true;
            this.saveBtn.Click += new System.EventHandler(this.saveBtn_Click);
            // 
            // closeBtn
            // 
            this.closeBtn.Location = new System.Drawing.Point(632, 544);
            this.closeBtn.Name = "closeBtn";
            this.closeBtn.Size = new System.Drawing.Size(75, 23);
            this.closeBtn.TabIndex = 13;
            this.closeBtn.Text = "Close";
            this.closeBtn.UseVisualStyleBackColor = true;
            this.closeBtn.Click += new System.EventHandler(this.closeBtn_Click);
            // 
            // savedFld
            // 
            this.savedFld.AutoSize = true;
            this.savedFld.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.savedFld.ForeColor = System.Drawing.Color.Black;
            this.savedFld.Location = new System.Drawing.Point(11, 545);
            this.savedFld.Name = "savedFld";
            this.savedFld.Size = new System.Drawing.Size(184, 25);
            this.savedFld.TabIndex = 15;
            this.savedFld.Text = "Changes Saved :)";
            this.savedFld.Visible = false;
            // 
            // savedTmr
            // 
            this.savedTmr.Interval = 3000;
            this.savedTmr.Tick += new System.EventHandler(this.savedTmr_Tick);
            // 
            // ledgerBtn
            // 
            this.ledgerBtn.Location = new System.Drawing.Point(482, 544);
            this.ledgerBtn.Name = "ledgerBtn";
            this.ledgerBtn.Size = new System.Drawing.Size(75, 23);
            this.ledgerBtn.TabIndex = 16;
            this.ledgerBtn.Text = "Ledger";
            this.ledgerBtn.UseVisualStyleBackColor = true;
            this.ledgerBtn.Click += new System.EventHandler(this.ledgerBtn_Click);
            // 
            // CorporateAccountFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(168)))), ((int)(((byte)(239)))));
            this.ClientSize = new System.Drawing.Size(800, 579);
            this.Controls.Add(this.ledgerBtn);
            this.Controls.Add(this.savedFld);
            this.Controls.Add(this.saveBtn);
            this.Controls.Add(this.closeBtn);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.contactGrp);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CorporateAccountFrm";
            this.Text = "Corporate Account";
            this.Load += new System.EventHandler(this.CorporateAccountFrm_Load);
            this.contactGrp.ResumeLayout(false);
            this.contactGrp.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.bookingsContextMnu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox contactGrp;
        private System.Windows.Forms.TextBox contactEmailFld;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox contactSecondaryNumFld;
        private System.Windows.Forms.TextBox contactPrimaryNumFld;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tradingNameFld;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox contactNameFld;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox abnFld;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox businessNameFld;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button copyAddressBtn;
        private System.Windows.Forms.Button copyContactBtn;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox addressFld;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListView pastBookingsLst;
        private System.Windows.Forms.TextBox notesFld;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox specialFld;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label bookingsFld;
        private System.Windows.Forms.ContextMenuStrip bookingsContextMnu;
        private System.Windows.Forms.ToolStripMenuItem openBookingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useAsDefaultToolStripMenuItem;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button saveBtn;
        private System.Windows.Forms.Button closeBtn;
        private System.Windows.Forms.Label savedFld;
        private System.Windows.Forms.Timer savedTmr;
        private System.Windows.Forms.Button ledgerBtn;
    }
}