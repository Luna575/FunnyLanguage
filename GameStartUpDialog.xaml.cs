using FunnyLanguage_WPF.Models;
using FunnyLanguage_WPF.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using YoutubeExplode.Videos;

namespace FunnyLanguage_WPF
{
    /// <summary>
    /// Interaction logic for GameStartUpDialog.xaml
    /// </summary>
    public partial class GameStartUpDialog : Window
    {
        public ObservableCollection<Models.Word> words;
        public GameStartUpDialog(int videoId)
        {
            InitializeComponent();
            using var db = new VideoContext();
            var wordList = db.WordLists.Where(x => x.VideoId == videoId).FirstOrDefault();
            if (wordList != null)
            {
                words = new ObservableCollection<Models.Word>(db.Words.Where(x => x.WordlistId == wordList.WordListId).ToList());
                var languages = new List<Models.Language>();
                var languages2 = new List<Models.Language>();
                foreach (var word in words)
                {

                    var l = db.Languages.Where(x => x.Code.Equals(word.FirstLanguage)).First();
                    var l2 = db.Languages.Where(x => x.Code.Equals(word.SecondLanguage)).First();
                    if (!languages.Contains(l))
                    {
                        languages.Add(l);
                    }
                    if (!languages2.Contains(l2))
                    {
                        languages2.Add(l2);
                    }

                }

                mode.ItemsSource = new string[] { "Random 20 words", "All words" };
                mode.SelectedIndex = 0;
                language1.ItemsSource = languages.OrderBy(x => x.Name).ToList();
                language1.DisplayMemberPath = "Name";
                language1.SelectedIndex= 0;
                language2.ItemsSource = languages2.OrderBy(x => x.Name).ToList();
                language2.DisplayMemberPath = "Name";
                language2.SelectedIndex= 0;
            }
            else 
            {
                MessageBox.Show("There are no words to play with!!!");
            }
        }
        private void Button_Play(object sender, RoutedEventArgs e)
        {

            DialogResult = true;
            this.Close();
        }
        private void Button_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
