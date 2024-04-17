using FunnyLanguage_WPF.Models;
using FunnyLanguage_WPF.Services;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
    /// Interaction logic for Words.xaml
    /// </summary>
    public partial class Words : Window
    {
        private Models.Language language1 = new Models.Language()
        {
            Name = "",
            Code = ""
        };
        private Models.Language language2 = new Models.Language()
        {
            Name = "",
            Code = ""
        };
        private int? _videoId;
        public ObservableCollection<Models.Word> words;
        private string knowit = "";
        private TextBox? statistic1xtb;
        private TextBox? statistic2xtb;
        public Words(int? videoId = null, TextBox? statistic1xtbl = null, TextBox? statistic2xtbl = null)
        {
            InitializeComponent();
            _videoId = videoId;
            using var db = new VideoContext();
            if (videoId != null)
            {
                var wordList = db.WordLists.Where(x => x.VideoId == videoId).FirstOrDefault();
                if (wordList != null)
                {
                    words = new ObservableCollection<Models.Word>(db.Words.Where(x => x.WordlistId == wordList.WordListId).ToList());
                }
            } else
            {
                words = new ObservableCollection<Models.Word>(db.Words.ToList());
            }
            if (words != null)
            {
                listview.ItemsSource = words;
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
                var l0 = new Models.Language()
                {
                    Name = "",
                    Code = ""
                };
                languages.Add(l0);
                languages2.Add(l0);
                knowIt.ItemsSource = new string[] { "", "Know it", "Don't know" };
                knowIt.SelectedIndex = 0;
                languageOriginal.ItemsSource = languages.OrderBy(x => x.Name).ToList();
                languageOriginal.DisplayMemberPath = "Name";
                languageOriginal.SelectedItem = l0;
                languageTranslated.ItemsSource = languages2.OrderBy(x => x.Name).ToList();
                languageTranslated.DisplayMemberPath = "Name";
                languageTranslated.SelectedItem = l0;
                listview.SelectedIndex= 0;
                listview.SelectedItem= null;
            } else { MessageBox.Show("There are no saved words for this video!!!"); }
            statistic1xtb = statistic1xtbl;
            statistic2xtb = statistic2xtbl;
        }

        private void languageOriginal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            language1 = (Models.Language)languageOriginal.SelectedItem;
            Load();
          
        }

        private void languageTranslated_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           language2 = (Models.Language)languageTranslated.SelectedItem;
            Load();
        }
        private void Load()
        {
            using var db = new VideoContext();
            if (_videoId != null)
            {
                var wordList = db.WordLists.Where(x => x.VideoId == _videoId).FirstOrDefault();
                if (wordList != null)
                {
                    if (!language1.Name.Equals("") && !language2.Name.Equals("") && knowit.Equals(""))
                    {
                        words = new ObservableCollection<Models.Word>(db.Words.Where(x => x.WordlistId == wordList.WordListId && x.FirstLanguage == language1.Code && x.SecondLanguage == language2.Code).ToList());

                    }
                    else if (language1.Name.Equals("") && !language2.Name.Equals("") && knowit.Equals(""))
                    {
                        words = new ObservableCollection<Models.Word>(db.Words.Where(x => x.WordlistId == wordList.WordListId && x.SecondLanguage == language2.Code).ToList());
                    }
                    else if (!language1.Name.Equals("") && language2.Name.Equals("") && knowit.Equals(""))
                    {
                        words = new ObservableCollection<Models.Word>(db.Words.Where(x => x.WordlistId == wordList.WordListId && x.FirstLanguage == language1.Code).ToList());
                    }
                    else if (!language1.Name.Equals("") && !language2.Name.Equals("") && !knowit.Equals(""))
                    {
                        words = new ObservableCollection<Models.Word>(db.Words.Where(x => x.WordlistId == wordList.WordListId && x.FirstLanguage == language1.Code && x.SecondLanguage == language2.Code && x.KnowIt.Equals(knowit)).ToList());

                    }
                    else if (language1.Name.Equals("") && !language2.Name.Equals("") && !knowit.Equals(""))
                    {
                        words = new ObservableCollection<Models.Word>(db.Words.Where(x => x.WordlistId == wordList.WordListId && x.SecondLanguage == language2.Code && x.KnowIt.Equals(knowit)).ToList());
                    }
                    else if (!language1.Name.Equals("") && language2.Name.Equals("") && !knowit.Equals(""))
                    {
                        words = new ObservableCollection<Models.Word>(db.Words.Where(x => x.WordlistId == wordList.WordListId && x.FirstLanguage == language1.Code && x.KnowIt.Equals(knowit)).ToList());
                    }
                    else if (language1.Name.Equals("") && language2.Name.Equals("") && !knowit.Equals(""))
                    {
                        words = new ObservableCollection<Models.Word>(db.Words.Where(x => x.WordlistId == wordList.WordListId && x.KnowIt.Equals(knowit)).ToList());
                    }
                    else
                    {
                        words = new ObservableCollection<Models.Word>(db.Words.Where(x => x.WordlistId == wordList.WordListId).ToList());
                    }


                    listview.ItemsSource = words;

                }
            } else
            {
                if (!language1.Name.Equals("") && !language2.Name.Equals("") && knowit.Equals(""))
                {
                    words = new ObservableCollection<Models.Word>(db.Words.Where(x => x.FirstLanguage == language1.Code && x.SecondLanguage == language2.Code).ToList());

                }
                else if (language1.Name.Equals("") && !language2.Name.Equals("") && knowit.Equals(""))
                {
                    words = new ObservableCollection<Models.Word>(db.Words.Where(x => x.SecondLanguage == language2.Code).ToList());
                }
                else if (!language1.Name.Equals("") && language2.Name.Equals("") && knowit.Equals(""))
                {
                    words = new ObservableCollection<Models.Word>(db.Words.Where(x =>  x.FirstLanguage == language1.Code).ToList());
                }
                else if (!language1.Name.Equals("") && !language2.Name.Equals("") && !knowit.Equals(""))
                {
                    words = new ObservableCollection<Models.Word>(db.Words.Where(x =>  x.FirstLanguage == language1.Code && x.SecondLanguage == language2.Code && x.KnowIt.Equals(knowit)).ToList());

                }
                else if (language1.Name.Equals("") && !language2.Name.Equals("") && !knowit.Equals(""))
                {
                    words = new ObservableCollection<Models.Word>(db.Words.Where(x =>  x.SecondLanguage == language2.Code && x.KnowIt.Equals(knowit)).ToList());
                }
                else if (!language1.Name.Equals("") && language2.Name.Equals("") && !knowit.Equals(""))
                {
                    words = new ObservableCollection<Models.Word>(db.Words.Where(x => x.FirstLanguage == language1.Code && x.KnowIt.Equals(knowit)).ToList());
                }
                else if (language1.Name.Equals("") && language2.Name.Equals("") && !knowit.Equals(""))
                {
                    words = new ObservableCollection<Models.Word>(db.Words.Where(x =>  x.KnowIt.Equals(knowit)).ToList());
                }
                else
                {
                    words = new ObservableCollection<Models.Word>(db.Words.ToList());
                }


                listview.ItemsSource = words;
            }
        }
        private void UpateTextInList()
        {
            if (statistic1xtb != null && statistic2xtb != null)
            {
                using var db = new VideoContext();
                var words2 = db.Words.Where(x => x.KnowIt.Equals("Know it")).ToList();
                if (words2 != null)
                {
                    statistic1xtb.Text = "Total number of learned words and phrases: " + words2.Count;
                }
                words2 = db.Words.Where(x => x.KnowIt.Equals("Know it") && x.LastTime.Date == DateTime.Now.Date).ToList();
                if (words2 != null)
                {
                    statistic2xtb.Text = "Total number of learned words and phrases for today: " + words2.Count;
                }
            }
        }

        private void knowIt_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            knowit = (string)knowIt.SelectedItem;
            Load();
        }

        private void Button_KnowIt(object sender, RoutedEventArgs e)
        {
            var w = (Models.Word)listview.SelectedItem;
            if (w != null)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you know this word and do not want to play with it?", "Confirmation", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    using var db = new VideoContext();


                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var word = db.Words.Where(x => x.WordId == w.WordId).FirstOrDefault();
                            if (word != null)
                            {
                                word.KnowIt = "Know it";
                                word.LastTime = DateTime.Now;
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
                    UpateTextInList();
                    Load();
                }
            }
        }

        private void Button_Delete(object sender, RoutedEventArgs e)
        {
            using var db = new VideoContext();
            var w = (Models.Word)listview.SelectedItem;
            if (w != null)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this word?", "Confirmation", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var word = db.Words.Where(x => x.WordId == w.WordId).FirstOrDefault();
                            if (word != null)
                            {
                                db.Remove(word);
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
                    UpateTextInList();
                    Load();
                }
            }
        }

        private void Button_LearnAgain(object sender, RoutedEventArgs e)
        {
            using var db = new VideoContext();
            var w = (Models.Word)listview.SelectedItem;
            if (w != null)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to play with this word again and reset the score?", "Confirmation", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var word = db.Words.Where(x => x.WordId == w.WordId).FirstOrDefault();
                            if (word != null)
                            {
                                word.SuccessRate = 0;
                                word.FailureRate = 0;
                                word.LastTrySuccess = "Failure";
                                word.KnewButHadRemember = 0;
                                word.LastTime = DateTime.Now;
                                word.AddedToListTime = DateTime.Now;
                                word.KnowIt = "Don't know";
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
                    UpateTextInList();
                    Load();
                }
            }
        }

        private void listview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (listview.SelectedItems.Count == 0)
            {
                knowitbtn.IsEnabled = false;
                deletebtn.IsEnabled = false;
                learnagainbtn.IsEnabled = false;
               
            }
            else
            {
                var word = (Models.Word)listview.SelectedItem;
                if (word != null)
                {
                    deletebtn.IsEnabled = true;
                    if (word.KnowIt.Equals("Know it"))
                    {
                        knowitbtn.IsEnabled = false;
                        learnagainbtn.IsEnabled = true;
                    }
                    else
                    {
                        knowitbtn.IsEnabled = true;
                        learnagainbtn.IsEnabled = false;
                    }
                }
                else
                {
                    knowitbtn.IsEnabled = false;
                    deletebtn.IsEnabled = false;
                    learnagainbtn.IsEnabled = false;

                }
                
            }
        }
    }
  
}
