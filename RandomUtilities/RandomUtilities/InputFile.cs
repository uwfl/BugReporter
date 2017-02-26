using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomUtilities
{
    public class InputFile
    {
        public string FilePath { get; set; }
        public string Filename { get; set; }
        public string Extension { get; set; }
        public string Content
        {
            get
            {
                if(File.Exists(FilePath))
                    return File.ReadAllText(FilePath);

                return string.Empty;
            }
        }

        public InputFile(string path)
        {
            FilePath = path;
            Filename = Path.GetFileName(path);
            Extension = Path.GetExtension(path);
        }
    }
}
