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

        public SettingsManagementFrm(Settings settings, bool canCancel)
        {
            InitializeComponent();
            _settings = settings;
            if (!canCancel)
                closeBtn.Enabled = false;
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
                MessageBox.Show("A valid path to Sarah Jane's Google Drive (TBR area) is needed. Example: G:\\My Drive\\Bookings\\TBR Booker");
            }
            
            if (newSettings.Username != null && (_settings.Username == null 
                || !_settings.Username.Equals(newSettings.Username)))
                Registry.SetValue(Settings.EnvironmentVarsRoot, Settings.UserKey, newSettings.Username,
                    RegistryValueKind.String);
            
            if (!_settings.WorkingDir.Equals(newSettings.WorkingDir))
                Registry.SetValue(Settings.EnvironmentVarsRoot, Settings.WorkingDirKey, newSettings.WorkingDir,
                    RegistryValueKind.String);

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
            Close();
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
