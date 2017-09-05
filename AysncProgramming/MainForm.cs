using System;
using System.ComponentModel;
using System.Windows.Forms;
using SlowLibrary;

namespace AysncProgramming
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        //runs on UI Thread
        private void StartButton_Click(object sender, EventArgs e)
        {
            int iterations = 0;

            if (int.TryParse(InputTextBox.Text, out iterations))
            {
                backgroundWorker.RunWorkerAsync(iterations);
            }
            StartButton.Enabled = !backgroundWorker.IsBusy;
            CancelButton.Enabled = backgroundWorker.IsBusy;
            OutputTextBox.Text = string.Empty;
        }

        //runs on UI Thread
        private void CancelButton_Click(object sender, EventArgs e)
        {
            // TODO: Implement cancellation process
        }

        //Runs on the background thread
        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            int iterations = (int)e.Argument;
            int result = 0;

            SlowProcessor processor = new SlowProcessor(iterations);
            foreach (var current in processor)
            {
                result = current;
            }

            e.Result = result;
        }

        //Runs the background worker thread
        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            OutputTextBox.Text = e.Result.ToString();
            StartButton.Enabled = !backgroundWorker.IsBusy;
            CancelButton.Enabled = backgroundWorker.IsBusy;
        }


    }
}
