using FunnyLanguage_WPF.Models;
using FunnyLanguage_WPF.Services;
using Microsoft.EntityFrameworkCore.Storage;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FunnyLanguage_WPF
{
    /// <summary>
    /// Interaction logic for PlayQuickShowMemmoryBackwards.xaml
    /// </summary>
    public partial class PlayQuickShowMemmoryBackwards : Window
    {
        private Models.Video _video;
        private List<Models.Word> _words = new List<Models.Word>();
        private DispatcherTimer timer;
        private int number = 0;
        private int result = 0;
        public PlayQuickShowMemmoryBackwards(Models.Video video)
        {
            InitializeComponent();
            _video = video;
            Closing += Action_Closing;
        }
        private void Action_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (timer != null) { timer.Stop(); }
        }
        private void StartUP()
        {
            var startUp = new GameStartUpDialog(_video.VideoId);
            if (startUp.ShowDialog() == true)
            {
                GetRandomWords(startUp.language1, startUp.language2, startUp.mode);
                if (_words.Count > 0)
                {
                    timer = new DispatcherTimer();
                    timer.Interval = TimeSpan.FromSeconds(10);
                    timer.Tick += new EventHandler(timer_Tick);
                    var txt = "You have 10 seconds. Try to remember the translation of:  ";
                    wordNtxtbl.Text = txt;
                    wordtxtbl.Text = _words[number].TranslatedText;
                    timer.Start();
                    playbtn.Visibility = Visibility.Collapsed;
                }
                else 
                {
                    MessageBox.Show("There are no words to play with!!!");
                    this.Close();
                }
            }
            else
            {
                this.Close();
            }
        }
        private void timer_Tick(object? sender, EventArgs e)
        {
            timer.Stop();
            if (timer.Interval == TimeSpan.FromSeconds(10))
            {
                var txt = "You have 30 seconds, try to translate this: ";
                wordNtxtbl.Text = txt;
                wordtxtbl.Text = _words[number].TranslatedText;
                entertxtbl.Visibility = Visibility.Visible;
                checktbtn.Visibility = Visibility.Visible;
                timer.Interval = TimeSpan.FromSeconds(30);
                timer.Start();
            }
            else
            {
                Check();
                checktbtn.Visibility= Visibility.Collapsed;
            }

        }

        private void GetRandomWords(ComboBox language1, ComboBox language2, ComboBox mode)
        {
            using var db = new VideoContext();
            var l1 = (Models.Language)language1.SelectedItem;
            var l2 = (Models.Language)language2.SelectedItem;
            var m = (string)mode.SelectedItem;
            var wordlist = db.WordLists.Where(x => x.VideoId == _video.VideoId).FirstOrDefault();
            var w = new List<Models.Word>();
            if (wordlist != null) 
            { 
                w = new List<Models.Word>(db.Words.Where(x => x.WordlistId == wordlist.WordListId && x.FirstLanguage.Equals(l1.Code) && x.SecondLanguage.Equals(l2.Code) && x.KnowIt.Equals("Don't know")));
            }
            else {  w = new List<Models.Word>(db.Words.Where(x => x.FirstLanguage.Equals(l1.Code) && x.SecondLanguage.Equals(l2.Code) && x.KnowIt.Equals("Don't know"))); }
            var rand = new Random();
            if (w.Count > 0)
            {
                if (mode.Equals("Random 20 words") && w.Count >= 20)
                {

                    for (int i = 0; i < 20; i++)
                    {

                        int index = rand.Next(w.Count);
                        _words.Add(w[index]);
                        w.RemoveAt(index);

                    }
                }
                else
                {
                    var count = w.Count;

                    for (int i = 0; i < count; i++)
                    {

                        int index = rand.Next(w.Count);
                        _words.Add(w[index]);
                        w.RemoveAt(index);


                    }


                }
            }
           
        }

        private void checktbtn_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            Check();
            checktbtn.Visibility = Visibility.Collapsed;
        }
        private void Check()
        {
            TextChecker textChecker = new TextChecker();
            if (textChecker.CheckText(entertxtbl.Text, _words[number].OriginalText))
            {
                resultlb.Content = "Success! Great work!";
                result++;
                SetResultForWord(true);

            }
            else
            {
                resultlb.Content = "Sorry, but the answer should have been this: " + _words[number].OriginalText;

                SetResultForWord(false);
            }
            if (number == _words.Count - 1)
            {
                MessageBox.Show("Your success is: " + result + " out of " + (number + 1) + "!!!");
                CheckKnowIt();
                this.Close();
            }
            else
            {
                nextbtn.Visibility = Visibility.Visible;
            }
        }

        private void nextbtn_Click(object sender, RoutedEventArgs e)
        {
            number++;
            var txt = "You have 10 seconds. Try to remember the translation of: ";
            wordNtxtbl.Text = txt;
            wordtxtbl.Text = _words[number].TranslatedText;
            timer.Interval = TimeSpan.FromSeconds(10);
            timer.Start();
            entertxtbl.Visibility = Visibility.Collapsed;
            resultlb.Content = "";
            entertxtbl.Text = "";
            nextbtn.Visibility = Visibility.Collapsed;
            checktbtn.Visibility = Visibility.Collapsed;
        }
        private void SetResultForWord(bool x)
        {
            using var db = new VideoContext();
            if (x)
            {
                var w = db.Words.Where(x => x.WordId == _words[number].WordId).First();
                w.SuccessRate += 1;
                w.LastTrySuccess = "Success";
                w.LastTime = DateTime.Now;
            }
            else
            {
                var w = db.Words.Where(x => x.WordId == _words[number].WordId).First();
                w.FailureRate += 1;
                w.LastTrySuccess = "Failure";
                w.LastTime = DateTime.Now;
            }
            db.SaveChanges();
        }
        private void CheckKnowIt()
        {
            using var db = new VideoContext();
            var w = new List<Models.Word>(db.Words.Where(x => x.SuccessRate - x.FailureRate >= 15));
            if (w.Any())
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var word in w)
                        {
                            if (word != null)
                            {
                                word.KnowIt = "Know it";
                            }
                        }
                        db.SaveChangesAsync();

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
        }

        private void playbtn_Click(object sender, RoutedEventArgs e)
        {
            StartUP();
        }
    }
}
