using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunnyLanguage_WPF.Models
{
    public class Language
    {
        [Key]
        public string Code { get; set; }

        public string Name { get; set; }
    }
}
