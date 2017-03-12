using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RandomUtilities
{
    public static class RegexMatcher
    {
        public static List<string> FindMatches(string input, Regex regex)
        {
            var matchesCollection = regex.Matches(input);
            var matches = new List<string>();

            foreach (var match in matchesCollection)
            {
                matches.Add(match.ToString());
            }

            return matches;
        }

        public static string FindMatches2(string input, Regex regex)
        {
            var matchesCollection = regex.Matches(input);
            StringBuilder sb = new StringBuilder();

            foreach (var match in matchesCollection)
            {
                sb.Append(match.ToString());
            }

            return sb.ToString();
        }

        public static List<string> FindMatches(string input, string pattern)
        {
            var regex = new Regex(pattern);
            return FindMatches(input, regex);
        }
    }
}
