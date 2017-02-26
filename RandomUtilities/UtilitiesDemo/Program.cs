using RandomUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilitiesDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = FilesFinder.DirectorySearch(@"C:\Users\Martin\Source\Repos\BugReporter\RandomUtilities\RandomUtilities");
            foreach (var r in result)
            {
                Console.WriteLine(string.Format("Filename: {0}, Extension: {1}.", r.Filename, r.Extension));
            }

            var classfiles = result.Where(f => f.Extension == ".cs" && !f.Filename.StartsWith("Temp") && !f.Filename.StartsWith("Assemb")).ToList();

            CodeCompiler.MergeClassFiles(classfiles, @"C:\Users\Martin\Source\Repos\BugReporter\RandomUtilities\MergedContent.cs");
            CodeCompiler.CompileExecutable(@"C:\Users\Martin\Source\Repos\BugReporter\RandomUtilities\MergedContent.cs", false);
        }
    }
}
