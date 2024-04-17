using FunnyLanguage_WPF.Models;
using FunnyLanguage_WPF.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Numerics;
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
using System.Windows.Threading;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.ClosedCaptions;

namespace FunnyLanguage_WPF
{
    
        
    
    /// <summary>
    /// Interaction logic for Player.xaml
    /// </summary>}")
    public partial class Player : Page
    {
       private DispatcherTimer timer;

        public ObservableCollection<Models.Language> languageCollection;
        private bool captionBool = false;
        private Models.Video _video;
        private Models.Language _language;
        private List<SubtitlesParser.Classes.SubtitleItem> subtitles;
        public Player(Models.Video video)
        {
            InitializeComponent();
            captiontxt.Visibility = Visibility.Collapsed;
            _video = video;
            if (System.IO.Path.Exists(video.FolderPath))
            {
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(1);
                timer.Tick += new EventHandler(timer_Tick);
                mediaElement.Source = new Uri(video.FolderPath, UriKind.RelativeOrAbsolute);
                using var db = new VideoContext();
                var video_Languages = db.VideoLanguages.Where(v => v.VideoId == video.VideoId).OrderBy(x => x.Language.Name).ToList();
                languageCollection = new ObservableCollection<Models.Language>();
                foreach (var l in video_Languages)
                {
                    var l_language = db.Languages.Where(x => x.Code == l.LanguageCode).FirstOrDefault();
                    if (l_language != null)
                    {
                        languageCollection.Add(l_language);
                    }
                    
                }
                language.ItemsSource = languageCollection;
                language.DisplayMemberPath = "Name";
                language.SelectedIndex = 0;
                fontSize.SelectedIndex = 0;

            }
            else
            {
                captiontxt.Text = "Files not found for this video";
                captiontxt.Visibility = Visibility.Visible;
            }
            
        }

        private void timer_Tick(object? sender, EventArgs e)
        {
            TimeSpan currentTime = mediaElement.Position;
            slider.Value = currentTime.TotalSeconds;
            beginTimeTxt.Content = currentTime.ToString(@"hh\:mm\:ss");

            if (captionBool)
            {
                if (subtitles != null)
                {
                    foreach (SubtitlesParser.Classes.SubtitleItem subtitle in subtitles)
                    {
                        if (currentTime.TotalMilliseconds >= subtitle.StartTime && currentTime.TotalMilliseconds <= subtitle.EndTime)
                        {
                            string text = "";
                            foreach(var txt in subtitle.PlaintextLines)
                            {
                                text += txt;
                            }
                            if (text != "")
                            {
                                captiontxt.Text = text;
                                captiontxt.Visibility = Visibility.Visible;
                                
                            }
                            else
                            {
                                captiontxt.Visibility = Visibility.Collapsed;
                            }
                            break;
                        }
                        else
                        {
                            captiontxt.Visibility = Visibility.Collapsed;
                        }
                    }
                }
                else { captiontxt.Visibility = Visibility.Collapsed; }
            } else { captiontxt.Visibility = Visibility.Collapsed; };
        }

        private void Button_Play(object sender, RoutedEventArgs e)
        {
            
            mediaElement.Play();
            
        }
        private void Button_Pause(object sender, RoutedEventArgs e)
        {

            mediaElement.Pause();
            
        }

        private void Button_Stop(object sender, RoutedEventArgs e)
        {

            mediaElement.Stop();
            captiontxt.Visibility = Visibility.Collapsed;

        }
        private void Button_Forward(object sender, RoutedEventArgs e)
        {
            mediaElement.Position += TimeSpan.FromSeconds(5);

            slider.Value = mediaElement.Position.TotalSeconds;
            beginTimeTxt.Content = mediaElement.Position.ToString(@"hh\:mm\:ss");

        }
        private void Button_Backward(object sender, RoutedEventArgs e)
        {

            mediaElement.Position -= TimeSpan.FromSeconds(5);

            slider.Value = mediaElement.Position.TotalSeconds;
            beginTimeTxt.Content = mediaElement.Position.ToString(@"hh\:mm\:ss");
        }
        private void Button_Search(object sender, RoutedEventArgs e)
        {
            Translate();

        }
        private void Translate()
        {
            var txt = captiontxt.SelectedText;
            Translator translator = new Translator(languageCollection, _language, _video, txt);
            translator.Show();
        }
        private void Button_CaptonOnOffC(object sender, RoutedEventArgs e)
        {
            if (captionBool)
            {
                captionBool = false;
                captionbtn.Content = "Subtitles Off...";
                timer.Tick += timer_Tick;
            }
            else {
                captionBool = true;
                captionbtn.Content = "Subtitles On...";
                timer.Tick += timer_Tick; 
            }
        
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaElement.Position = TimeSpan.FromSeconds(slider.Value);
        }

        private void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            beginTimeTxt.Content = mediaElement.Position.ToString(@"hh\:mm\:ss");
            endTimeTxt.Content = mediaElement.NaturalDuration.TimeSpan.ToString();
            slider.Maximum= mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
            slider.Value = mediaElement.Position.TotalSeconds;
            timer.Start();
        }

        private void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            timer.Stop();
        }
        private List<SubtitlesParser.Classes.SubtitleItem> GetCaptions()
        {
            if (_language != null)
            {
                string filePath = System.IO.Path.Combine(_video.CaptionFolderPath, _language.Code + ".srt");

                if (File.Exists(filePath))
                {
                    var parser = new SubtitlesParser.Classes.Parsers.SrtParser();
                    using (var fileStream = File.OpenRead(filePath))
                    {
                        var items = parser.ParseStream(fileStream, Encoding.Default);
                        return items;
                    }

                }
            }
            return null;
        }

        private void language_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _language = (Models.Language)language.SelectedItem;
            subtitles = GetCaptions();
        }

        private void fontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            captiontxt.FontSize = (int)fontSize.SelectedItem;
            
        }

        private void Button_Words(object sender, RoutedEventArgs e)
        {
            var words = new FunnyLanguage_WPF.Words(_video.VideoId);
            words.Show();
        }


        private void captiontxt_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Translate();
        }
    }
}
