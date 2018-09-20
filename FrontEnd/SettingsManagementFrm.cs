using Microsoft.Win32;
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
            var newSettings = (Settings)SettingsGrid.SelectedObject;

            if (string.IsNullOrEmpty(newSettings.Username))
            {
                MessageBox.Show("A Username is required to go any further.");
                return;
            }

            if (!Directory.Exists(newSettings.WorkingDir))
            {
                MessageBox.Show("A valid working directory is needed (preferrably a Google Drive Stream location) is needed. Example: G:\\My Drive\\Bookings\\TBR Booker");
            }

            // don't touch registry entries in test or we will mess with production config
            if (!Settings.IsForcedToTestMode())
            {
                if (newSettings.Username != null && (_settings.Username == null
                || !_settings.Username.Equals(newSettings.Username)))
                    Registry.SetValue(Settings.EnvironmentVarsRoot, Settings.UserKey, newSettings.Username,
                        RegistryValueKind.String);
                if (!_settings.WorkingDir.Equals(newSettings.WorkingDir))
                    Registry.SetValue(Settings.EnvironmentVarsRoot, Settings.WorkingDirKey, newSettings.WorkingDir,
                        RegistryValueKind.String);
            }

            var settingsFilename = newSettings.WorkingDir + "\\config\\" + newSettings.Username + "_settings.json";
            try
            {
                using (StreamWriter file = File.CreateText(settingsFilename))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, newSettings);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to write the settings to " + settingsFilename + " because: " + ex.Message);
                return;
            }

            Settings.SetInst(newSettings);
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
