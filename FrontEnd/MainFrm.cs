using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TBRBooker.Business;
using TBRBooker.Model.Enums;
using TBRBooker.Model.Entities;

namespace TBRBooker.FrontEnd
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
                    var day = new DayPanel(i*7 + j + 1);
                    daysPanel.Controls.Add(day);
                    day.Location = new Point(j * (day.Size.Height + 5) + 5, i * (day.Size.Width + 5) + 5);
                    _days[i, j] = day;
                }
            }
        }

        private void databaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //var customer = new Customer()
            //{
            //    FirstName = "Joeltest",
            //    LastName = "Conntest",
            //    MobileNumber = "0412345678",
            //    OtherNumbers = "",
            //    EmailAddress = "joel.connop@gmail.com",
            //    CreatedDate = DateTime.Now,
            //    CompanyName = "Happy Testers"
            //};
            //DBBox.WriteItem(customer);

            
        }

        private void databaseReadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var customer = DBBox.ReadItem<Customer>("89f5e12e-8e37-49d7-84ee-becb1e216761");
            MessageBox.Show(customer.SmartName());
        }
    }
}
