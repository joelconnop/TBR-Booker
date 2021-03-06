﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TBRBooker.FrontEnd
{
    public partial class InputDialog : Form
    {

        public string InputValue { get; set; }
        private readonly bool _isNullOk;
        private readonly ValidatingTextbox _validator;

        public InputDialog(string description, string initialValue, bool isNullOk,
            ValidatingTextbox.TextBoxValidationType validationType = ValidatingTextbox.TextBoxValidationType.NotSet,
            bool isPassword = false)
        {
            InitializeComponent();

            Styles.SetFormStyles(this);
            _isNullOk = isNullOk;
            if (validationType != ValidatingTextbox.TextBoxValidationType.NotSet)
                _validator = new ValidatingTextbox(this, inputFld, validationType);
            descFld.Text = description;
            if (isPassword)
            {
                inputFld.ForeColor = inputFld.BackColor;
            }
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            SaveAndClose();
        }

        private void SaveAndClose()
        {
            if (!_isNullOk && string.IsNullOrEmpty(inputFld.Text.Trim()))
            {
                MessageBox.Show(this, "This input is mandatory (or cancel).", "Input Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            InputValue = inputFld.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void inputFld_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
                SaveAndClose();
        }
    }
}
