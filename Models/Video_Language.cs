using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunnyLanguage_WPF.Models
{
    public class Video_Language
    {
        public int VideoId { get; set; }
        public Video Video { get; set; }
        public string LanguageCode { get; set; }
        public Language Language { get; set; }
    }
}
