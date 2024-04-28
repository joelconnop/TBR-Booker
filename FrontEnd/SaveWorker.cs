using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TBRBooker.Business;
using TBRBooker.Model.Entities;

namespace TBRBooker.FrontEnd
{
    public class SaveWorker
    {

        public BackgroundWorker Worker = new BackgroundWorker();
        PictureBox _savingBox;

        public void InitializeBackgroundWorker(PictureBox savingBox)
        {
            _savingBox = savingBox;
            _savingBox.Image = Properties.Resources.saving;
            Worker.DoWork += new DoWorkEventHandler(bgWorker_DoWork);
            Worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bgWorker_RunWorkerCompleted);
            Worker.ProgressChanged += new ProgressChangedEventHandler(bgWorker_ProgressChanged);
            Worker.WorkerReportsProgress = true;
            Worker.WorkerSupportsCancellation = true;
        }

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            SetPictureBoxVisibility(true);
            var booking = e.Argument as Booking;
            // Perform the save operation here
            BookingBL.SaveBookingEtc(booking);
            // You can report progress if needed
            Worker.ReportProgress(50);
        }

        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            SetPictureBoxVisibility(false);
            //if (e.Cancelled)
            //{
            //    MessageBox.Show("Operation was canceled.");
            //}
            //else if (e.Error != null)
            //{
            //    MessageBox.Show("Error occurred: " + e.Error.Message);
            //}
            //else
            //{
            //    MessageBox.Show("Booking saved successfully!");
            //}
        }

        private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // progressBar.Value = e.ProgressPercentage; // Update progress bar if needed
        }

        private void SetPictureBoxVisibility(bool visible)
        {
            if (_savingBox.InvokeRequired)
            {
                _savingBox.Invoke(new Action(() => SetPictureBoxVisibility(visible)));
            }
            else
            {
                _savingBox.Visible = visible;
            }
        }
    }
}
