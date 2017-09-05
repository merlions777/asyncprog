using SlowLibrary;
using System.ComponentModel;
using System.Windows;

namespace BGW.WPF
{
    public partial class MainWindow : Window
    {
        private BackgroundWorker backgroundWorker;

        public MainWindow()
        {
            InitializeComponent();
            InitializeBackgroundWorker();
        }

        //Intialize the back ground worker.
        private void InitializeBackgroundWorker()
        {
            backgroundWorker = new BackgroundWorker();
            
            //Background Process
            backgroundWorker.DoWork += backgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;

            //Progress reporting
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.ProgressChanged += backgroundWorker_ProgressChanged;

            //Cancellation
            backgroundWorker.WorkerSupportsCancellation = true;
        }

        void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            MainProgressBar.Value = e.ProgressPercentage;
            OutputTextBox.Text = e.UserState.ToString();
        }

        // Runs on UI Thread
        void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //If there is any error message. Then show the message in the text box
            // stack trace in the message box.
            if (e.Error != null)
            {
                OutputTextBox.Text = e.Error.Message;
                MessageBox.Show(e.Error.StackTrace);
            }
            else if (e.Cancelled) // Event is cancelled.
            {
                OutputTextBox.Text = "Cancelled";
            }
            else
            {
                OutputTextBox.Text = e.Result.ToString();
                MainProgressBar.Value = 0;
            }
            
            StartButton.IsEnabled = !backgroundWorker.IsBusy;
            CancelButton.IsEnabled = backgroundWorker.IsBusy;

        }

        // Runs on Background worker thread
        void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;

            int result = 0;
            int iterations = (int)e.Argument;

            SlowProcessor processor = new SlowProcessor((int)e.Argument);

            foreach (var current in processor)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }
                if (worker.WorkerReportsProgress)
                {
                    int percentageCompletion = (int)((float)current / (float)iterations * 100);
                    string progressMessage = string.Format("Iteration {0} of {1}", current, iterations);

                    // Send the percentage completion and progress message.
                    worker.ReportProgress(percentageCompletion, progressMessage);
                }

                result = current;
            }

            e.Result = result;
        }


        /// <summary>
        /// Runs on the UI Thread
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            int iterations = 0;

            if (int.TryParse(InputTextBox.Text, out iterations))
            {
                backgroundWorker.RunWorkerAsync(iterations);
            }

            StartButton.IsEnabled = !backgroundWorker.IsBusy;
            CancelButton.IsEnabled = backgroundWorker.IsBusy;
            OutputTextBox.Text = string.Empty;
        }

        //runs on the UI thread
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            backgroundWorker.CancelAsync();
        }

    }
}
