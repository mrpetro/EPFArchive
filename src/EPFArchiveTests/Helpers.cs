using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EPFArchiveTests
{
    public static class Helpers
    {
        public static bool FileEquals(string filePath1, string filePath2)
        {
            byte[] file1 = File.ReadAllBytes(filePath1);
            byte[] file2 = File.ReadAllBytes(filePath2);
            if (file1.Length == file2.Length)
            {
                for (int i = 0; i < file1.Length; i++)
                {
                    if (file1[i] != file2[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public static bool DeployResource(string outFilePath, string resourceName)
        {
            try
            {
                var asm = Assembly.GetExecutingAssembly();
                var resource = string.Format("EPFArchiveTests.Resources.{0}", resourceName);
                using (var stream = asm.GetManifestResourceStream(resource))
                {
                    using (var outStream = File.Create(outFilePath))
                    {
                        stream.CopyTo(outStream);
                        return true;
                    }
                }
            }
            catch { }

            return false;
        }
    }
}
