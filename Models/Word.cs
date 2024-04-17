using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FunnyLanguage_WPF.Models
{

    public class Word
    {

        public int WordId { get; set; }
        public int WordlistId { get; set; }
        public string FirstLanguage { get; set; }
        public string SecondLanguage { get; set; }
        public string OriginalText { get; set; }
        public string TranslatedText { get; set; }
        public int SuccessRate { get; set; }  = 0;
        public string LastTrySuccess { get; set; } = "Failure";
        public int FailureRate { get; set;} = 0;
        public int KnewButHadRemember { get; set; } = 0;
        public DateTime LastTime { get; set; } = DateTime.Now;
        public DateTime AddedToListTime {  get; set; } = DateTime.Now;
        public string KnowIt { get; set; } = "Don't know";

    }
}
