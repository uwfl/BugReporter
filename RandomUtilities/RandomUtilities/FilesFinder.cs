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
            DirectorySearch(directory, results);

            return results;
        }

        private static List<InputFile> DirectorySearch(string directory, List<InputFile> results)
        {
            try
            {
                foreach (string subDirectory in Directory.GetDirectories(directory))
                {
                    DirectorySearch(subDirectory, results);
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
