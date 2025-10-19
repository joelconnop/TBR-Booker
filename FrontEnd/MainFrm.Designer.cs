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
            this.googleMapsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.googleOnMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.googleOffMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reportsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allGeneralSummariesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generalSummaryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.last30DaysToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.last12MonthsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.currentFinancialYearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.previousFinancialYearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.allTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectedMonth1YearAgoJobkeeperToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.travelLogLastFYearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.penaltiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.databaseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.databaseReadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.googleCalendarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createRecurringEventToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.calendarLbl = new System.Windows.Forms.Label();
            this.SavingPic = new System.Windows.Forms.PictureBox();
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
            this.mainMnu.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SavingPic)).BeginInit();
            this.searchPnl.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainMnu
            // 
            this.mainMnu.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.mainMnu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.mainMnu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.testingToolStripMenuItem});
            this.mainMnu.Location = new System.Drawing.Point(0, 0);
            this.mainMnu.Name = "mainMnu";
            this.mainMnu.Size = new System.Drawing.Size(2856, 33);
            this.mainMnu.TabIndex = 0;
            this.mainMnu.Text = "Main Menu Strip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.googleMapsToolStripMenuItem,
            this.reportsToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(54, 29);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // googleMapsToolStripMenuItem
            // 
            this.googleMapsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.googleOnMenuItem,
            this.googleOffMenuItem});
            this.googleMapsToolStripMenuItem.Name = "googleMapsToolStripMenuItem";
            this.googleMapsToolStripMenuItem.Size = new System.Drawing.Size(270, 34);
            this.googleMapsToolStripMenuItem.Text = "Google Integration";
            // 
            // googleOnMenuItem
            // 
            this.googleOnMenuItem.Checked = true;
            this.googleOnMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.googleOnMenuItem.Enabled = false;
            this.googleOnMenuItem.Name = "googleOnMenuItem";
            this.googleOnMenuItem.Size = new System.Drawing.Size(270, 34);
            this.googleOnMenuItem.Text = "On";
            this.googleOnMenuItem.Click += new System.EventHandler(this.googleOnMenuItem_Click);
            // 
            // googleOffMenuItem
            // 
            this.googleOffMenuItem.Name = "googleOffMenuItem";
            this.googleOffMenuItem.Size = new System.Drawing.Size(270, 34);
            this.googleOffMenuItem.Text = "Off";
            this.googleOffMenuItem.Click += new System.EventHandler(this.googleOffMenuItem_Click);
            // 
            // reportsToolStripMenuItem
            // 
            this.reportsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.allGeneralSummariesToolStripMenuItem,
            this.generalSummaryToolStripMenuItem,
            this.travelLogLastFYearToolStripMenuItem,
            this.penaltiesToolStripMenuItem});
            this.reportsToolStripMenuItem.Name = "reportsToolStripMenuItem";
            this.reportsToolStripMenuItem.Size = new System.Drawing.Size(270, 34);
            this.reportsToolStripMenuItem.Text = "Reports";
            // 
            // allGeneralSummariesToolStripMenuItem
            // 
            this.allGeneralSummariesToolStripMenuItem.Name = "allGeneralSummariesToolStripMenuItem";
            this.allGeneralSummariesToolStripMenuItem.Size = new System.Drawing.Size(291, 34);
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
            this.generalSummaryToolStripMenuItem.Size = new System.Drawing.Size(291, 34);
            this.generalSummaryToolStripMenuItem.Text = "General Summary";
            // 
            // last30DaysToolStripMenuItem
            // 
            this.last30DaysToolStripMenuItem.Name = "last30DaysToolStripMenuItem";
            this.last30DaysToolStripMenuItem.Size = new System.Drawing.Size(474, 34);
            this.last30DaysToolStripMenuItem.Text = "Last 30 Days (prior to selected date)";
            this.last30DaysToolStripMenuItem.Click += new System.EventHandler(this.last30DaysToolStripMenuItem_Click);
            // 
            // last12MonthsToolStripMenuItem
            // 
            this.last12MonthsToolStripMenuItem.Name = "last12MonthsToolStripMenuItem";
            this.last12MonthsToolStripMenuItem.Size = new System.Drawing.Size(474, 34);
            this.last12MonthsToolStripMenuItem.Text = "Last 12 Months (prior to selected date)";
            this.last12MonthsToolStripMenuItem.Click += new System.EventHandler(this.last12MonthsToolStripMenuItem_Click);
            // 
            // currentFinancialYearToolStripMenuItem
            // 
            this.currentFinancialYearToolStripMenuItem.Name = "currentFinancialYearToolStripMenuItem";
            this.currentFinancialYearToolStripMenuItem.Size = new System.Drawing.Size(474, 34);
            this.currentFinancialYearToolStripMenuItem.Text = "Current Financial Year (selected date)";
            this.currentFinancialYearToolStripMenuItem.Click += new System.EventHandler(this.currentFinancialYearToolStripMenuItem_Click);
            // 
            // previousFinancialYearToolStripMenuItem
            // 
            this.previousFinancialYearToolStripMenuItem.Name = "previousFinancialYearToolStripMenuItem";
            this.previousFinancialYearToolStripMenuItem.Size = new System.Drawing.Size(474, 34);
            this.previousFinancialYearToolStripMenuItem.Text = "Previous Financial Year (prior to selected date)";
            this.previousFinancialYearToolStripMenuItem.Click += new System.EventHandler(this.previousFinancialYearToolStripMenuItem_Click);
            // 
            // allTimeToolStripMenuItem
            // 
            this.allTimeToolStripMenuItem.Name = "allTimeToolStripMenuItem";
            this.allTimeToolStripMenuItem.Size = new System.Drawing.Size(474, 34);
            this.allTimeToolStripMenuItem.Text = "All Time";
            this.allTimeToolStripMenuItem.Click += new System.EventHandler(this.allTimeToolStripMenuItem_Click);
            // 
            // selectedMonth1YearAgoJobkeeperToolStripMenuItem
            // 
            this.selectedMonth1YearAgoJobkeeperToolStripMenuItem.Name = "selectedMonth1YearAgoJobkeeperToolStripMenuItem";
            this.selectedMonth1YearAgoJobkeeperToolStripMenuItem.Size = new System.Drawing.Size(474, 34);
            this.selectedMonth1YearAgoJobkeeperToolStripMenuItem.Text = "Selected month 1 year ago (Jobkeeper)";
            this.selectedMonth1YearAgoJobkeeperToolStripMenuItem.Click += new System.EventHandler(this.selectedMonth1YearAgoJobkeeperToolStripMenuItem_Click);
            // 
            // travelLogLastFYearToolStripMenuItem
            // 
            this.travelLogLastFYearToolStripMenuItem.Name = "travelLogLastFYearToolStripMenuItem";
            this.travelLogLastFYearToolStripMenuItem.Size = new System.Drawing.Size(291, 34);
            this.travelLogLastFYearToolStripMenuItem.Text = "Travel Log last f year";
            this.travelLogLastFYearToolStripMenuItem.Click += new System.EventHandler(this.travelLogLastFYearToolStripMenuItem_Click);
            // 
            // penaltiesToolStripMenuItem
            // 
            this.penaltiesToolStripMenuItem.Name = "penaltiesToolStripMenuItem";
            this.penaltiesToolStripMenuItem.Size = new System.Drawing.Size(291, 34);
            this.penaltiesToolStripMenuItem.Text = "Penalties";
            this.penaltiesToolStripMenuItem.Visible = false;
            this.penaltiesToolStripMenuItem.Click += new System.EventHandler(this.penaltiesToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(270, 34);
            this.settingsToolStripMenuItem.Text = "Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(270, 34);
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
            this.testingToolStripMenuItem.Size = new System.Drawing.Size(83, 29);
            this.testingToolStripMenuItem.Text = "Testing";
            this.testingToolStripMenuItem.Visible = false;
            // 
            // databaseToolStripMenuItem
            // 
            this.databaseToolStripMenuItem.Name = "databaseToolStripMenuItem";
            this.databaseToolStripMenuItem.Size = new System.Drawing.Size(291, 34);
            this.databaseToolStripMenuItem.Text = "Database write";
            this.databaseToolStripMenuItem.Click += new System.EventHandler(this.databaseToolStripMenuItem_Click);
            // 
            // databaseReadToolStripMenuItem
            // 
            this.databaseReadToolStripMenuItem.Name = "databaseReadToolStripMenuItem";
            this.databaseReadToolStripMenuItem.Size = new System.Drawing.Size(291, 34);
            this.databaseReadToolStripMenuItem.Text = "Database read";
            this.databaseReadToolStripMenuItem.Click += new System.EventHandler(this.databaseReadToolStripMenuItem_Click);
            // 
            // googleCalendarToolStripMenuItem
            // 
            this.googleCalendarToolStripMenuItem.Name = "googleCalendarToolStripMenuItem";
            this.googleCalendarToolStripMenuItem.Size = new System.Drawing.Size(291, 34);
            this.googleCalendarToolStripMenuItem.Text = "Google Calendar";
            this.googleCalendarToolStripMenuItem.Click += new System.EventHandler(this.googleCalendarToolStripMenuItem_Click);
            // 
            // createRecurringEventToolStripMenuItem
            // 
            this.createRecurringEventToolStripMenuItem.Name = "createRecurringEventToolStripMenuItem";
            this.createRecurringEventToolStripMenuItem.Size = new System.Drawing.Size(291, 34);
            this.createRecurringEventToolStripMenuItem.Text = "Create Recurring Event";
            this.createRecurringEventToolStripMenuItem.Click += new System.EventHandler(this.createRecurringEventToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.calendarLbl);
            this.panel1.Controls.Add(this.SavingPic);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.searchFld);
            this.panel1.Controls.Add(this.refreshBtn);
            this.panel1.Controls.Add(this.showBookingsBtn);
            this.panel1.Controls.Add(this.switchMonitorBtn);
            this.panel1.Controls.Add(this.datePicker);
            this.panel1.Controls.Add(this.nextBtn);
            this.panel1.Controls.Add(this.prevBtn);
            this.panel1.Controls.Add(this.monthsLbl);
            this.panel1.Location = new System.Drawing.Point(0, 42);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(2856, 154);
            this.panel1.TabIndex = 1;
            // 
            // calendarLbl
            // 
            this.calendarLbl.AutoSize = true;
            this.calendarLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.calendarLbl.Location = new System.Drawing.Point(2273, 42);
            this.calendarLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.calendarLbl.Name = "calendarLbl";
            this.calendarLbl.Size = new System.Drawing.Size(183, 25);
            this.calendarLbl.TabIndex = 18;
            this.calendarLbl.Text = "Loading Bookings...";
            this.calendarLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SavingPic
            // 
            this.SavingPic.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.SavingPic.Cursor = System.Windows.Forms.Cursors.Default;
            this.SavingPic.InitialImage = null;
            this.SavingPic.Location = new System.Drawing.Point(702, 15);
            this.SavingPic.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.SavingPic.Name = "SavingPic";
            this.SavingPic.Size = new System.Drawing.Size(125, 133);
            this.SavingPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.SavingPic.TabIndex = 17;
            this.SavingPic.TabStop = false;
            this.SavingPic.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2274, 88);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 20);
            this.label1.TabIndex = 16;
            this.label1.Text = "Customer Search";
            // 
            // searchFld
            // 
            this.searchFld.Location = new System.Drawing.Point(2274, 117);
            this.searchFld.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.searchFld.Name = "searchFld";
            this.searchFld.Size = new System.Drawing.Size(229, 26);
            this.searchFld.TabIndex = 15;
            this.searchFld.TextChanged += new System.EventHandler(this.searchFld_TextChanged);
            // 
            // refreshBtn
            // 
            this.refreshBtn.BackgroundImage = global::TBRBooker.FrontEnd.Properties.Resources.refresh;
            this.refreshBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.refreshBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.refreshBtn.Location = new System.Drawing.Point(2082, 14);
            this.refreshBtn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.refreshBtn.Name = "refreshBtn";
            this.refreshBtn.Size = new System.Drawing.Size(150, 134);
            this.refreshBtn.TabIndex = 14;
            this.refreshBtn.UseVisualStyleBackColor = true;
            this.refreshBtn.Click += new System.EventHandler(this.refreshBtn_Click);
            // 
            // showBookingsBtn
            // 
            this.showBookingsBtn.BackgroundImage = global::TBRBooker.FrontEnd.Properties.Resources.bookings;
            this.showBookingsBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.showBookingsBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.showBookingsBtn.Location = new System.Drawing.Point(78, 14);
            this.showBookingsBtn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.showBookingsBtn.Name = "showBookingsBtn";
            this.showBookingsBtn.Size = new System.Drawing.Size(150, 134);
            this.showBookingsBtn.TabIndex = 5;
            this.showBookingsBtn.UseVisualStyleBackColor = true;
            this.showBookingsBtn.Click += new System.EventHandler(this.showBookingsBtn_Click);
            // 
            // switchMonitorBtn
            // 
            this.switchMonitorBtn.BackgroundImage = global::TBRBooker.FrontEnd.Properties.Resources.switch_monitors;
            this.switchMonitorBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.switchMonitorBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.switchMonitorBtn.Location = new System.Drawing.Point(312, 14);
            this.switchMonitorBtn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.switchMonitorBtn.Name = "switchMonitorBtn";
            this.switchMonitorBtn.Size = new System.Drawing.Size(291, 134);
            this.switchMonitorBtn.TabIndex = 4;
            this.switchMonitorBtn.UseVisualStyleBackColor = true;
            this.switchMonitorBtn.Click += new System.EventHandler(this.switchMonitorBtn_Click);
            // 
            // datePicker
            // 
            this.datePicker.Location = new System.Drawing.Point(1296, 14);
            this.datePicker.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.datePicker.Name = "datePicker";
            this.datePicker.Size = new System.Drawing.Size(298, 26);
            this.datePicker.TabIndex = 3;
            this.datePicker.ValueChanged += new System.EventHandler(this.datePicker_ValueChanged);
            // 
            // nextBtn
            // 
            this.nextBtn.BackgroundImage = global::TBRBooker.FrontEnd.Properties.Resources.next;
            this.nextBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.nextBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.nextBtn.Location = new System.Drawing.Point(1890, 42);
            this.nextBtn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.nextBtn.Name = "nextBtn";
            this.nextBtn.Size = new System.Drawing.Size(90, 92);
            this.nextBtn.TabIndex = 2;
            this.nextBtn.UseVisualStyleBackColor = true;
            this.nextBtn.Click += new System.EventHandler(this.nextBtn_Click);
            // 
            // prevBtn
            // 
            this.prevBtn.BackgroundImage = global::TBRBooker.FrontEnd.Properties.Resources.back;
            this.prevBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.prevBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.prevBtn.Location = new System.Drawing.Point(922, 42);
            this.prevBtn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.prevBtn.Name = "prevBtn";
            this.prevBtn.Size = new System.Drawing.Size(90, 92);
            this.prevBtn.TabIndex = 1;
            this.prevBtn.UseVisualStyleBackColor = true;
            this.prevBtn.Click += new System.EventHandler(this.prevBtn_Click);
            // 
            // monthsLbl
            // 
            this.monthsLbl.AutoSize = true;
            this.monthsLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.monthsLbl.Location = new System.Drawing.Point(1140, 49);
            this.monthsLbl.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.monthsLbl.Name = "monthsLbl";
            this.monthsLbl.Size = new System.Drawing.Size(629, 82);
            this.monthsLbl.TabIndex = 0;
            this.monthsLbl.Text = "July 17- August 17";
            this.monthsLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // daysPanel
            // 
            this.daysPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(168)))), ((int)(((byte)(239)))));
            this.daysPanel.Location = new System.Drawing.Point(0, 222);
            this.daysPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.daysPanel.Name = "daysPanel";
            this.daysPanel.Size = new System.Drawing.Size(2265, 1325);
            this.daysPanel.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.label6.Location = new System.Drawing.Point(106, 200);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(106, 25);
            this.label6.TabIndex = 7;
            this.label6.Text = "MONDAY";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.label8.Location = new System.Drawing.Point(420, 200);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(114, 25);
            this.label8.TabIndex = 8;
            this.label8.Text = "TUESDAY";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.label9.Location = new System.Drawing.Point(729, 200);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(150, 25);
            this.label9.TabIndex = 9;
            this.label9.Text = "WEDNESDAY";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.label10.Location = new System.Drawing.Point(1071, 200);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(129, 25);
            this.label10.TabIndex = 10;
            this.label10.Text = "THURSDAY";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.label11.Location = new System.Drawing.Point(1410, 200);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(89, 25);
            this.label11.TabIndex = 11;
            this.label11.Text = "FRIDAY";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.label12.Location = new System.Drawing.Point(1708, 200);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(129, 25);
            this.label12.TabIndex = 12;
            this.label12.Text = "SATURDAY";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.label13.Location = new System.Drawing.Point(2042, 200);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(101, 25);
            this.label13.TabIndex = 13;
            this.label13.Text = "SUNDAY";
            // 
            // dashboardPnl
            // 
            this.dashboardPnl.Location = new System.Drawing.Point(2274, 222);
            this.dashboardPnl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.dashboardPnl.Name = "dashboardPnl";
            this.dashboardPnl.Size = new System.Drawing.Size(564, 1325);
            this.dashboardPnl.TabIndex = 14;
            // 
            // searchPnl
            // 
            this.searchPnl.BackColor = System.Drawing.SystemColors.Control;
            this.searchPnl.Controls.Add(this.bookingLst);
            this.searchPnl.Controls.Add(this.searcCloseBtn);
            this.searchPnl.Controls.Add(this.searchLst);
            this.searchPnl.Location = new System.Drawing.Point(2272, 198);
            this.searchPnl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.searchPnl.Name = "searchPnl";
            this.searchPnl.Size = new System.Drawing.Size(584, 531);
            this.searchPnl.TabIndex = 17;
            this.searchPnl.Visible = false;
            // 
            // bookingLst
            // 
            this.bookingLst.HideSelection = false;
            this.bookingLst.Location = new System.Drawing.Point(12, 294);
            this.bookingLst.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.bookingLst.MultiSelect = false;
            this.bookingLst.Name = "bookingLst";
            this.bookingLst.Size = new System.Drawing.Size(552, 230);
            this.bookingLst.TabIndex = 2;
            this.bookingLst.UseCompatibleStateImageBehavior = false;
            this.bookingLst.View = System.Windows.Forms.View.List;
            this.bookingLst.ItemActivate += new System.EventHandler(this.bookingLst_ItemActivate);
            // 
            // searcCloseBtn
            // 
            this.searcCloseBtn.Location = new System.Drawing.Point(494, 6);
            this.searcCloseBtn.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.searcCloseBtn.Name = "searcCloseBtn";
            this.searcCloseBtn.Size = new System.Drawing.Size(72, 35);
            this.searcCloseBtn.TabIndex = 1;
            this.searcCloseBtn.Text = "X";
            this.searcCloseBtn.UseVisualStyleBackColor = true;
            this.searcCloseBtn.Click += new System.EventHandler(this.searcCloseBtn_Click);
            // 
            // searchLst
            // 
            this.searchLst.HideSelection = false;
            this.searchLst.Location = new System.Drawing.Point(12, 52);
            this.searchLst.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.searchLst.MultiSelect = false;
            this.searchLst.Name = "searchLst";
            this.searchLst.Size = new System.Drawing.Size(552, 230);
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
            // MainFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(168)))), ((int)(((byte)(239)))));
            this.ClientSize = new System.Drawing.Size(2856, 1540);
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
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MainFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "TBR Booker";
            this.Load += new System.EventHandler(this.MainFrm_Load);
            this.mainMnu.ResumeLayout(false);
            this.mainMnu.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SavingPic)).EndInit();
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
        private System.Windows.Forms.PictureBox SavingPic;
        private System.Windows.Forms.ToolStripMenuItem googleMapsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem googleOnMenuItem;
        private System.Windows.Forms.ToolStripMenuItem googleOffMenuItem;
        private System.Windows.Forms.Label calendarLbl;
    }
}

