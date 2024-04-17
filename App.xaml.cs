using FunnyLanguage_WPF.Services;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Data;
using System.Windows;

namespace FunnyLanguage_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        
        App() {
            using (var dbContext = new VideoContext()) 
            {
                dbContext.Database.EnsureCreated();
                dbContext.Dispose();
            }
        }
              
    }
    

}
