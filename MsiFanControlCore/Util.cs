using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
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

		public static void WmiQueryForeach<T>(ManagementObjectSearcher searcher, Func<ManagementObject, uint, T> factory, Action<ManagementObject, T> worker) where T: class
		{
			using (var enumerator = searcher.Get().GetEnumerator())
			{
				uint index = 0;

				while (enumerator.MoveNext())
				{
					var obj = (ManagementObject)enumerator.Current;
					var model = factory(obj, index);

					worker(obj, model);

					index += 1;
				}
			}
		}
	}
}
