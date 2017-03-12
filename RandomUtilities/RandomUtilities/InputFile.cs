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

        public InputFile(string path, string filename)
        {
            FilePath = path;
            Filename = filename;
            Extension = Path.GetExtension(filename);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj.GetType() != typeof(InputFile))
                return false;

            var other = obj as InputFile;

            if (this.Filename != other.Filename)
                return false;

            if (this.FilePath != other.FilePath)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            int hash = 0;

            foreach (var c in FilePath)
            {
                hash += (int)c;
            }

            foreach (var c in Filename)
            {
                hash += (int)c;
            }

            return hash;
        }

        public override string ToString()
        {
            return string.Format("Path: {0}, Filename: {1}, Extension: {2}.", FilePath, Filename, Extension);
        }
    }
}
