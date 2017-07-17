using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FrontEnd
{
    public partial class MainFrm : Form
    {

        private DayPanel[,] _days;

        public MainFrm()
        {
            InitializeComponent();
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {
            AddDayPanels();
        }

        private void AddDayPanels()
        {
            _days = new DayPanel[4,7];
            for (int i = 0; i <= 3; i++)
            {
                for (int j = 0; j <= 6; j++)
                {
                    var day = new DayPanel();
                    daysPanel.Controls.Add(day);
                    day.Location = new Point(j * (day.Size.Height + 5) + 5, i * (day.Size.Width + 5) + 5);
                    _days[i, j] = day;
                }
            }
        }
    }
}
