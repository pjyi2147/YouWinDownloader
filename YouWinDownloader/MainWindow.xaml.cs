using System;
using System.Collections.Generic;
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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void urlTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            urlTextBox.Text = "";
        }

        private void urlTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (urlTextBox.Text == "")
            {
                urlTextBox.Text = "Full url including http://";
            }
        }


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

        private void urlTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                validateBtn_Click(sender, e);
            }
        }


        // button checker functions
        // music Check Box
        private void musicCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (videoCheckBox.IsChecked == true)
            {
                videoCheckBox.IsChecked = false;
            }
            musicAACCheckBox.IsEnabled = true;
            musicMp3CheckBox.IsEnabled = true;
            musicOpusCheckBox.IsEnabled = true;
        }

        private void musicCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            musicAACCheckBox.IsEnabled = false;
            musicMp3CheckBox.IsEnabled = false;
            musicOpusCheckBox.IsEnabled = false;
        }

        // video Check Box
        private void videoCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (musicCheckBox.IsChecked == true)
            {
                musicCheckBox.IsChecked = false;
            }
            videoAviCheckBox.IsEnabled = true;
            videoMkvCheckBox.IsEnabled = true;
            videoMp4CheckBox.IsEnabled = true;
        }

        private void videoCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            videoAviCheckBox.IsEnabled = false;
            videoMkvCheckBox.IsEnabled = false;
            videoMp4CheckBox.IsEnabled = false;
        }


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


        // clear button
        private void clearBtn_Click(object sender, RoutedEventArgs e)
        {
            urlTextBox.Text = "";

            musicAACCheckBox.IsChecked = false;
            musicMp3CheckBox.IsChecked = false;
            musicOpusCheckBox.IsChecked = false;
            videoAviCheckBox.IsChecked = false;
            videoMkvCheckBox.IsChecked = false;
            videoMp4CheckBox.IsChecked = false;
            musicCheckBox.IsChecked = false;
            videoCheckBox.IsChecked = false;

            musicCheckBox.IsEnabled = false;
            videoCheckBox.IsEnabled = false;
            openFolderBtn.IsEnabled = false;
            downloadBtn.IsEnabled = false;
        }
    }
}
