using FunnyLanguage_WPF.Services;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
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
using YoutubeExplode.Videos.ClosedCaptions;
using YoutubeExplode;
using System.Text.RegularExpressions;
using YoutubeExplode.Videos.Streams;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.EntityFrameworkCore;
using FunnyLanguage_WPF.Models;
using Microsoft.EntityFrameworkCore.Storage;
using YoutubeExplode.Videos;
using System.Net;

namespace FunnyLanguage_WPF
{
    /// <summary>
    /// Interaction logic for AddVideoDialog.xaml
    /// </summary>
    public partial class AddVideoDialog : Window
    {
        public AddVideoDialog()
        {
            InitializeComponent();
        }

        public string VideoName { get => videoNametxtbl.Text.Trim(); set => videoNametxtbl.Text = value; }
        public string VideoUrl { get => videoUrltxtbl.Text.Trim(); set => videoUrltxtbl.Text = value; }

        private void Button_Add(object sender, RoutedEventArgs e)
        {
           

            string name = videoNametxtbl.Text.Trim();
            string url = videoUrltxtbl.Text;
            if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(url))
            {
                using var db = new VideoContext();
                var video = db.Videos.Where(v => v.Url == url).FirstOrDefault();
                var video2 = db.Videos.Where(v => v.NameForDownload == name).FirstOrDefault();

                // Check if the input string contains any of the specified characters
                if (!ValidateVideoName(name))
                {
                    errorMessage.Content = "You can only use letters, numbers, spaces and '_' or '-' !!!";
                }
                else if (!ValidateUrl(url))
                {
                    errorMessage.Content = "The URL is not a valid YouTube URL address!!!";
                }
                else if (video != null)
                {
                   
                    errorMessage.Content = "Video with this url already exists!!!";
                } else if (video2!= null)
                {
                    errorMessage.Content = "There is already a video under this name!!!";
                }
                else
                {
                    errorMessage.Content = "";
                    MessageBox.Show("The video has started downloading, please wait.");
                    DialogResult = true;
                    this.Close();
                }

            }
            else
            {
                errorMessage.Content = "You need to fill the name of the video and url of the video to save it!!!";
            }

        }
        /// <summary>
        /// vygenerované CHATGPT
        /// </summary>
        /// <param name="videoName"></param>
        /// <returns></returns>
        private bool ValidateVideoName(string videoName)
        {
            // Define the regular expression pattern to match the specified characters
            string pattern = @"^[a-zA-Z0-9 _-]*$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(videoName);
        }
        /// <summary>
        /// inšpirované  touto stránkou: https://stackoverflow.com/questions/14823544/how-to-check-if-youtube-url-in-c-sharp
        /// </summary>
        /// <param name="videoUrl"></param>
        /// <returns></returns>
        private bool ValidateUrl(string videoUrl)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(videoUrl);
                request.Method = "HEAD";
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    return response.ResponseUri.ToString().Contains("youtube.com") ? true : false;
                }
            } catch {
                MessageBox.Show("Wrong url!");
                return false;
            }
        }

        private void Button_Cancel(object sender, RoutedEventArgs e)
        {  
            this.Close();
        }
      
    }
}
