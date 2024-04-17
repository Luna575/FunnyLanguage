using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunnyLanguage_WPF.Models
{
    public class Video
    {
        public int VideoId { get; set; }
        public string Title { get; set; }
        public string NameForDownload { get; set; }
        public string VideoCode { get; set; }
        public string Url { get; set; }
        public string FolderPath { get; set; }
        public string CaptionFolderPath { get; set; }
        // Foreign key property for Language 
        public List<string> LanguageCodes { get; set; }

        public bool IsVideoDeleted { get; set; } = false;
    }
    
}
