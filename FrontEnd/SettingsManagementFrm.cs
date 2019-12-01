using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TBRBooker.Base;
using TBRBooker.Business;

namespace TBRBooker.FrontEnd
{
    public partial class SettingsManagementFrm : Form
    {

        private Settings _settings;
        private readonly bool _initConfigNeeded;

        public SettingsManagementFrm(Settings settings, bool initConfigNeeded)
        {
            InitializeComponent();
            _settings = settings;
            _initConfigNeeded = initConfigNeeded;
        }

        private void SettingsManagementFrm_Load(object sender, EventArgs e)
        {
            SettingsGrid.SelectedObject = (Settings)_settings.Clone();
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            try
            {
                var newSettings = (Settings)SettingsGrid.SelectedObject;

                Settings.PersistSettings(newSettings, _settings, false);

                Settings.SetInst(newSettings);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleError(this, "saving settings", ex);
            }
            
            DialogResult = DialogResult.OK;
            Close();
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            if (_initConfigNeeded && MessageBox.Show(this,
                "Initial Configuration is required to go any further. Would you like to set this up now?"
                 + Environment.NewLine + Environment.NewLine
                 + "- Choose YES to stay here and setup your username and working directory."
                 + Environment.NewLine + "- Choose NO to exit the program for now (check your file system and/or registry entries)."
                 + Environment.NewLine + Environment.NewLine + "HINT: If using Google Drive Stream, and it is not running, choose NO, and then startup Google Drive Stream before trying again.",
                "TBR Booker Startup Failed", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                        == DialogResult.Yes)
                return;

            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
