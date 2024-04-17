using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunnyLanguage_WPF.Models
{ 
    public class WordList
    {
        public int WordListId {  get; set; }
        public List<Word> Words { get; set; } = new List<Word>();
        public int VideoId { get; set; }

        public Video Video { get; set; }
    }
}
