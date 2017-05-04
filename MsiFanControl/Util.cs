using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsiFanControl
{
    static class Util
    {
        public static bool isInstalled()
        {
            // TODO: also some sanity checks on registry?

            string syswow64 = Environment.GetFolderPath(Environment.SpecialFolder.SystemX86);
            string filename = "MsiWmiAcpiMof.dll";

            return System.IO.File.Exists(syswow64 + System.IO.Path.DirectorySeparatorChar + filename);
        }
    }
}
