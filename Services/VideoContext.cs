using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using FunnyLanguage_WPF.Models;
namespace FunnyLanguage_WPF.Services
{
    public class VideoContext : DbContext
    {

        public DbSet<Video> Videos { get; set; }
        public DbSet<WordList> WordLists { get; set; }
        public DbSet<Word> Words { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Video_Language> VideoLanguages { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string appDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string mainFolderName = "FunnyLanguageApp_data";
            string mainFolderPath = System.IO.Path.Combine(appDataFolderPath, mainFolderName);
            if (!Directory.Exists(mainFolderPath))
            {
                Directory.CreateDirectory(mainFolderPath);
            }
            string folderName = "Database";
            string path = System.IO.Path.Combine(mainFolderPath, folderName);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var dbPath = System.IO.Path.Join(path, "videosDB.db");
            optionsBuilder
                .UseSqlite($"Data Source={dbPath}");
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Language>()
                      .HasIndex(l => l.Code)
                      .IsUnique();
            modelBuilder.Entity<Video_Language>()
                        .HasKey(m => new { m.VideoId, m.LanguageCode });
        }
         
    }
}

