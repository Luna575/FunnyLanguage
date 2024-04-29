using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
        /// inšpirované zo stránky: https://github.com/ssut/py-googletrans/issues/268 a tiež zo stránky https://stackoverflow.com/questions/70185058/how-to-replace-obsolete-webclient-with-httpclient-in-net-6
        /// </summary>
        /// <param name="text"></param>
        /// <param name="inputLanguage"></param>
        /// <param name="targetlanguage"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public string TranslateTextAsync(string? text, string? inputLanguage, string? targetlanguage)
        {
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException(nameof(text));
            if (string.IsNullOrEmpty(inputLanguage))
                throw new ArgumentNullException(nameof(inputLanguage));
            if (string.IsNullOrEmpty(targetlanguage))
                throw new ArgumentNullException(nameof(targetlanguage));
            string url = $"{baseUrl}{inputLanguage}&tl={targetlanguage}&dt=t&q={HttpUtility.UrlEncode(text)}";
            using var httpClient = new HttpClient();
           
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                var response = httpClient.Send(request);
                using var reader = new StreamReader(response.Content.ReadAsStream());
                var responseBody = reader.ReadToEnd();
                if (!string.IsNullOrEmpty(responseBody))
                {
                    responseBody = responseBody.Substring(4, responseBody.IndexOf("\"", 4, StringComparison.Ordinal) - 4);
                    return responseBody;
                }
                
               return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }


    }
   

}
