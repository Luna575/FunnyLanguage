﻿using FunnyLanguage_WPF.Services;
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
using System.Net.Http;

namespace FunnyLanguage_WPF
{
    /// <summary>
    /// Interaction logic for AddVideoDialog.xaml
    /// </summary>
    public partial class AddVideoDialog : Window
    {
        private MainWindow _window;
        public AddVideoDialog(MainWindow window)
        {
            InitializeComponent();
            _window = window;
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
                    errorMessage.Content = "You can only use letters, numbers, spaces and the underscore character or '-' !!!";
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
                    _window.downloadinglbl.Visibility = System.Windows.Visibility.Visible;
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
            string pattern = @"^[a-zA-Z0-9 _\-]*$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(videoName);
        }
       
        private bool ValidateUrl(string videoUrl)
        {
            using var httpClient = new HttpClient();
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, videoUrl);
                var response = httpClient.Send(request);
                using var reader = new StreamReader(response.Content.ReadAsStream());
                var responseBody = reader.ReadToEnd();
                if (!string.IsNullOrEmpty(responseBody))
                {
                  //TODO otestovať
                    return videoUrl.Contains("youtube.com") ? true : false;
                }
                return false;
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
