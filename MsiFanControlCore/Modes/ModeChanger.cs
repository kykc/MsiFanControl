using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace MsiFanControl.Modes
{
	class ModeChanger
	{
		private static Dictionary<ControlMode, Func<int, int>> GetTransforms()
		{
			return new Dictionary<ControlMode, Func<int, int>>()
			{
				{ ControlMode.auto, (i) => i & 63 },
				{ ControlMode.basic, (i) => (i & 127) | 64 },
				{ ControlMode.advanced, (i) => (i | 128) & 191 }
			};
		}

		public static ControlMode GetCurrentMode()
		{
			var transforms = GetTransforms();

			var searcher = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM MSI_System");
			const string propName = "System";
			ControlMode? result = null;

			Util.WmiQueryForeach(searcher, (obj, index) => MsiWmiInstance.FromWmi(index, propName, obj), (obj, instance) =>
			{
				if (instance.Index == 9)
				{
					if (instance.IsValid())
					{
						int value = Convert.ToInt32(obj.GetPropertyValue(propName));

						foreach (var pair in transforms)
						{
							if (value == pair.Value(value))
							{
								result = pair.Key;
							}
						}
					}
				}
			});

			return result.Value;
		}

		public static void ChangeMode(ControlMode mode)
		{
			var transforms = GetTransforms();

			var searcher = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM MSI_System");
			const string propName = "System";

			Util.WmiQueryForeach(searcher, (obj, index) => MsiWmiInstance.FromWmi(index, propName, obj), (obj, instance) =>
			{
				if (instance.Index == 9)
				{
					if (instance.IsValid())
					{
						int value = Convert.ToInt32(obj.GetPropertyValue(propName));
						value = transforms[mode](value);
						obj.SetPropertyValue(propName, value);
						obj.Put();
					}
					else
					{
						throw new InvalidWmiInstanceException();
					}
				}
			});
		}
	}
}
