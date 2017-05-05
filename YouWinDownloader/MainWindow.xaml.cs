using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Management;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace YouWinDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // backgroundworker variables
        BackgroundWorker downloadWorker;
        
        // Main Window
        public MainWindow()
        {
            InitializeComponent();

            // Backgroundworker init
            downloadWorker = new BackgroundWorker();
            downloadWorker.DoWork += new DoWorkEventHandler(downloadWorker_DoWork);
            downloadWorker.ProgressChanged += new ProgressChangedEventHandler(downloadWorker_ProgressChanged);
            downloadWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(downloadWorker_RunWorkerCompleted);
            downloadWorker.WorkerReportsProgress = true;
            downloadWorker.WorkerSupportsCancellation = true;
        }

        // Do Work!
        private void downloadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            Process process = new Process();
            string scriptText = e.Argument.ToString();
            process.StartInfo.FileName = "cmd.exe";
            // process.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
            process.StartInfo.Arguments = "/C set path=%path%;" + System.AppDomain.CurrentDomain.BaseDirectory + "&" + scriptText;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardError = true;

            process.Start();

            while (!process.StandardOutput.EndOfStream)
            {
                if (downloadWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                else
                {
                    string line = process.StandardOutput.ReadLine();
                    worker.ReportProgress(1, line);
                }
            }
        }

        // Progress changed!
        private void downloadWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            CMDoutputTextBox.Text = e.UserState.ToString();
            downloadBtn.Content = "Abort";
            downloadBtn.IsEnabled = false;
        }
        
        // Finished!
        private void downloadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show("Download Aborted!", "Abort");
            }
            else if (musicCheckBox.IsChecked == true)
            {
                MessageBox.Show("Music Download finished.", "Successful");
            }
            else if (videoCheckBox.IsChecked == true) 
            {
                MessageBox.Show("Video Download finished.", "Successful");
            }
            downloadBtn.Content = "Download!";
            downloadBtn.IsEnabled = true;
        }

        // urlTextBox methods
        // GotFocus
        private void urlTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            urlTextBox.Text = "";
        }

        // Lost focus
        private void urlTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (urlTextBox.Text == "")
            {
                urlTextBox.Text = "Full url including http://";
            }
        }

        // key input from urlTextBox
        private void urlTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                validateBtn_Click(sender, e);
            }
        }

        // ValidateBtn methods
        // validate button cilck (Update needed)
        private void validateBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Uri.IsWellFormedUriString(urlTextBox.Text, UriKind.Absolute))
            {
                MessageBox.Show("Website Validated!", "Validation Successful!");
                openFolderBtn.IsEnabled = true;
                musicCheckBox.IsEnabled = true;
                videoCheckBox.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Invaild format: Try Again", "Error");
            }
        }
        
        // music Check Box methods
        // checked
        private void musicCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (videoCheckBox.IsChecked == true)
            {
                videoCheckBox.IsChecked = false;
            }
            musicAACRadioButton.IsEnabled = true;
            musicMp3RadioButton.IsEnabled = true;
            musicOpusRadioButton.IsEnabled = true;
        }

        // unchecked
        private void musicCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // unable checkboxes
            musicAACRadioButton.IsEnabled = false;
            musicMp3RadioButton.IsEnabled = false;
            musicOpusRadioButton.IsEnabled = false;
            // uncheck them
            musicAACRadioButton.IsChecked = false;
            musicMp3RadioButton.IsChecked = false;
            musicOpusRadioButton.IsChecked = false;
        }

        // video Check Box 
        // checked
        private void videoCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (musicCheckBox.IsChecked == true)
            {
                musicCheckBox.IsChecked = false;
            }
            videoAviRadioButton.IsEnabled = true;
            videoMkvRadioButton.IsEnabled = true;
            videoMp4RadioButton.IsEnabled = true;
            videoWebmRadioButton.IsEnabled = true;
        }

        // unchecked
        private void videoCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // unable checkboxes
            videoAviRadioButton.IsEnabled = false;
            videoMkvRadioButton.IsEnabled = false;
            videoMp4RadioButton.IsEnabled = false;
            videoWebmRadioButton.IsEnabled = false;
           // no checks
            videoAviRadioButton.IsChecked = false;
            videoMkvRadioButton.IsChecked = false;
            videoMp4RadioButton.IsChecked = false;
            videoWebmRadioButton.IsChecked = false;
        }

        // OpenfolderBtn
        // folder opening
        private void openFolderBtn_Click(object sender, RoutedEventArgs e)
        {
            string folderLocation = "";
            System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog();
            fbd.RootFolder = System.Environment.SpecialFolder.MyComputer;
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                folderLocation = fbd.SelectedPath;
                fileLocationLabel.Text = folderLocation;
                downloadBtn.IsEnabled = true;
            }
            else
            {
                MessageBox.Show("Folder open failed!", "Error");
            }
        }

        // ClearBtn method
        // Click
        private void clearBtn_Click(object sender, RoutedEventArgs e)
        {
            urlTextBox.Text = "";
            fileLocationLabel.Text = "";
            CMDoutputTextBox.Text = "";

            musicAACRadioButton.IsChecked = false;
            musicMp3RadioButton.IsChecked = false;
            musicOpusRadioButton.IsChecked = false;
            videoAviRadioButton.IsChecked = false;
            videoMkvRadioButton.IsChecked = false;
            videoMp4RadioButton.IsChecked = false;
            musicCheckBox.IsChecked = false;
            videoCheckBox.IsChecked = false;

            musicCheckBox.IsEnabled = false;
            videoCheckBox.IsEnabled = false;
            openFolderBtn.IsEnabled = false;
            downloadBtn.IsEnabled = false;
        }

        // downloadBtn methods
        // Click
        private void downloadBtn_Click(object sender, RoutedEventArgs e)
        {
            if (downloadWorker.IsBusy)
            {
                downloadWorker.CancelAsync();
                // MessageBox.Show("Download Aborted!", "Abort");
                clearBtn_Click(sender, e);
            }
            else
            {
                string scriptText = "cd " + fileLocationLabel.Text.ToString() + "&" + "youtube-dl " + urlTextBox.Text.ToString();

                // a function for other stuff should be made here
                // then this if statement for music also be moved to there.
                if (musicCheckBox.IsChecked == true)
                {
                    scriptText += " -x";
                }

                if (musicCheckBox.IsChecked == true || videoCheckBox.IsChecked == true)
                {
                    MessageBox.Show("Download Started!", "Started");
                    downloadWorker.RunWorkerAsync(scriptText);
                }
                else
                {
                    MessageBox.Show("Please choose one of the options", "Error");
                }
            }
        }
    }
}
