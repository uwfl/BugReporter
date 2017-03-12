using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomUtilities
{
    public class FileExtension
    {
        /// <summary>
        /// The description of the file extension (e.g. Microsoft Excel file, XML File). 
        /// </summary>
        public string Description { get; protected set; }

        /// <summary>
        /// The extension type of this fileextension (e,g. exe, csv, xml).
        /// </summary>
        public string Extension { get; protected set; }

        /// <summary>
        /// Creates a new instance of FileExtension.
        /// </summary>
        /// <param name="extension">Extension type, e.g. 'exe', 'csv' or 'xml'.</param>
        /// <param name="description">The description of the file extension, e.g. Microsoft Excel file.</param>
        public FileExtension(string extension, string description)
        {
            Extension = extension.Replace(".", "").ToUpper();
            Description = description;
        }
    }
}
