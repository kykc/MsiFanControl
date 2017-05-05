using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace MsiFanControl.Modes
{
	class FanBasicControlMode
	{
		public static void applyProfile(int newValue)
		{
			var searcher = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM MSI_System");
			const string propName = "System";

			Util.WmiQueryForeach(searcher, (obj, index) => MsiWmiInstance.FromWmi(index, propName, obj), (obj, instance) =>
			{
				if (instance.Index == 10)
				{
					if (instance.IsValid())
					{
						int value = Convert.ToInt32(obj.GetPropertyValue(propName));

						value &= 240;

						if (newValue > 0)
						{
							value += newValue;
							value &= 127;
						}
						else
						{
							value += Math.Abs(newValue);
							value |= 128;
						}

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
