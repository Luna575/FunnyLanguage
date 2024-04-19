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
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Windows.Threading;
using FunnyLanguage_WPF.Services;
using Microsoft.EntityFrameworkCore.Storage;

namespace FunnyLanguage_WPF
{
    /// <summary>
    /// Interaction logic for MemmoryCards.xaml
    /// </summary>
    public partial class MemmoryCards : Window
    {
        private Models.Video _video;
        private List<Models.Word> _words = new List<Models.Word>();
        private int number = 0;
        public MemmoryCards(Models.Video video)
        {
            InitializeComponent();
            _video = video;
        }
        private void StartUP()
        {
            var startUp = new GameStartUpDialog(_video.VideoId);
            if (startUp.ShowDialog() == true)
            {
                GetRandomWords(startUp.language1, startUp.language2, startUp.mode);
                var txt = "Do you know this: ";
                wordNtxtbl.Text = txt;
                wordtxtbl.Text = _words[number].OriginalText;
                playbtn.Visibility = Visibility.Collapsed;
                dontknowbtn.Visibility = Visibility.Visible;
                knowbtn.Visibility = Visibility.Visible;
                littleknowbtn.Visibility = Visibility.Visible;
            }
            else
            {
                this.Close();
            }
        }
        private void GetRandomWords(ComboBox language1, ComboBox language2, ComboBox mode)
        {
            using var db = new VideoContext();
            var l1 = (Models.Language)language1.SelectedItem;
            var l2 = (Models.Language)language2.SelectedItem;
            var m = (string)mode.SelectedItem;
            var w = new List<Models.Word>(db.Words.Where(x => x.FirstLanguage.Equals(l1.Code) && x.SecondLanguage.Equals(l2.Code) && x.KnowIt.Equals("Don't know")));
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

                    for (int i = 0; i < w.Count; i++)
                    {

                        int index = rand.Next(w.Count);
                        _words.Add(w[index]);
                        w.RemoveAt(index);


                    }


                }
            }
        }
        private void playbtn_Click(object sender, RoutedEventArgs e)
        {
            StartUP();
        }

        private void nextbtn_Click(object sender, RoutedEventArgs e)
        {
            if (number == _words.Count - 1)
            {
                MessageBox.Show("There are no more words!!!");
                CheckKnowIt();
                this.Close();
            }
            else
            {
                number++;
                var txt = "Do you know this: ";
                wordNtxtbl.Text = txt;
                wordtxtbl.Text = _words[number].OriginalText;
                answertxtbl.Text = "";
                answertxtbl.Visibility = Visibility.Collapsed;
                nextbtn.Visibility = Visibility.Collapsed;
                dontknowbtn.Visibility = Visibility.Visible;
                knowbtn.Visibility = Visibility.Visible;
                littleknowbtn.Visibility = Visibility.Visible;
            }
            
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

        private void knowbtn_Click(object sender, RoutedEventArgs e)
        {
            using var db = new VideoContext();

            var w = db.Words.Where(x => x.WordId == _words[number].WordId).First();
            w.SuccessRate += 1;
            w.LastTrySuccess = "Success";
            w.LastTime = DateTime.Now;
            db.SaveChanges();
            nextbtn.Visibility = Visibility.Visible;
            dontknowbtn.Visibility = Visibility.Collapsed;
            knowbtn.Visibility = Visibility.Collapsed;
            littleknowbtn.Visibility = Visibility.Collapsed;
        }

        private void dontknowbtn_Click(object sender, RoutedEventArgs e)
        {
            using var db = new VideoContext();

            var w = db.Words.Where(x => x.WordId == _words[number].WordId).First();
            w.FailureRate += 1;
            w.LastTrySuccess = "Failure";
            w.LastTime = DateTime.Now;

            db.SaveChanges();
            nextbtn.Visibility = Visibility.Visible;
            answertxtbl.Visibility = Visibility.Visible;
            answertxtbl.Text = _words[number].TranslatedText;
            dontknowbtn.Visibility = Visibility.Collapsed;
            knowbtn.Visibility = Visibility.Collapsed;
            littleknowbtn.Visibility = Visibility.Collapsed;

        }

        private void littleknowbtn_Click(object sender, RoutedEventArgs e)
        {
            using var db = new VideoContext();

            var w = db.Words.Where(x => x.WordId == _words[number].WordId).First();
            w.KnewButHadRemember += 1;
            w.LastTrySuccess = "Success";
            w.LastTime = DateTime.Now;

            db.SaveChanges();
            nextbtn.Visibility = Visibility.Visible;
            dontknowbtn.Visibility = Visibility.Collapsed;
            knowbtn.Visibility = Visibility.Collapsed;
            littleknowbtn.Visibility = Visibility.Collapsed;
        }
    }
}
