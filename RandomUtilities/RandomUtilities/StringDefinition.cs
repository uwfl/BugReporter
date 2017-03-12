using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomUtilities
{
    public class StringDefinition
    {
        public int LineIndex { get; set; }
        public string LineContent { get; set; }

        public StringDefinition(int index, string content)
        {
            LineIndex = index;
            LineContent = content;
        }

        public override string ToString()
        {
            return string.Format("Line: {0}, Content: '{1}'.", LineIndex, LineContent);
        }
    }
}
