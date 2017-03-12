using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RandomUtilities
{
    public static class Regexes
    {
        public static Regex STRING_REGEX = new Regex("(?<=\").{4,}?(?=\")", RegexOptions.Compiled);
        public static Regex STRING_REGEX2 = new Regex("\".*?\"", RegexOptions.Compiled);
        
        public static Regex PATH_REGEX2 = new Regex("(?<=\")[ \\w -/\\ ]+?(?=\")", RegexOptions.Compiled);
        public static Regex FILE_REGEX = new Regex(@"/.+\..{1, 5}/", RegexOptions.Compiled);
    }
}
