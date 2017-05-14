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

            while (!process.StandardOutput.EndOfStream || !process.StandardError.EndOfStream)
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
                else if (!process.StandardOutput.EndOfStream)
                {
                    string line = process.StandardOutput.ReadLine();
                    worker.ReportProgress(1, line);
                }
                else if (!process.StandardError.EndOfStream)
                {
                    string error = process.StandardError.ReadLine();
                    worker.ReportProgress(1, error);
                    throw new Exception(error);
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
            if (e.Error != null)
            {
                MessageBox.Show("Error encountered!\r\n\r\n" + e.Error.Message, "Error");
            }
            else if (e.Cancelled)
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
                outputCheckBox.IsEnabled = true;
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
            // if video was checked
            if (videoCheckBox.IsChecked == true)
            {
                // uncheck it!
                videoCheckBox.IsChecked = false;
            }
            // enable music buttons
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
            addMetadataCheckBox.IsEnabled = false;
            musicAddThumbnailCheckBox.IsEnabled = false;
            // uncheck them
            musicAACRadioButton.IsChecked = false;
            musicMp3RadioButton.IsChecked = false;
            musicOpusRadioButton.IsChecked = false;
            addMetadataCheckBox.IsChecked = false;
            musicAddThumbnailCheckBox.IsChecked = false;
        }

        // video Check Box 
        // checked
        private void videoCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // if music box was checked
            if (musicCheckBox.IsChecked == true)
            {
                // uncheck it
                musicCheckBox.IsChecked = false;
            }
            // video button enablers
            videoMkvRadioButton.IsEnabled = true;
            videoMp4RadioButton.IsEnabled = true;

            video4KRadioButton.IsEnabled = true;
            video2KRadioButton.IsEnabled = true;
            videoFHDRadioButton.IsEnabled = true;
            videoHDRadioButton.IsEnabled = true;
            video480pRadioButton.IsEnabled = true;
            videoBestRadioButton.IsEnabled = true;


            MessageBox.Show("If you choose to download video, it may take time depending on video size and your computer power.\r\n\r\n" +
                "When it is finished, it will show up a messagebox that says it is finished. \r\nSo please allow upto an hour to finish or just abort and choose best file option.\r\n\r\n" +
                "For now, video download only supports bestvideo downloads. If you try to download 8K video, then this program will actually download 8K version.\r\n\r\n" +
                "Update will be released soon!",
                "Notice");
        }

        // unchecked
        private void videoCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // unable checkboxes
            videoMkvRadioButton.IsEnabled = false;
            videoMp4RadioButton.IsEnabled = false;
            // no checks
            videoMkvRadioButton.IsChecked = false;
            videoMp4RadioButton.IsChecked = false;

            video4KRadioButton.IsEnabled = false;
            video2KRadioButton.IsEnabled = false;
            videoFHDRadioButton.IsEnabled = false;
            videoHDRadioButton.IsEnabled = false;
            video480pRadioButton.IsEnabled = false;
            videoBestRadioButton.IsEnabled = false;
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

            // reset all checks
            musicAACRadioButton.IsChecked = false;
            musicMp3RadioButton.IsChecked = false;
            musicOpusRadioButton.IsChecked = false;
            videoMkvRadioButton.IsChecked = false;
            videoMp4RadioButton.IsChecked = false;
            musicCheckBox.IsChecked = false;
            videoCheckBox.IsChecked = false;

            video4KRadioButton.IsChecked = false;
            video2KRadioButton.IsChecked = false;
            videoFHDRadioButton.IsChecked = false;
            videoHDRadioButton.IsChecked = false;
            video480pRadioButton.IsChecked = false;
            videoBestRadioButton.IsChecked = false;

            addMetadataCheckBox.IsChecked = false;
            musicAddThumbnailCheckBox.IsChecked = false;

            outputCheckBox.IsChecked = false;

            // reset all enables
            musicCheckBox.IsEnabled = false;
            videoCheckBox.IsEnabled = false;
            openFolderBtn.IsEnabled = false;
            downloadBtn.IsEnabled = false;

            video4KRadioButton.IsEnabled = false;
            video2KRadioButton.IsEnabled = false;
            videoFHDRadioButton.IsEnabled = false;
            videoHDRadioButton.IsEnabled = false;
            video480pRadioButton.IsEnabled = false;
            videoBestRadioButton.IsEnabled = false;

            addMetadataCheckBox.IsEnabled = false;
            musicAddThumbnailCheckBox.IsEnabled = false;

            outputCheckBox.IsEnabled = false;

            // make these two buttons work again
            urlTextBox.IsEnabled = true;
            validateBtn.IsEnabled = true;
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
                if (musicCheckBox.IsChecked == true )
                {
                    MessageBox.Show("Music Download Started!", "Started");
                    downloadWorker.RunWorkerAsync(scripts);
                }
                else if (videoCheckBox.IsChecked == true)
                {
                    MessageBox.Show("Video Download Started!", "Started");
                    downloadWorker.RunWorkerAsync(scripts);
                }
                else
                {
                    MessageBox.Show("Please choose Video or Music.", "Error");
                }
            }
        }

        // metadata Check Box event Handlers
        // Checked
        private void addMetadataCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            metadataString.IsEnabled = true;
        }

        // Unchecked
        private void addMetadataCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            metadataString.IsEnabled = false;
        }

        // metadata Text Box event Handlers
        // Got Focus
        private void metadataString_GotFocus(object sender, RoutedEventArgs e)
        {
            metadataString.Text = "";
        }

        // Lost Focus
        private void metadataString_LostFocus(object sender, RoutedEventArgs e)
        {
            if (metadataString.Text == "")
            {
                metadataString.Text = "%(title)s for title, %(artist)s for artist";
            }
        }

        // musicMp3RadioButton event handlers
        private void musicMp3RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            addMetadataCheckBox.IsEnabled = true;
            musicAddThumbnailCheckBox.IsEnabled = true;
        }

        // musicopusRadioButton event handlers
        private void musicOpusRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Opus format does not support music metadata.\r\nTherefore, it is not supported in this program.", "Notice");
            addMetadataCheckBox.IsChecked = false;
            addMetadataCheckBox.IsEnabled = false;
            musicAddThumbnailCheckBox.IsEnabled = false;
            musicAddThumbnailCheckBox.IsChecked = false;
            metadataString_LostFocus(sender, e);
        }

        // musicM4aRadioButton event handlers
        private void musicAACRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            addMetadataCheckBox.IsEnabled = true;
            musicAddThumbnailCheckBox.IsEnabled = true;
        }

        // output CheckBox event handlers 
        private void outputCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            outputNameTextBox.IsEnabled = true;
        }

        private void outputCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            outputNameTextBox.Text = "default = %(title)s.%(ext)s";
            outputNameTextBox.IsEnabled = false;
        }

        // outputTextBox event handlers
        private void outputNameTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (outputNameTextBox.Text == "default = %(title)s.%(ext)s")
            {
                outputNameTextBox.Text = "";
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
            // music options
            if (musicCheckBox.IsChecked == true)
            {
                scriptText += " -x";
                // ext format
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

                // metadata
                if (addMetadataCheckBox.IsChecked == true)
                {
                    scriptText += " --metadata-from-title " + "\"" + metadataString.Text + "\"";
                    scriptText += " --add-metadata";
                }

                // Thumbnail
                if (musicAddThumbnailCheckBox.IsChecked == true)
                {
                    scriptText += " --embed-thumbnail";
                }
            }
            // video option
            else if (videoCheckBox.IsChecked == true)
            {
                string resolution = "";
                if (video4KRadioButton.IsChecked == true)
                {
                    resolution = "[height=2160]";
                }
                else if (video2KRadioButton.IsChecked == true)
                {
                    resolution = "[height=1440]";
                }
                else if (videoFHDRadioButton.IsChecked == true)
                {
                    resolution = "[height=1080]";
                }
                else if (videoHDRadioButton.IsChecked == true)
                {
                    resolution = "[height=720]";
                }
                else if (video480pRadioButton.IsChecked == true)
                {
                    resolution = "[height<=480]";
                }
                
                if (videoMkvRadioButton.IsChecked == true)
                {
                    
                    if (!String.IsNullOrEmpty(resolution))
                    {
                        scriptText += " -f bestvideo" + resolution+ "[ext=webm]+bestaudio[ext = opus]/bestvideo" + resolution + "+bestaudio/best" + resolution;
                    }
                    else
                    {
                        scriptText += " -f bestvideo[ext=webm]+bestaudio[ext=opus]/bestvideo+bestaudio/best";
                    }
                    scriptText += " --recode-video mkv";
                }
                else if (videoMp4RadioButton.IsChecked == true)
                {
                    if (!String.IsNullOrEmpty(resolution))
                    {
                        scriptText += " -f bestvideo" + resolution + "[ext=mp4]+bestaudio[ext=m4a]/bestvideo" + resolution + "+bestaudio/best" + resolution;
                    }
                    else
                    {
                        scriptText += " -f bestvideo[ext=mp4]+bestaudio[ext=m4a]/bestvideo+bestaudio/best";
                    }
                    scriptText += " --recode-video mp4";
                }
                else
                {
                    if (!String.IsNullOrEmpty(resolution))
                    {
                        scriptText += " -f bestvideo" + resolution + "[ext=mp4]+bestaudio[ext=m4a]/bestvideo" + resolution + "+bestaudio/best" + resolution;
                    }
                    else
                    {
                        scriptText += " -f bestvideo[ext=mp4]+bestaudio[ext=m4a]/bestvideo+bestaudio/best";
                    }
                    scriptText += " --recode-video mp4";
                }
            }

            // output selection
            if (outputCheckBox.IsChecked == true)
            {
                scriptText += " -o \"" + outputNameTextBox.Text + ".%(ext)s\"";
            }
            else
            {
                scriptText += " -o \"%(title)s.%(ext)s\"";
            }
            scriptText += " --hls-prefer-native";
            return scriptText;
        }
    }
}
