using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace MsiFanControl.Modes
{
	class FanAdvancedControlMode
	{
		public static void applyProfile(bool isCPU, int value0, int value1, int value2, int value3, int value4, int value5)
		{
			int num = 0;
			string propertyName = "CPU";
			ManagementObjectSearcher managementObjectSearcher;
			if (!isCPU)
			{
				propertyName = "VGA";
				managementObjectSearcher = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM MSI_VGA");
			}
			else
			{
				managementObjectSearcher = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM MSI_CPU");
			}

			try
			{
				using (var enumerator = managementObjectSearcher.Get().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ManagementObject managementObject = (ManagementObject)enumerator.Current;
						switch (num)
						{
							case 11:
								managementObject.SetPropertyValue(propertyName, value0);
								managementObject.Put();
								break;
							case 12:
								managementObject.SetPropertyValue(propertyName, value1);
								managementObject.Put();
								break;
							case 13:
								managementObject.SetPropertyValue(propertyName, value2);
								managementObject.Put();
								break;
							case 14:
								managementObject.SetPropertyValue(propertyName, value3);
								managementObject.Put();
								break;
							case 15:
								managementObject.SetPropertyValue(propertyName, value4);
								managementObject.Put();
								break;
							case 16:
								managementObject.SetPropertyValue(propertyName, value5);
								managementObject.Put();
								break;
						}
						num++;
					}
				}
				num = 0;
				var managementObjectSearcher2 = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM MSI_System");
				using (var enumerator2 = managementObjectSearcher2.Get().GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						ManagementObject managementObject2 = (ManagementObject)enumerator2.Current;
						if (num == 9)
						{
							int num2;
							int.TryParse(managementObject2["System"].ToString(), out num2);
							num2 |= 128;
							num2 &= 191;
							managementObject2.SetPropertyValue("System", num2);
							managementObject2.Put();
							break;
						}
						num++;
					}
				}
			}
			catch (Exception)
			{
			}
		}
	}
}
