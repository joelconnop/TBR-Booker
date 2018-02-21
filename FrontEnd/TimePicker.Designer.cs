namespace TBRBooker.FrontEnd
{
    partial class TimePicker
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
            this.picker = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // picker
            // 
            this.picker.DropDownHeight = 500;
            this.picker.FormattingEnabled = true;
            this.picker.IntegralHeight = false;
            this.picker.Location = new System.Drawing.Point(3, 3);
            this.picker.MaxDropDownItems = 50;
            this.picker.Name = "picker";
            this.picker.Size = new System.Drawing.Size(78, 21);
            this.picker.TabIndex = 0;
            this.picker.SelectedIndexChanged += new System.EventHandler(this.picker_SelectedIndexChanged);
            // 
            // TimePicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.picker);
            this.Name = "TimePicker";
            this.Size = new System.Drawing.Size(84, 27);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox picker;
    }
}
