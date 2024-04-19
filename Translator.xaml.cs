using AngleSharp.Dom;
using FunnyLanguage_WPF.Models;
using FunnyLanguage_WPF.Services;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.ClosedCaptions;
using static System.Net.Mime.MediaTypeNames;

namespace FunnyLanguage_WPF
{
    /// <summary>
    /// Interaction logic for Translator.xaml
    /// </summary>
    public partial class Translator : Window
    {
        private static Translator instance;
        private TranslatorClass translator = new TranslatorClass();
        private ObservableCollection<Models.Language> languageCollection;
        public ObservableCollection<Models.Language> languageCollectionVideo;
        private Models.Video _video;
        
        public Translator(ObservableCollection<Models.Language> languages, Models.Language language, Models.Video video, string? txt)
        {
            InitializeComponent();
            _video = video;
            AddLanguage();
            using var db = new VideoContext(); 
            languageCollection = new ObservableCollection<Models.Language>(db.Languages.OrderBy(x => x.Name).ToList());
            languageCollectionVideo = languages;
            language1.ItemsSource = languageCollectionVideo;
            language1.DisplayMemberPath = "Name";
            language1.SelectedItem = language;
            language2.ItemsSource = languageCollection;
            language2.DisplayMemberPath = "Name";
            language2.Text= "Slovak";
            if ( !string.IsNullOrEmpty(txt)) { textToTranslate.Text = txt.Trim(); }

        }
        /// <summary>
        /// Napísané s pomocou CHATGPT
        /// </summary>
        /// <param name="languages"></param>
        /// <param name="language"></param>
        /// <param name="video"></param>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static Translator GetInstance(ObservableCollection<Models.Language> languages, Models.Language language, Models.Video video, string? txt)
        {
            if(instance == null || !instance.IsVisible)
            {
                instance = new Translator(languages, language, video, txt);
            }
            else
            {
               instance.language1.SelectedItem = language;
               if (!string.IsNullOrEmpty(txt)) { instance.textToTranslate.Text = txt.Trim(); }
            }
            return instance;
        }
      

        private async void Button_Translate(object sender, RoutedEventArgs e)
        {
            translatedText.Text = "";
            Models.Language l1 = (Models.Language)language1.SelectedItem;
            Models.Language l2 = (Models.Language)language2.SelectedItem;
            string code1 = l1.Code;
            string code2 = l2.Code;
            if (InternetConnection())
            {
                try
                {
                    if (code1 != null && code2 != null)
                    {

                        if (textToTranslate != null)
                        {
                            try
                            {
                                string text = await translator.TranslateTextAsync(textToTranslate.Text, code1, code2);
                                translatedText.Text = text;
                            }
                            catch (Exception ex)
                            {
                                throw;
                            }


                        }
                        else
                        {
                            translatedText.Text = "No text to translate!!!";
                            return;
                        }
                    }
                    else
                    {
                        translatedText.Text = "Both languages need to be selected!!!";
                        return;
                    }
                }
                catch (Exception ex)
                {
                    translatedText.Text = "Sorry something went wrong. Maybe you reached the daily limit for the translation!!!";
                }
            }
            else
            {
                MessageBox.Show("Please connect to the Internet or check your Internet connection!!!");
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
            using (Ping ping = new Ping())
            {
                
                int timeout = 1000;
                try
                {
                    PingReply reply = ping.Send(hostNameOrAddress, timeout); 
                    pingStatus = (reply.Status == IPStatus.Success);
                }
                catch (Exception)
                {
                    pingStatus = false;
                }
                return pingStatus;
            }

        }
        private void AddLanguage()
        {
            var language = new Models.Language();
            language.Name = "Slovak";
            language.Code = "sk";
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
        }
        private void Button_Save(object sender, RoutedEventArgs e)
        {
            if (translatedText.Text.Length > 0 && !textToTranslate.Text.TrimStart().Equals("") && textToTranslate.Text.Length > 0)
            {
                Models.WordList wordList = new Models.WordList(); 

                var error = 0;
                using (var videoContext = new VideoContext())
                {
                    using (var transaction = videoContext.Database.BeginTransaction())
                    {
                        try
                        {

                            var existingList = videoContext.WordLists.Find(_video.VideoId);
                            if (existingList == null)
                            {
                                
                                wordList.VideoId = _video.VideoId;
                                videoContext.Add(wordList);
                            }
                            else { wordList = existingList; }


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
                            error += 1;
                        }
                    }
                    if (error == 0)
                    {
                        var l1 = (Models.Language)language1.SelectedItem;
                        var l2 = (Models.Language)language2.SelectedItem;
                        var checker = new TextChecker();
                        var originaltxt = checker.NormalizeText(textToTranslate.Text.Trim());
                        var translatedtxt = checker.NormalizeText(translatedText.Text.Trim());
                        Models.Word word = new Models.Word()
                        {
                            FirstLanguage = l1.Code,
                            SecondLanguage = l2.Code,
                            OriginalText = originaltxt,
                            TranslatedText = translatedtxt,
                            WordlistId = videoContext.WordLists.First(x => x.WordListId == wordList.WordListId).WordListId
                        };
                        using (var transaction = videoContext.Database.BeginTransaction())
                        {

                            try
                            {

                                var existingWord = videoContext.Words.Where(x => x.WordlistId == wordList.WordListId && x.OriginalText.ToLower().Equals(originaltxt.ToLower()) && x.TranslatedText.ToLower().Equals(translatedtxt.ToLower())).FirstOrDefault();
                                if (existingWord == null)
                                {

                                    videoContext.Add(word);
                                    videoContext.SaveChanges();
                                    videoContext.WordLists.Where(x => x.VideoId == wordList.VideoId).First().Words.Add(word);
                                    videoContext.SaveChanges();
                                    transaction.Commit();
                                    if (!Words.IsInstanceNull())
                                    {
                                        var words = Words.GetInstance(_video.VideoId);
                                        words.Show();
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Text is already in the list!!!");
                                    error += 1;
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
                                error += 1;
                            }
                        }
                    }
                }
                if (error == 0) { MessageBox.Show("The text has been successfully saved!"); }
            }
            else
            {
                MessageBox.Show("You need to have original and translated text filled!");
            }
        }

        private void Button_Words(object sender, RoutedEventArgs e)
        {
            var words = Words.GetInstance(_video.VideoId);
            words.Show();
        }

        private void textToTranslate_TextChanged(object sender, TextChangedEventArgs e)
        {
            translatedText.Text = "";
        }
    }
}
