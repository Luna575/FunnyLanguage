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

namespace FunnyLanguage_WPF
{
    /// <summary>
    /// Interaction logic for Games.xaml
    /// </summary>
    public partial class Games : Page
    {
        private Models.Video _video;
        public Games(Models.Video video)
        {
            InitializeComponent();
            _video = video;
        }

        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var game = new PlayQuickShowMemmory(_video);
            try {
                game.Show();
            } catch (Exception)
            {
                MessageBox.Show("Game was closed!!!");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var game = new PlayQuickShowMemmoryBackwards(_video);
            try
            {
                game.Show();
            }
            catch (Exception)
            {
                MessageBox.Show("Game was closed!!!");
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var game = new MemmoryCards(_video);
            try
            {
                game.Show();
            }
            catch (Exception)
            {
                MessageBox.Show("Game was closed!!!");
            }
        }
    }
}
