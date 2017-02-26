using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomUtilities
{
    public class CodeCompiler
    {
        public static string MergeClassFiles(List<InputFile> classfiles)
        {

            var usingDirectives = new List<string>();
            var codeLines = new List<string>();

            foreach (var c in classfiles)
            {
                if (File.Exists(c.FilePath))
                {
                    var content = File.ReadAllLines(c.FilePath);

                    usingDirectives.AddRange(content.Where(l => l.StartsWith("using")).ToList());
                    codeLines.AddRange(content.Where(l => !l.StartsWith("using")).ToList());
                }
            }

            var sb = new StringBuilder();

            usingDirectives = usingDirectives.Distinct().ToList();
            foreach (var usingDirective in usingDirectives)
            {
                sb.AppendLine(usingDirective);
            }

            foreach (var codeLine in codeLines)
            {
                sb.AppendLine(codeLine);
            }
            
            return sb.ToString();
        }

        public static void MergeClassFiles(List<InputFile> classfiles, string path)
        {
            var mergedContent = MergeClassFiles(classfiles);
            File.WriteAllText(path, mergedContent);
        }

        public static void CreateAssembly(InputFile classfile)
        {
            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();

            // True - memory generation, false - external file generation
            parameters.GenerateInMemory = false;
            // True - exe file generation, false - dll file generation
            parameters.GenerateExecutable = false;

            CompilerResults results = provider.CompileAssemblyFromSource(parameters, classfile.Content);
        }

        public static bool CompileExecutable(String sourceName, bool executable)
        {
            FileInfo sourceFile = new FileInfo(sourceName);
            CodeDomProvider provider = null;
            bool compileOk = false;

            // Select the code provider based on the input file extension.
            if (sourceFile.Extension.ToUpper(CultureInfo.InvariantCulture) == ".CS")
            {
                provider = CodeDomProvider.CreateProvider("CSharp");
            }
            else if (sourceFile.Extension.ToUpper(CultureInfo.InvariantCulture) == ".VB")
            {
                provider = CodeDomProvider.CreateProvider("VisualBasic");
            }
            else
            {
                Console.WriteLine("Source file must have a .cs or .vb extension");
            }

            if (provider != null)
            {
                // Format the executable file name.
                // Build the output assembly path using the current directory
                // and <source>_cs.exe or <source>_vb.exe.
                String filename = String.Format(@"{0}\{1}.{2}", 
                    System.Environment.CurrentDirectory, 
                    sourceFile.Name.Replace(".", "_"), 
                    (executable) ? "exe" : "dll");

                CompilerParameters cp = new CompilerParameters();

                // Generate an executable instead of 
                // a class library.
                cp.GenerateExecutable = executable;

                // Specify the assembly file name to generate.
                cp.OutputAssembly = filename;

                // Save the assembly as a physical file.
                cp.GenerateInMemory = false;

                // Set whether to treat all warnings as errors.
                cp.TreatWarningsAsErrors = false;

                // Invoke compilation of the source file.
                CompilerResults cr = provider.CompileAssemblyFromFile(cp, sourceName);

                if (cr.Errors.Count > 0)
                {
                    // Display compilation errors.
                    Console.WriteLine("Errors building {0} into {1}",
                        sourceName, cr.PathToAssembly);
                    foreach (CompilerError ce in cr.Errors)
                    {
                        Console.WriteLine("  {0}", ce.ToString());
                        Console.WriteLine();
                    }
                }
                else
                {
                    // Display a successful compilation message.
                    Console.WriteLine("Source {0} built into {1} successfully.",
                        sourceName, cr.PathToAssembly);
                }

                // Return the results of the compilation.
                if (cr.Errors.Count > 0)
                {
                    compileOk = false;
                }
                else
                {
                    compileOk = true;
                }
            }
            return compileOk;
        }
    }
}
