using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace FunnyLanguage_WPF
{
    internal class TranslatorClass
    {
        private const string baseUrl = " https://translate.googleapis.com/translate_a/single?client=gtx&sl=";
        public TranslatorClass()
        {
            
        }
        /// <summary>
        /// inšpirované zo stránky: https://github.com/ssut/py-googletrans/issues/268
        /// </summary>
        /// <param name="text"></param>
        /// <param name="inputLanguage"></param>
        /// <param name="targetlanguage"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<string> TranslateTextAsync(string text, string inputLanguage, string targetlanguage)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException(nameof(text));
            if (string.IsNullOrEmpty(inputLanguage))
                throw new ArgumentNullException(nameof(inputLanguage));
            if (string.IsNullOrEmpty(targetlanguage))
                throw new ArgumentNullException(nameof(targetlanguage));
            string url = $"{baseUrl}{inputLanguage}&tl={targetlanguage}&dt=t&q={HttpUtility.UrlEncode(text)}";
            var webclient = new WebClient
            {
                Encoding = System.Text.Encoding.UTF8
            };
            var result = webclient.DownloadString(url) ;
            try
            {
                result = result.Substring(4, result.IndexOf("\"", 4, StringComparison.Ordinal) - 4);
               return result ;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }

        }


    }
   

}
