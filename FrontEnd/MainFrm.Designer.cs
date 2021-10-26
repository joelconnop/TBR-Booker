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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainFrm));
            this.mainMnu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reportsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allGeneralSummariesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generalSummaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.last30DaysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.last12MonthsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.currentFinancialYearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.previousFinancialYearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectedMonth1YearAgoJobkeeperToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.penaltiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.databaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.databaseReadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.googleCalendarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createRecurringEventToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.searchFld = new System.Windows.Forms.TextBox();
            this.refreshBtn = new System.Windows.Forms.Button();
            this.showBookingsBtn = new System.Windows.Forms.Button();
            this.switchMonitorBtn = new System.Windows.Forms.Button();
            this.datePicker = new System.Windows.Forms.DateTimePicker();
            this.nextBtn = new System.Windows.Forms.Button();
            this.prevBtn = new System.Windows.Forms.Button();
            this.monthsLbl = new System.Windows.Forms.Label();
            this.daysPanel = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.dashboardPnl = new System.Windows.Forms.Panel();
            this.searchPnl = new System.Windows.Forms.Panel();
            this.bookingLst = new System.Windows.Forms.ListView();
            this.searcCloseBtn = new System.Windows.Forms.Button();
            this.searchLst = new System.Windows.Forms.ListView();
            this.dateTmr = new System.Windows.Forms.Timer(this.components);
            this.travelLogLastFYearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainMnu.SuspendLayout();
            this.panel1.SuspendLayout();
            this.searchPnl.SuspendLayout();
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
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reportsToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // reportsToolStripMenuItem
            // 
            this.reportsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allGeneralSummariesToolStripMenuItem,
            this.generalSummaryToolStripMenuItem,
            this.travelLogLastFYearToolStripMenuItem,
            this.penaltiesToolStripMenuItem});
            this.reportsToolStripMenuItem.Name = "reportsToolStripMenuItem";
            this.reportsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.reportsToolStripMenuItem.Text = "Reports";
            // 
            // allGeneralSummariesToolStripMenuItem
            // 
            this.allGeneralSummariesToolStripMenuItem.Name = "allGeneralSummariesToolStripMenuItem";
            this.allGeneralSummariesToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.allGeneralSummariesToolStripMenuItem.Text = "All General Summaries";
            this.allGeneralSummariesToolStripMenuItem.ToolTipText = "Each report forces the same database read, so use this if you plan to view more t" +
    "han one report";
            this.allGeneralSummariesToolStripMenuItem.Click += new System.EventHandler(this.allGeneralSummariesToolStripMenuItem_Click);
            // 
            // generalSummaryToolStripMenuItem
            // 
            this.generalSummaryToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.last30DaysToolStripMenuItem,
            this.last12MonthsToolStripMenuItem,
            this.currentFinancialYearToolStripMenuItem,
            this.previousFinancialYearToolStripMenuItem,
            this.allTimeToolStripMenuItem,
            this.selectedMonth1YearAgoJobkeeperToolStripMenuItem});
            this.generalSummaryToolStripMenuItem.Name = "generalSummaryToolStripMenuItem";
            this.generalSummaryToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.generalSummaryToolStripMenuItem.Text = "General Summary";
            // 
            // last30DaysToolStripMenuItem
            // 
            this.last30DaysToolStripMenuItem.Name = "last30DaysToolStripMenuItem";
            this.last30DaysToolStripMenuItem.Size = new System.Drawing.Size(316, 22);
            this.last30DaysToolStripMenuItem.Text = "Last 30 Days (prior to selected date)";
            this.last30DaysToolStripMenuItem.Click += new System.EventHandler(this.last30DaysToolStripMenuItem_Click);
            // 
            // last12MonthsToolStripMenuItem
            // 
            this.last12MonthsToolStripMenuItem.Name = "last12MonthsToolStripMenuItem";
            this.last12MonthsToolStripMenuItem.Size = new System.Drawing.Size(316, 22);
            this.last12MonthsToolStripMenuItem.Text = "Last 12 Months (prior to selected date)";
            this.last12MonthsToolStripMenuItem.Click += new System.EventHandler(this.last12MonthsToolStripMenuItem_Click);
            // 
            // currentFinancialYearToolStripMenuItem
            // 
            this.currentFinancialYearToolStripMenuItem.Name = "currentFinancialYearToolStripMenuItem";
            this.currentFinancialYearToolStripMenuItem.Size = new System.Drawing.Size(316, 22);
            this.currentFinancialYearToolStripMenuItem.Text = "Current Financial Year (selected date)";
            this.currentFinancialYearToolStripMenuItem.Click += new System.EventHandler(this.currentFinancialYearToolStripMenuItem_Click);
            // 
            // previousFinancialYearToolStripMenuItem
            // 
            this.previousFinancialYearToolStripMenuItem.Name = "previousFinancialYearToolStripMenuItem";
            this.previousFinancialYearToolStripMenuItem.Size = new System.Drawing.Size(316, 22);
            this.previousFinancialYearToolStripMenuItem.Text = "Previous Financial Year (prior to selected date)";
            this.previousFinancialYearToolStripMenuItem.Click += new System.EventHandler(this.previousFinancialYearToolStripMenuItem_Click);
            // 
            // allTimeToolStripMenuItem
            // 
            this.allTimeToolStripMenuItem.Name = "allTimeToolStripMenuItem";
            this.allTimeToolStripMenuItem.Size = new System.Drawing.Size(316, 22);
            this.allTimeToolStripMenuItem.Text = "All Time";
            this.allTimeToolStripMenuItem.Click += new System.EventHandler(this.allTimeToolStripMenuItem_Click);
            // 
            // selectedMonth1YearAgoJobkeeperToolStripMenuItem
            // 
            this.selectedMonth1YearAgoJobkeeperToolStripMenuItem.Name = "selectedMonth1YearAgoJobkeeperToolStripMenuItem";
            this.selectedMonth1YearAgoJobkeeperToolStripMenuItem.Size = new System.Drawing.Size(316, 22);
            this.selectedMonth1YearAgoJobkeeperToolStripMenuItem.Text = "Selected month 1 year ago (Jobkeeper)";
            this.selectedMonth1YearAgoJobkeeperToolStripMenuItem.Click += new System.EventHandler(this.selectedMonth1YearAgoJobkeeperToolStripMenuItem_Click);
            // 
            // penaltiesToolStripMenuItem
            // 
            this.penaltiesToolStripMenuItem.Name = "penaltiesToolStripMenuItem";
            this.penaltiesToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.penaltiesToolStripMenuItem.Text = "Penalties";
            this.penaltiesToolStripMenuItem.Visible = false;
            this.penaltiesToolStripMenuItem.Click += new System.EventHandler(this.penaltiesToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.quitToolStripMenuItem.Text = "Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // testingToolStripMenuItem
            // 
            this.testingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.databaseToolStripMenuItem,
            this.databaseReadToolStripMenuItem,
            this.googleCalendarToolStripMenuItem,
            this.createRecurringEventToolStripMenuItem});
            this.testingToolStripMenuItem.Name = "testingToolStripMenuItem";
            this.testingToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.testingToolStripMenuItem.Text = "Testing";
            this.testingToolStripMenuItem.Visible = false;
            // 
            // databaseToolStripMenuItem
            // 
            this.databaseToolStripMenuItem.Name = "databaseToolStripMenuItem";
            this.databaseToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.databaseToolStripMenuItem.Text = "Database write";
            this.databaseToolStripMenuItem.Click += new System.EventHandler(this.databaseToolStripMenuItem_Click);
            // 
            // databaseReadToolStripMenuItem
            // 
            this.databaseReadToolStripMenuItem.Name = "databaseReadToolStripMenuItem";
            this.databaseReadToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.databaseReadToolStripMenuItem.Text = "Database read";
            this.databaseReadToolStripMenuItem.Click += new System.EventHandler(this.databaseReadToolStripMenuItem_Click);
            // 
            // googleCalendarToolStripMenuItem
            // 
            this.googleCalendarToolStripMenuItem.Name = "googleCalendarToolStripMenuItem";
            this.googleCalendarToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.googleCalendarToolStripMenuItem.Text = "Google Calendar";
            this.googleCalendarToolStripMenuItem.Click += new System.EventHandler(this.googleCalendarToolStripMenuItem_Click);
            // 
            // createRecurringEventToolStripMenuItem
            // 
            this.createRecurringEventToolStripMenuItem.Name = "createRecurringEventToolStripMenuItem";
            this.createRecurringEventToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.createRecurringEventToolStripMenuItem.Text = "Create Recurring Event";
            this.createRecurringEventToolStripMenuItem.Click += new System.EventHandler(this.createRecurringEventToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.searchFld);
            this.panel1.Controls.Add(this.refreshBtn);
            this.panel1.Controls.Add(this.showBookingsBtn);
            this.panel1.Controls.Add(this.switchMonitorBtn);
            this.panel1.Controls.Add(this.datePicker);
            this.panel1.Controls.Add(this.nextBtn);
            this.panel1.Controls.Add(this.prevBtn);
            this.panel1.Controls.Add(this.monthsLbl);
            this.panel1.Location = new System.Drawing.Point(0, 27);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1904, 100);
            this.panel1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1516, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 13);
            this.label1.TabIndex = 16;
            this.label1.Text = "Customer Search";
            // 
            // searchFld
            // 
            this.searchFld.Location = new System.Drawing.Point(1516, 76);
            this.searchFld.Name = "searchFld";
            this.searchFld.Size = new System.Drawing.Size(154, 20);
            this.searchFld.TabIndex = 15;
            this.searchFld.TextChanged += new System.EventHandler(this.searchFld_TextChanged);
            // 
            // refreshBtn
            // 
            this.refreshBtn.BackgroundImage = global::TBRBooker.FrontEnd.Properties.Resources.refresh;
            this.refreshBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.refreshBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.refreshBtn.Location = new System.Drawing.Point(1388, 9);
            this.refreshBtn.Name = "refreshBtn";
            this.refreshBtn.Size = new System.Drawing.Size(100, 87);
            this.refreshBtn.TabIndex = 14;
            this.refreshBtn.UseVisualStyleBackColor = true;
            this.refreshBtn.Click += new System.EventHandler(this.refreshBtn_Click);
            // 
            // showBookingsBtn
            // 
            this.showBookingsBtn.BackgroundImage = global::TBRBooker.FrontEnd.Properties.Resources.bookings;
            this.showBookingsBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.showBookingsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.showBookingsBtn.Location = new System.Drawing.Point(52, 9);
            this.showBookingsBtn.Name = "showBookingsBtn";
            this.showBookingsBtn.Size = new System.Drawing.Size(100, 87);
            this.showBookingsBtn.TabIndex = 5;
            this.showBookingsBtn.UseVisualStyleBackColor = true;
            this.showBookingsBtn.Click += new System.EventHandler(this.showBookingsBtn_Click);
            // 
            // switchMonitorBtn
            // 
            this.switchMonitorBtn.BackgroundImage = global::TBRBooker.FrontEnd.Properties.Resources.switch_monitors;
            this.switchMonitorBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.switchMonitorBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.switchMonitorBtn.Location = new System.Drawing.Point(208, 9);
            this.switchMonitorBtn.Name = "switchMonitorBtn";
            this.switchMonitorBtn.Size = new System.Drawing.Size(194, 87);
            this.switchMonitorBtn.TabIndex = 4;
            this.switchMonitorBtn.UseVisualStyleBackColor = true;
            this.switchMonitorBtn.Click += new System.EventHandler(this.switchMonitorBtn_Click);
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
            this.daysPanel.Location = new System.Drawing.Point(0, 144);
            this.daysPanel.Name = "daysPanel";
            this.daysPanel.Size = new System.Drawing.Size(1510, 861);
            this.daysPanel.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.label6.Location = new System.Drawing.Point(71, 130);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(73, 16);
            this.label6.TabIndex = 7;
            this.label6.Text = "MONDAY";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.label8.Location = new System.Drawing.Point(280, 130);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(80, 16);
            this.label8.TabIndex = 8;
            this.label8.Text = "TUESDAY";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.label9.Location = new System.Drawing.Point(486, 130);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(105, 16);
            this.label9.TabIndex = 9;
            this.label9.Text = "WEDNESDAY";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.label10.Location = new System.Drawing.Point(714, 130);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(92, 16);
            this.label10.TabIndex = 10;
            this.label10.Text = "THURSDAY";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.label11.Location = new System.Drawing.Point(940, 130);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(63, 16);
            this.label11.TabIndex = 11;
            this.label11.Text = "FRIDAY";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.label12.Location = new System.Drawing.Point(1139, 130);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(91, 16);
            this.label12.TabIndex = 12;
            this.label12.Text = "SATURDAY";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.label13.Location = new System.Drawing.Point(1361, 130);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(71, 16);
            this.label13.TabIndex = 13;
            this.label13.Text = "SUNDAY";
            // 
            // dashboardPnl
            // 
            this.dashboardPnl.Location = new System.Drawing.Point(1516, 144);
            this.dashboardPnl.Name = "dashboardPnl";
            this.dashboardPnl.Size = new System.Drawing.Size(376, 861);
            this.dashboardPnl.TabIndex = 14;
            // 
            // searchPnl
            // 
            this.searchPnl.BackColor = System.Drawing.SystemColors.Control;
            this.searchPnl.Controls.Add(this.bookingLst);
            this.searchPnl.Controls.Add(this.searcCloseBtn);
            this.searchPnl.Controls.Add(this.searchLst);
            this.searchPnl.Location = new System.Drawing.Point(1515, 129);
            this.searchPnl.Name = "searchPnl";
            this.searchPnl.Size = new System.Drawing.Size(389, 345);
            this.searchPnl.TabIndex = 17;
            this.searchPnl.Visible = false;
            // 
            // bookingLst
            // 
            this.bookingLst.Location = new System.Drawing.Point(8, 191);
            this.bookingLst.MultiSelect = false;
            this.bookingLst.Name = "bookingLst";
            this.bookingLst.Size = new System.Drawing.Size(369, 151);
            this.bookingLst.TabIndex = 2;
            this.bookingLst.UseCompatibleStateImageBehavior = false;
            this.bookingLst.View = System.Windows.Forms.View.List;
            this.bookingLst.ItemActivate += new System.EventHandler(this.bookingLst_ItemActivate);
            // 
            // searcCloseBtn
            // 
            this.searcCloseBtn.Location = new System.Drawing.Point(329, 4);
            this.searcCloseBtn.Name = "searcCloseBtn";
            this.searcCloseBtn.Size = new System.Drawing.Size(48, 23);
            this.searcCloseBtn.TabIndex = 1;
            this.searcCloseBtn.Text = "X";
            this.searcCloseBtn.UseVisualStyleBackColor = true;
            this.searcCloseBtn.Click += new System.EventHandler(this.searcCloseBtn_Click);
            // 
            // searchLst
            // 
            this.searchLst.Location = new System.Drawing.Point(8, 34);
            this.searchLst.MultiSelect = false;
            this.searchLst.Name = "searchLst";
            this.searchLst.Size = new System.Drawing.Size(369, 151);
            this.searchLst.TabIndex = 0;
            this.searchLst.UseCompatibleStateImageBehavior = false;
            this.searchLst.View = System.Windows.Forms.View.List;
            this.searchLst.ItemActivate += new System.EventHandler(this.searchLst_ItemActivate);
            // 
            // dateTmr
            // 
            this.dateTmr.Interval = 3000;
            this.dateTmr.Tick += new System.EventHandler(this.dateTmr_Tick);
            // 
            // travelLogLastFYearToolStripMenuItem
            // 
            this.travelLogLastFYearToolStripMenuItem.Name = "travelLogLastFYearToolStripMenuItem";
            this.travelLogLastFYearToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
            this.travelLogLastFYearToolStripMenuItem.Text = "Travel Log last f year";
            this.travelLogLastFYearToolStripMenuItem.Click += new System.EventHandler(this.travelLogLastFYearToolStripMenuItem_Click);
            // 
            // MainFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(168)))), ((int)(((byte)(239)))));
            this.ClientSize = new System.Drawing.Size(1904, 1001);
            this.Controls.Add(this.searchPnl);
            this.Controls.Add(this.dashboardPnl);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.daysPanel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.mainMnu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mainMnu;
            this.Name = "MainFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "TBR Booker";
            this.Load += new System.EventHandler(this.MainFrm_Load);
            this.mainMnu.ResumeLayout(false);
            this.mainMnu.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.searchPnl.ResumeLayout(false);
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
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.Button switchMonitorBtn;
        private System.Windows.Forms.Button showBookingsBtn;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button refreshBtn;
        private System.Windows.Forms.Panel dashboardPnl;
        private System.Windows.Forms.ToolStripMenuItem googleCalendarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reportsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generalSummaryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem last30DaysToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem currentFinancialYearToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem previousFinancialYearToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allTimeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allGeneralSummariesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createRecurringEventToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox searchFld;
        private System.Windows.Forms.Panel searchPnl;
        private System.Windows.Forms.Button searcCloseBtn;
        private System.Windows.Forms.ListView searchLst;
        private System.Windows.Forms.ListView bookingLst;
        private System.Windows.Forms.ToolStripMenuItem penaltiesToolStripMenuItem;
        private System.Windows.Forms.Timer dateTmr;
        private System.Windows.Forms.ToolStripMenuItem last12MonthsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectedMonth1YearAgoJobkeeperToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem travelLogLastFYearToolStripMenuItem;
    }
}

