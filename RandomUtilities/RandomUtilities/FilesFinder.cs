using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomUtilities
{
    public class FilesFinder
    {
        public static List<InputFile> DirectorySearch(string directory)
        {
            var results = new List<InputFile>();
            DirectorySearch(directory, new List<string>(), results);

            return results;
        }

        public static List<InputFile> DirectorySearch(string directory, List<string> ignoredDirectories)
        {
            var results = new List<InputFile>();
            DirectorySearch(directory, ignoredDirectories, results);

            return results;
        }

        private static List<InputFile> DirectorySearch(string directory, List<string> ignoredDirectories, List<InputFile> results)
        {
            if (ignoredDirectories.Any(d => directory.EndsWith(d)))
                return results;

            try
            {
                foreach (string subDirectory in Directory.GetDirectories(directory))
                {
                    DirectorySearch(subDirectory, ignoredDirectories, results);
                }

                foreach (string f in Directory.GetFiles(directory))
                {
                    results.Add(new InputFile(f));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error while searching files in directory. See inner exception", ex);
            }

            return results;
        }
    }
}