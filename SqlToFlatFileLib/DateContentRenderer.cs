using System;
using System.Text;
using System.Text.RegularExpressions;

namespace SqlToFlatFileLib
{
    public static class DateContentRenderer
    {            
        private static string _defaultDateFormat = "yyyyMMdd";

        /// <summary>
        /// Substitutes the tag with a date/time string
        /// </summary>
        /// <param name="input">the entire string that includes the date tag</param>
        /// <param name="metaTag">The name of the tag, for example:   this is today's date {currentDate:format=yyMMdd} </param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string Render(string input, string metaTag, DateTime date)
        {           
            StringBuilder buildOutput = new StringBuilder(input);

            var regex = new Regex(@"\{"+ metaTag + @".*?\}", RegexOptions.Compiled);

            var matches = regex.Matches(input);
            for (int i = matches.Count - 1; i > -1; i-- )
            {
                var dateFormat = _defaultDateFormat;
                var match = matches[i];
                int posFormat =  match.Value.IndexOf(":format=", StringComparison.Ordinal);
                if (posFormat > 0)
                {
                    dateFormat = match.Value.Substring(posFormat + 8,
                        (match.Length - 1  - (posFormat + 8)));
                }
                buildOutput.Remove(match.Index, match.Length);
                buildOutput.Insert(match.Index, date.ToString(dateFormat));
            }
            return buildOutput.ToString();
        }
    }
}
