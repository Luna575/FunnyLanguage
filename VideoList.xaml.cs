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
using YoutubeExplode.Videos.Streams;
using Newtonsoft.Json;
using FunnyLanguage_WPF.Models;
using System.IO.Packaging;
using System.Reflection.Metadata;
using FunnyLanguage_WPF.Services;
using System.Collections.ObjectModel;
using YoutubeExplode.Videos;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore.Storage;
using System.Net.NetworkInformation;
using AngleSharp.Dom;

namespace FunnyLanguage_WPF
{
    /// <summary>
    /// Interaction logic for VideoList.xaml
    /// </summary>
    public partial class VideoList : Page
    {
        public ObservableCollection<Models.Video> videoCollection;
        private FunnyLanguage_WPF.MainWindow _window;
        public VideoList(FunnyLanguage_WPF.MainWindow window)
        {
            InitializeComponent();

            using var db = new VideoContext();

            videoCollection = new ObservableCollection<Models.Video>(db.Videos.ToList());
            videolist.ItemsSource = videoCollection;
            videolist.DisplayMemberPath = "Title";
            videolist.SelectedIndex = 0;
            _window = window;
            window.backbtn.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Button_Add(object sender, RoutedEventArgs e)
        {
            if (InternetConnection())
            {
                var addWindow = new AddVideoDialog(_window);
                if (addWindow.ShowDialog() == true)
                {
                    
                    var name = addWindow.VideoName;
                    var url = addWindow.VideoUrl;
                    try
                    {
                        Task.Run(() => AddVideos(name, url)).Wait();
                    }
                    catch (Exception ex) { MessageBox.Show("Error: " + ex); }
                    _window.downloadinglbl.Visibility=System.Windows.Visibility.Collapsed;
                    using var db = new VideoContext();
                    videoCollection = new ObservableCollection<Models.Video>(db.Videos.ToList());
                    videolist.ItemsSource = videoCollection;
                }
            } else
            {
                MessageBox.Show("Please connect to the Internet or check your Internet connection!!!");
            }
          

        }
        private void Button_Delete(object sender, RoutedEventArgs e)
        {

            Models.Video video = (Models.Video)videolist.SelectedItem;
            if (video != null)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this video with the words to play with?", "Confirmation", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {

                    
                    try
                    {
                        Task.Run(() => Deletevideo(video)).Wait();
                    }
                    catch (Exception ex) { MessageBox.Show("Error: " + ex); }
                    using var db = new VideoContext();
                    videoCollection = new ObservableCollection<Models.Video>(db.Videos.ToList());
                    videolist.ItemsSource = videoCollection;
                }
            }
        }
        private void Button_DeleteVideoFileOnly(object sender, RoutedEventArgs e)
        {

            Models.Video video = (Models.Video)videolist.SelectedItem;
            if (video != null)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this video's files from your device's memory?", "Confirmation", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {


                    try
                    {
                        DeletevideoFiles(video);
                        using var db = new VideoContext();
                        videoCollection = new ObservableCollection<Models.Video>(db.Videos.ToList());
                        videolist.ItemsSource = videoCollection;
                    }
                    catch (Exception ex) { MessageBox.Show("Error: " + ex); }
                   
                }
            }
        }
        private void DeletevideoFiles(Models.Video video)
        {
            using (var videoContext = new VideoContext())
            {
                using (var transaction = videoContext.Database.BeginTransaction())
                {
                    try
                    {

                        var v = videoContext.Videos.Where(x=> x.Url == video.Url).FirstOrDefault();
                        if (v != null)
                        {
                            v.IsVideoDeleted = true;
                            videoContext.SaveChangesAsync();
                        }
    
                        transaction.Commit();

                        if (File.Exists(video.FolderPath))
                        {
                            File.Delete(video.FolderPath);
                            MessageBox.Show("The video has been successfully deleted.");
                        }
                        else
                        {
                            MessageBox.Show("The video does not exist!");
                        }
                        if (Directory.Exists(video.CaptionFolderPath))
                        {
                            Directory.Delete(video.CaptionFolderPath, true);
                            MessageBox.Show("The subtitle's folder was successfully deleted.");
                        }
                        else
                        {
                            MessageBox.Show("The subtitle's folder does not exist!");
                        }

                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction only if it is active
                        if (transaction.GetDbTransaction() != null)
                        {
                            transaction.Rollback();
                        }
                        MessageBox.Show("Error to perform a transaction: " + ex.Message);
                    }
                }
            }

           


        }

        private async Task Deletevideo(Models.Video video)
        {
            using (var videoContext = new VideoContext())
            {
                using (var transaction = videoContext.Database.BeginTransaction())
                {
                    try
                    {

                        Models.Video_Language[] video_Languages = videoContext.VideoLanguages.Where(v => v.VideoId == video.VideoId).ToArray();
                        if (video_Languages.Length > 0)
                        {
                            foreach (var language in video_Languages)
                            {
                                videoContext.Remove(language);
                            }
                        }
                        Models.WordList? wordList = videoContext.WordLists.Where(v => v.VideoId == video.VideoId).FirstOrDefault();
                        if (wordList != null)
                        {
                            Models.Word[]? words = videoContext.Words.Where(v => v.WordlistId == wordList.WordListId).ToArray();
                            if (words.Length > 0 && words != null)
                            {
                                foreach (var word in words)
                                {
                                    videoContext.Remove(word);
                                }
                                
                            }
                            videoContext.Remove(wordList);
                        }
                        videoContext.Remove(video);
                        await videoContext.SaveChangesAsync();

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction only if it is active
                        if (transaction.GetDbTransaction() != null)
                        {
                            transaction.Rollback();
                        }
                        MessageBox.Show("Error to perform a transaction: " + ex.Message);
                    }
                }
            }

           DeletevideoFiles(video);

        }

  
        private void Button_PlayVideo(object sender, RoutedEventArgs e)
        {
            Models.Video? video = (Models.Video)videolist.SelectedItem;
            if (video != null)
            {
                if (!video.IsVideoDeleted)
                {
                    _window.backbtn.Visibility = System.Windows.Visibility.Visible;
                    _window.Main.Content = new FunnyLanguage_WPF.Player(video);
                } else
                {
                    MessageBoxResult result = MessageBox.Show("There are no files for this video. Do you want to download them again?", "Confirmation", MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        if (InternetConnection())
                        {
                            _window.downloadinglbl.Visibility = System.Windows.Visibility.Visible;
                            MessageBox.Show("The video has started downloading, please wait.");
                            var name = video.NameForDownload;
                            var url = video.Url;
                            try
                            {
                                Task.Run(() => AddVideos(name, url)).Wait();
                                using (var videoContext = new VideoContext())
                                {
                                    using (var transaction = videoContext.Database.BeginTransaction())
                                    {
                                        try
                                        {

                                            var v = videoContext.Videos.Where(x => x.Url == video.Url).FirstOrDefault();
                                            if (v != null)
                                            {
                                                v.IsVideoDeleted = false;
                                                videoContext.SaveChangesAsync();
                                            }

                                            transaction.Commit();
                                            _window.backbtn.Visibility = System.Windows.Visibility.Visible;
                                            _window.downloadinglbl.Visibility = System.Windows.Visibility.Collapsed;
                                            _window.Main.Content = new FunnyLanguage_WPF.Player(video);

                                        }
                                        catch (Exception ex)
                                        {
                                            // Rollback the transaction only if it is active
                                            if (transaction.GetDbTransaction() != null)
                                            {
                                                transaction.Rollback();
                                            }
                                            MessageBox.Show("Error to perform a transaction: " + ex.Message);
                                        }
                                    }
                                }
                            }
                            catch (Exception ex) { MessageBox.Show("Error: " + ex); }
                            
                        }

                        else
                        {
                            MessageBox.Show("Please connect to the Internet or check your Internet connection!!!");
                        }
                    }
                }
            }
        }
        private async Task AddVideos(string name, string link)
        {
            var videoUrl = link;
            var youtube = new YoutubeClient();
            string error = string.Empty;
            string title = "";
            string videoId = "";
            List<Models.Language> languages = new List<Models.Language>();
            List<string> codes = new List<string>();
            string appDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string mainFolderName = "FunnyLanguageApp_data";
            string mainFolderPath = System.IO.Path.Combine(appDataFolderPath, mainFolderName);
            if (!Directory.Exists(mainFolderPath))
            {
                Directory.CreateDirectory(mainFolderPath);
            }
            try
            {
                var video = await youtube.Videos.GetAsync(videoUrl);
                title = video.Title;
                videoId = video.Id;
            }
            catch (Exception ex)
            {
                error += "Problem with URL!!! ";
            }
            if (string.IsNullOrEmpty(error))
            {
                string captionfolderPath = "";
                try
                {
                    ClosedCaptionManifest trackManifest = await youtube.Videos.ClosedCaptions.GetManifestAsync(videoUrl);
                    var nonAutoGeneratedTracks = trackManifest.Tracks.Where(track => !track.IsAutoGenerated);

                    if (nonAutoGeneratedTracks.Any())
                    {

                        // Create a new folder path within the AppData folder
                        string folderName = "DownloadedCaptions";
                        string helpfolderPath = System.IO.Path.Combine(mainFolderPath, folderName);
                        if (!Directory.Exists(helpfolderPath))
                        {
                            Directory.CreateDirectory(helpfolderPath);
                        }
                        string folderName2 = name;
                        captionfolderPath = System.IO.Path.Combine(helpfolderPath, folderName2);

                        // Check if the folder exists, if not, create it
                        if (!Directory.Exists(captionfolderPath))
                        {
                            Directory.CreateDirectory(captionfolderPath);
                        }
                        foreach (var track in nonAutoGeneratedTracks)
                        {

                            var language = new Models.Language();
                            language.Name = track.Language.Name;
                            language.Code = track.Language.Code;
                            using (var videoContext = new VideoContext())
                            {
                                using (var transaction = videoContext.Database.BeginTransaction())
                                {
                                    try
                                    {

                                        var existingLanguage = videoContext.Languages.Find(language.Code);
                                        if (existingLanguage == null)
                                        {

                                            videoContext.Add(language);
                                        }

                                        videoContext.SaveChanges();
                                        transaction.Commit();
                                    }
                                    catch (Exception ex)
                                    {
                                        // Rollback the transaction only if it is active
                                        if (transaction.GetDbTransaction() != null)
                                        {
                                            transaction.Rollback();
                                        }
                                        MessageBox.Show("Error to perform a transaction: " + ex.Message);
                                    }
                                }

                            }
                            languages.Add(language);
                            codes.Add(language.Code);
                            var trackInfo = trackManifest.GetByLanguage(track.Language.Code);
                            await youtube.Videos.ClosedCaptions.DownloadAsync(trackInfo, "cc_track.srt");



                            // Specify the file name for the downloaded video
                            string fileName = $"{track.Language.Code}.srt";

                            // Combine the folder path with the file name to get the full file path
                            string captionfilePathInAppDataFolder = System.IO.Path.Combine(captionfolderPath, fileName);
                            // Assuming you have downloaded the video file and saved it locally
                            string captionFilePath = "cc_track.srt";

                            // Read the video file into a byte array
                            byte[] captionBytes = File.ReadAllBytes(captionFilePath);
                            // Save the downloaded video with audio to the specified file path
                            // You can use file operations like File.WriteAllBytes() to save the file
                            // For example:
                            File.WriteAllBytes(captionfilePathInAppDataFolder, captionBytes);
                        }
                    }
                    else
                    {
                        error += "Subtitles that aren't automatically generated don't exist for this video, so you can't download this video. ";
                    }
                }
                catch (Exception ex) { error += "Could not get the captions for this video. "; }
                string filePathInAppDataFolder = "";
                if (!videoId.Equals("") && string.IsNullOrEmpty(error))
                {

                    try
                    {
                        var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoUrl);
                        var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestVideoQuality();
                        var stream = await youtube.Videos.Streams.GetAsync(streamInfo);
                        // Download the stream to a file
                        await youtube.Videos.Streams.DownloadAsync(streamInfo, $"video.{streamInfo.Container}");
                        // Create a new folder path within the AppData folder
                        string folderName = "DownloadedVideos";
                        string folderPath = System.IO.Path.Combine(mainFolderPath, folderName);

                        // Check if the folder exists, if not, create it
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        // Specify the file name for the downloaded video
                        string fileName = $"{name}.{streamInfo.Container}";

                        // Combine the folder path with the file name to get the full file path
                        filePathInAppDataFolder = System.IO.Path.Combine(folderPath, fileName);
                        // Assuming you have downloaded the video file and saved it locally
                        string videoFilePath = $"video.{streamInfo.Container}";

                        // Read the video file into a byte array
                        byte[] videoBytes = File.ReadAllBytes(videoFilePath);
                        // Save the downloaded video with audio to the specified file path
                        // You can use file operations like File.WriteAllBytes() to save the file
                        // For example:
                        File.WriteAllBytes(filePathInAppDataFolder, videoBytes);
                    }
                    catch (Exception ex) { error = "Could not get this video. "; }
                }

                if (error.Equals(string.Empty))
                {
                    var video = new Models.Video();
                    video.Url = videoUrl;
                    video.FolderPath = filePathInAppDataFolder;
                    video.CaptionFolderPath = captionfolderPath;
                    video.Title = title;
                    video.NameForDownload = name;
                    video.VideoCode = videoId;
                    video.LanguageCodes = codes;
                    using (var videoContext = new VideoContext())
                    {
                        using (var transaction = videoContext.Database.BeginTransaction())
                        {
                            try
                            {
                                var v = videoContext.Videos.Where(x => x.Url == videoUrl).FirstOrDefault();
                                if (v == null)
                                {
                                    videoContext.Add(video);


                                    videoContext.SaveChanges();
                                    foreach (var lcode in video.LanguageCodes)
                                    {
                                        var l = videoContext.Languages.Where(l => l.Code == lcode).FirstOrDefault();
                                        if (l != null)
                                        {
                                            Video_Language video_Language = new Video_Language();
                                            video_Language.Video = video;
                                            video_Language.VideoId = video.VideoId;
                                            video_Language.LanguageCode = lcode;
                                            video_Language.Language = l;
                                            videoContext.Add(video_Language);
                                            videoContext.SaveChanges();
                                        }

                                    }

                                    // end of transaction
                                    transaction.Commit();
                                }
                            }
                            catch (Exception ex)
                            {
                                // Rollback the transaction only if it is active
                                if (transaction.GetDbTransaction() != null)
                                {
                                    transaction.Rollback();
                                }
                                MessageBox.Show("Error to perform a transaction: " + ex.Message);
                                if (File.Exists(video.FolderPath))
                                {
                                    File.Delete(video.FolderPath);

                                }

                                if (Directory.Exists(video.CaptionFolderPath))
                                {
                                    Directory.Delete(video.CaptionFolderPath, true);

                                }

                            }
                        }
                        MessageBox.Show("Video has been successfully added");

                    }


                }
                else
                {
                    MessageBox.Show(error);
                }
            }
        }
        /// <summary>
        /// Inšpirované videom (https://www.youtube.com/watch?v=m_SDXyLR0Oc), táto metóda sa používa na zistenie pripojenia k Internetu.
        /// </summary>
        /// <returns> bool </returns>
        public bool InternetConnection()
        {
            var pingStatus = false;
            string hostNameOrAddress = "Google.com";
            using(Ping ping = new Ping())
            {
               
                int timeout = 1000;
                try
                {
                    PingReply reply = ping.Send(hostNameOrAddress, timeout);
                    pingStatus = (reply.Status == IPStatus.Success);
                }catch (Exception ) 
                { 
                    pingStatus = false;
                }
                return pingStatus;
            }

        }

        private void Button_Words(object sender, RoutedEventArgs e)
        {
            Models.Video video = (Models.Video)videolist.SelectedItem;
            if (video != null)
            {
                using var db = new VideoContext();
                var wordList = db.WordLists.Where(x => x.VideoId == video.VideoId).FirstOrDefault();
                if (wordList != null)
                {
                   var wordsl = new ObservableCollection<Models.Word>(db.Words.Where(x => x.WordlistId == wordList.WordListId).ToList());
                    if (wordsl != null)
                    {
                        var words = Words.GetInstance(video.VideoId, this);
                        words.Show();
                    }
                    else { MessageBox.Show("There are no saved words for this video!!!"); }

                }
                else { MessageBox.Show("There are no saved words for this video!!!"); }

            }
        }

        private void Button_Games(object sender, RoutedEventArgs e)
        {

            Models.Video? video = (Models.Video)videolist.SelectedItem;
            if (video != null)
            {
                using var db = new VideoContext();
                var wordList = db.WordLists.Where(x => x.VideoId == video.VideoId).FirstOrDefault();
                if (wordList != null)
                {
                    var word = db.Words.Where(x => x.WordlistId == wordList.WordListId && x.KnowIt.Equals("Don't know")).FirstOrDefault();
                    if (word != null)
                    {
                        _window.backbtn.Visibility = System.Windows.Visibility.Visible;
                        _window.Main.Content = new FunnyLanguage_WPF.Games(video);
                    }
                    else
                    {
                        MessageBox.Show("There are no words to play with!!!");
                    }
                }
                else
                {
                    MessageBox.Show("There are no words to play with!!!");
                }
            }
           
        }

        private void Button_AllWords(object sender, RoutedEventArgs e)
        {
            using var db = new VideoContext();
            var wordsl = new ObservableCollection<Models.Word>(db.Words.ToList());
            if (wordsl != null)
            {
                var words = Words.GetInstance(null, this);
                words.Show();
            }
            else { MessageBox.Show("There are no saved words for this video!!!"); }
        }

        private void videolist_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using var db = new VideoContext();
            if (videolist.SelectedItems.Count == 0)
            {
                deletebtn.IsEnabled = false;
                wordsbtn.IsEnabled = false;
                playGamebtn.IsEnabled = false;
                playVideobtn.IsEnabled = false;
                deleteVideoFilebtn.IsEnabled = false;
            }
            else
            {
                
                Models.Video? video = (Models.Video)videolist.SelectedItem;
                
                if (video != null)
                {
                    if (video.IsVideoDeleted)
                    {
                        deleteVideoFilebtn.IsEnabled = false;
                    } else { deleteVideoFilebtn.IsEnabled = true; }
                    deletebtn.IsEnabled = true;
                    playVideobtn.IsEnabled = true;
                    var wordList = db.WordLists.Where(x => x.VideoId == video.VideoId).FirstOrDefault();
                    if (wordList != null)
                    {
                        var word = db.Words.Where(x => x.WordlistId == wordList.WordListId).FirstOrDefault();
                        if (word != null)
                        {
                            wordsbtn.IsEnabled = true;
                            word = db.Words.Where(x => x.WordlistId == wordList.WordListId && x.KnowIt.Equals("Don't know")).FirstOrDefault();
                            if (word != null)
                            {
                                playGamebtn.IsEnabled = true;
                            }
                            else { playGamebtn.IsEnabled = false; }
                        }
                        else
                        {
                            wordsbtn.IsEnabled = false;
                            playGamebtn.IsEnabled = false;
                        }
                    }
                    else
                    {
                        wordsbtn.IsEnabled = false;
                        playGamebtn.IsEnabled = false;
                    }
                }
                else
                {
                    deletebtn.IsEnabled = false;
                    playVideobtn.IsEnabled = false;
                    wordsbtn.IsEnabled = false;
                    playGamebtn.IsEnabled = false;
                    deleteVideoFilebtn.IsEnabled = false;
                }

               
            }
            var wordsl = new ObservableCollection<Models.Word>(db.Words.ToList());
            if (wordsl != null)
            {
                allwordsbtn.IsEnabled = true;
                var  words2 = db.Words.Where(x=> x.KnowIt.Equals("Know it")).ToList();
                if (words2 != null)
                {
                    statistic1xtbl.Text = "Total number of learned words and phrases: " + words2.Count;
                }
               words2 = db.Words.Where(x => x.KnowIt.Equals("Know it") && x.LastTime.Date == DateTime.Now.Date).ToList();
                if (words2 != null)
                {
                    statistic2xtbl.Text = "Total number of learned words and phrases for today: " + words2.Count;
                }

            }
            else { allwordsbtn.IsEnabled = false; }
        }
        /// <summary>
        /// vygenerované CHATGPT
        /// </summary>
        /// <param name="query"></param>
        private void SearchVideos(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                videolist.ItemsSource = videoCollection;
            }
            else
            {
                var filteredVideos = new ObservableCollection<Models.Video>(
                    videoCollection.Where(video => video.Title.ToLower().Contains(query.ToLower())).ToList());
                videolist.ItemsSource = filteredVideos;
            }
        }
        /// <summary>
        ///  vygenerované CHATGPT
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchtxtbl_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchQuery = searchtxtbl.Text.Trim();
            SearchVideos(searchQuery);
        }
    }

}
