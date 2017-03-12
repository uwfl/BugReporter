using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomUtilities
{
    public class CodeParser
    {
        private readonly List<FileExtension> extensions;
        public List<string> Paths { get; set; }
        public List<string> Files { get; set; }

        public CodeParser(List<FileExtension> allowedExtensions)
        {
            extensions = allowedExtensions;
            Paths = new List<string>();
            Files = new List<string>();                
        }

        public List<StringDefinition> FindStrings(string[] code)
        {
            List<StringDefinition> stringDefinitions = new List<StringDefinition>();

            string cleanedCode;
            var trimArray = new char[] { '\t', '\r', '\n' };
            List<int> quoteIndexes = new List<int>();
            for (int i = 0; i < code.Length; i++)
            {
                if (!code[i].Contains("\""))
                    continue;

                cleanedCode = code[i].Trim(trimArray);                
                if (cleanedCode.StartsWith("///"))
                    continue;

                quoteIndexes.Clear();
                for (int j = 0; j < cleanedCode.Length; j++)
                {
                    if (cleanedCode[j] == '\"')
                        quoteIndexes.Add(j);
                }

                if (quoteIndexes.Count % 2 == 0)
                {
                    for (int j = 0; j < quoteIndexes.Count; j += 2)
                    {
                        var stringDefinition = cleanedCode.Substring(quoteIndexes[j] + 1, quoteIndexes[j + 1] - quoteIndexes[j] - 1);

                        // Remove HTTP Parts.
                        if(stringDefinition.ToUpper().Contains("HTTP"))
                        {
                            // Find first whitespace.
                            var httpIndex = stringDefinition.ToUpper().IndexOf("HTTP");
                            var whiteSpaceIndex = stringDefinition.Substring(httpIndex, stringDefinition.Length - httpIndex).IndexOf(' ');

                            // If the string only consists of an http part, continue with the next string.
                            if (whiteSpaceIndex < httpIndex)
                                continue;

                            // Remove HTTP part.
                            stringDefinition = stringDefinition.Remove(httpIndex, whiteSpaceIndex + 1); 
                        }

                        if (!string.IsNullOrWhiteSpace(stringDefinition))
                        {
                            if (stringDefinition.Contains("file:///"))
                                stringDefinitions.Add(new StringDefinition(i, stringDefinition.Replace("file:///", "")));
                            else
                                stringDefinitions.Add(new StringDefinition(i, stringDefinition));
                        }
                    }
                }                

                if (quoteIndexes.Count % 2 != 0 && quoteIndexes.Count > 1)
                    Console.WriteLine("open quote found");
            }

            var validStringDefinitions = stringDefinitions.Where(s => IsValid(s.LineContent.ToUpper())).ToList();            

            return validStringDefinitions;
        }

        private bool IsValid(string s)
        {
            // Valid name of directory or file.
            if (s.Contains('*') || s.Contains('?') || s.Contains('<') || s.Contains('>') || s.Contains('|') || s.Contains('"'))
                return false;

            // Name must be either a directory using '/' or '\' or a file name associated with an extension.
            if (!(s.Contains('/') || s.Contains('.') || s.Contains('\\')))
                return false;

            // Name can not start or end with a '.'.
            if (s.StartsWith(".") || s.EndsWith("."))
                return false;

            // Name can not start and end with a '+'.
            if (s.StartsWith("+") && s.EndsWith("+"))
                return false;

            // If the string contains a '/' or '\', then it must contain a minimum of one and a maximum of two ':'.
            if ((s.Contains('/') || s.Contains('\\')) && (s.Count(c => c == ':') < 1 || s.Count(c => c == ':') > 2))
                return false;

            // If the string contains a '/' or '\', and contains at least 1 and a maximum of 2 ':', the colon must be placed before the slashes.
            if (s.Contains('/') || s.Contains('\\'))
            {
                int indexOfSlash;
                if (s.Contains('/'))
                    indexOfSlash = s.IndexOf('/');
                else
                    indexOfSlash = s.IndexOf('\\');

                int indexOfColon = s.IndexOf(':');

                if (indexOfSlash < indexOfColon)
                    return false;
            }

            // If the string contains no '/' or '\' but a '.', it must end with one of the allowed file extensions.
            if (!(s.Contains("\\") || s.Contains("/")))
            {
                var correctExtension = false;
                foreach (var allowedExtension in extensions)
                {
                    correctExtension = s.EndsWith(allowedExtension.Extension);

                    if (correctExtension)
                        break;
                }

                if (correctExtension == false)
                    return false;
            }

            // Name can not contain a URL.
            if (s.Contains("HTTP:"))
                return false;

            return true;
        }

        public bool IsPath(string s)
        {
            // Check general definitions first, as naming schemes.
            if (!IsValid(s))
                return false;

            // If the string contains a '/' or '\', then it must contain a minimum of one and a maximum of two ':'.
            if ((s.Contains('/') || s.Contains('\\')) && (s.Count(c => c == ':') < 1 || s.Count(c => c == ':') > 2))
                return false;

            // If the string contains a '/' or '\', and contains at least 1 and a maximum of 2 ':', the colon must be placed before the slashes.
            if (s.Contains('/') || s.Contains('\\'))
            {
                int indexOfSlash;
                if (s.Contains('/'))
                    indexOfSlash = s.IndexOf('/');
                else
                    indexOfSlash = s.IndexOf('\\');

                int indexOfColon = s.IndexOf(':');

                if (indexOfSlash < indexOfColon)
                    return false;
            }

            return true;
        }

        public bool IsFile(string s)
        {
            // Check general definitions first, as naming schemes.
            if (!IsValid(s))
                return false;

            // If the string contains no '/' or '\' but a '.', it must end with one of the allowed file extensions.
            if (!(s.Contains("\\") || s.Contains("/")))
            {
                var correctExtension = false;
                foreach (var allowedExtension in extensions)
                {
                    correctExtension = s.EndsWith(allowedExtension.Extension);

                    if (correctExtension)
                        break;
                }

                if (correctExtension == false)
                    return false;
            }

            return true;
        }
    }
}
