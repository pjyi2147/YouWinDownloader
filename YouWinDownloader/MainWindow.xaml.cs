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

        // BackgroundWorker Methods ///////////////////////////////////////////////////////////////////////////////////
        // DoWork Method 
        private void downloadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            Process process = new Process();
            string[] scripts = (string[])e.Argument;
            string scriptText = scripts[1];
            string dir = scripts[0];
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.WorkingDirectory = dir;
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
                    foreach (var youtube in Process.GetProcessesByName("youtube-dl"))
                    {
                        youtube.Kill();
                    }
                    foreach (var ffmpeg in Process.GetProcessesByName("ffmpeg"))
                    {
                        ffmpeg.Kill();
                    }
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

        // ProgressChanged Method
        private void downloadWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            CMDoutputTextBox.Text = e.UserState.ToString();
            downloadBtn.Content = "Abort";
        }

        // When finished (RunWorkerCompleted Method)
        private void downloadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MessageBox.Show("Download Aborted!\r\nYou might need to delete .part file manually in the download folder.", "Abort");
            }
            else if (musicCheckBox.IsChecked == true)
            {
                MessageBox.Show("Music Download finished.", "Successful");
            }
            else if (videoCheckBox.IsChecked == true)
            {
                MessageBox.Show("Video Download finished.", "Successful");
            }
            CMDoutputTextBox.Text = "";
            downloadBtn.Content = "Download!";
            downloadBtn.IsEnabled = true;
        }
        
        // Event Handlers //////////////////////////////////////////////////////////////////////////////////////////////
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
                urlTextBox.IsEnabled = false;
                validateBtn.IsEnabled = false;
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
            MessageBox.Show("Music always downloads to the best quality possible.", "Notice");
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
            MessageBox.Show("If you choose .avi or .webm as the video format, it may take time depending on video size and your computer power." +
                "\r\nWhen it is finished, it will show up a messagebox that says it is finished. \r\nSo please allow upto an hour to finish or just abort and choose best file option.",
                "Notice");
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
                clearBtn_Click(sender, e);
            }
            else
            {
                string[] scripts = new string[2];

                // string scriptText = "cd " + fileLocationLabel.Text.ToString() + "&" + "youtube-dl " + urlTextBox.Text.ToString();
                string scriptText = ScriptOptionVaildator();

                scripts[0] = fileLocationLabel.Text.ToString();
                scripts[1] = scriptText;
                scriptTextBox.Text = scriptText;
                if (musicCheckBox.IsChecked == true || videoCheckBox.IsChecked == true)
                {
                    MessageBox.Show("Download Started!", "Started");
                    downloadWorker.RunWorkerAsync(scripts);
                }
                else
                {
                    MessageBox.Show("Please choose one of the options", "Error");
                }
            }
        }
        
        // Other Functions ////////////////////////////////////////////////////////////////////////////////////////////
        // but needed functions

        // url vaildator
        private void UrlVaildator(string url)
        {

        }

        // ScriptOptionValidator
        // Sees the user's option and makes the script to download the exact video
        private string ScriptOptionVaildator()
        {
            string scriptText = "youtube-dl " + urlTextBox.Text.ToString();

            if (musicCheckBox.IsChecked == true)
            {
                scriptText += " -x";
                if (musicAACRadioButton.IsChecked == true)
                {
                    scriptText += " -f bestaudio[ext=m4a]/bestaudio/best --audio-format m4a";
                }
                else if (musicMp3RadioButton.IsChecked == true)
                {
                    scriptText += " -f bestaudio[ext=mp3]/bestaudio/best --audio-format mp3";
                }
                else if (musicOpusRadioButton.IsChecked == true)
                {
                    scriptText += " -f bestaudio[ext=opus]/bestaudio/best --audio-format opus";
                }
                else
                {
                    scriptText += " -f bestaudio[ext=mp3]/bestaudio/best --audio-format mp3";
                }
                scriptText += " 0";
            }
            else if (videoCheckBox.IsChecked == true)
            {
                if (videoMkvRadioButton.IsChecked == true)
                {
                    scriptText += " -f bestvideo[ext=webm]+bestaudio[ext=opus]/bestvideo+bestaudio/best --recode-video mkv";
                }
                else if (videoMp4RadioButton.IsChecked == true)
                {
                    scriptText += " -f bestvideo[ext=mp4]+bestaudio[ext=m4a]/bestvideo+bestaudio/best --recode-video mp4";
                }
                else if (videoWebmRadioButton.IsChecked == true)
                {
                    scriptText += " -f bestvideo[ext=webm]+bestaudio[ext=opus]/bestvideo+bestaudio/best --recode-video webm";
                }
                else if (videoAviRadioButton.IsChecked == true)
                {
                    scriptText += " --recode-video avi";
                }
            }
            scriptText += " --hls-prefer-native";
            return scriptText;
        }
    }
}
