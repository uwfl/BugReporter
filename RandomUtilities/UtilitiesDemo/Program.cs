using RandomUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Packaging;

namespace UtilitiesDemo
{
    class Program
    {
        public static List<FileExtension> allowedExtensions = new List<FileExtension>()
        {
            new FileExtension("exe", "Executable"),
            new FileExtension("csv", "CSV"),
            new FileExtension("xls", "Excel File"),
            new FileExtension("xlsx", "Excel File"),
            new FileExtension("xlsm", "Excel File"),
            new FileExtension("xml", "XML"),
            new FileExtension("txt", "Text file"),
            new FileExtension("xsd", "XSD files")
        };

        static void Main(string[] args)
        {
            var result = FilesFinder.DirectorySearch(@"C:\Users\Martin\Documents\Coding\MapForce_Automation\40_Csharp", new List<string>()
            {
                "bin",
                "obj",
                "Altova",
                "AltovaXML",
                "AltovaText",
                "AltovaFunctions"
            });

            var classfiles = result.Where(f => f.Extension == ".cs" && !f.Filename.StartsWith("Temp") && !f.Filename.StartsWith("Assemb") && !(f.Filename.StartsWith("MappingForm"))).ToList();

            string path = @"C:\Users\Martin\Documents\Coding\MapForce_Automation\MergedContent.cs";

            var usingDirectives = System.IO.File.ReadAllLines(path).Where(l => l.StartsWith("using ") && l.Contains("System")).ToList();

            string[] split = new string[] { "using " };
            for (int i = 0; i < usingDirectives.Count; i++)
            {
                usingDirectives[i] = usingDirectives[i].Split(split, StringSplitOptions.RemoveEmptyEntries)[0];
                usingDirectives[i] = usingDirectives[i].Substring(0, usingDirectives[i].IndexOf(';')) + ".dll";
            }

            usingDirectives.Clear();
            usingDirectives.Add("System.dll");
            usingDirectives.Add("System.Xml.dll");
            usingDirectives.Add("mscorlib.dll");
            usingDirectives.Add("WindowsBase.dll");

            var assemblies = AppDomain.CurrentDomain
                            .GetAssemblies()
                            .Where(a => !a.IsDynamic)
                            .Select(a => a.Location).ToList();

            if(!assemblies.Any(a => a.ToUpper().EndsWith("SYSTEM.XML.DLL")))
                assemblies.Add("System.Xml.dll");

            if (!assemblies.Any(a => a.ToUpper().EndsWith("SYSTEM.DATA.DLL")))
                assemblies.Add(@"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.2\System.Data.dll");

            assemblies.Add(@"C:\Users\Martin\Documents\Coding\MapForce_Automation\Libs\Altova.dll");
            assemblies.Add(@"C:\Users\Martin\Documents\Coding\MapForce_Automation\Libs\AltovaFunctions.dll");
            assemblies.Add(@"C:\Users\Martin\Documents\Coding\MapForce_Automation\Libs\AltovaText.dll"); 
            assemblies.Add(@"C:\Users\Martin\Documents\Coding\MapForce_Automation\Libs\AltovaXML.dll");
            assemblies.Add(@"C:\Users\Martin\Documents\Coding\MapForce_Automation\Libs\WindowsBase.dll");

            CodeCompiler.MergeClassFiles(classfiles, path);
            
            var test = System.IO.File.ReadAllLines(path);
            var trimArray = new char[] { '\t', '\r', '\n' };
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < test.Length; i++)
            {
                var t = test[i].Trim(trimArray);

                if (t.Contains("public static bool Validate(string input, TypeInfo.TypeInfo info)"))
                {
                    test[i] = test[i].Replace("TypeInfo.TypeInfo info", "Altova.TypeInfo.TypeInfo info");
                }
                else if (t.Contains("TypeInfo.TypeInfo typeInfo;"))
                {
                    test[i] = test[i].Replace("TypeInfo.TypeInfo typeInfo;", "Altova.TypeInfo.TypeInfo typeInfo;");
                }
                else if(t.Contains("public SimpleType(TypeInfo.TypeInfo typeInfo)"))
                {
                    test[i] = test[i].Replace("public SimpleType(TypeInfo.TypeInfo typeInfo)", "public SimpleType(Altova.TypeInfo.TypeInfo typeInfo)");
                }

                sb.AppendLine(test[i]);
            }
            System.IO.File.WriteAllText(path, sb.ToString());

            CodeCompiler.Compile(path, executable: false, referencedAssemblies: assemblies);
            //CodeCompiler.Compile(path, executable: false, referencedAssemblies: usingDirectives);

            
            //GetInputFiles(path);
        }

        static void GetInputFiles(string path)
        {
            if (System.IO.File.Exists(path))
            {
                var content = System.IO.File.ReadAllLines(path);

                CodeParser cp = new CodeParser(allowedExtensions);

                var resultStrings = cp.FindStrings(content);
                var files = new List<StringDefinition>();
                var paths = new List<StringDefinition>();
                StringDefinition file;
                foreach (var s in resultStrings)
                {
                    file = null;
                    foreach (var extension in allowedExtensions)
                    {
                        if (s.LineContent.ToUpper().EndsWith(extension.Extension))
                        {
                            file = s;
                            break;
                        }
                    }

                    if (file == null)
                        paths.Add(s);
                    else
                        files.Add(s);
                }

                var inputfiles = new List<InputFile>();
                var strangeInputfiles = new List<InputFile>();

                // Try to find for each file the path needed.
                foreach (var f in files)
                {
                    var possiblePaths = paths.Where(p => (f.LineIndex - 5 <= p.LineIndex) && (p.LineIndex <= f.LineIndex)).OrderByDescending(p => p.LineIndex).ToList();

                    if (possiblePaths.Count == 1)
                    {
                        inputfiles.Add(new InputFile(possiblePaths[0].LineContent, f.LineContent));
                    }
                    else
                    {
                        strangeInputfiles.Add(new InputFile(f.LineContent));
                    }
                }

                inputfiles = inputfiles.Distinct().ToList();
                strangeInputfiles = strangeInputfiles.Distinct().ToList();

                var difference = strangeInputfiles.Where(s => !inputfiles.Any(i => i.Filename == s.Filename)).ToList();
            }
        }        
    }
}
