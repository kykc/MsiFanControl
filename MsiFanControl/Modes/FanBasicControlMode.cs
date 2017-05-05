using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace MsiFanControl.Modes
{
	class BasicModeModel
	{
		private readonly ManagementObjectSearcher _searcher;
		private readonly string _propName;
		
		public int Value { get; set; }

		private static int toRaw(int newValue)
		{
			if (newValue > 0)
			{
				return newValue;
			}
			else
			{
				return 128 + Math.Abs(newValue);
			}
		}

		private static int fromRaw(int value)
		{
			if (value >= 128)
			{
				return 128 - value;
			}
			else
			{
				return value;
			}
		}

		private void RefreshFromDb()
		{
			bool init = false;

			Util.WmiQueryForeach(_searcher, (obj, index) => MsiWmiInstance.FromWmi(index, _propName, obj), (obj, instance) =>
			{
				if (instance.Index == 10)
				{
					if (instance.IsValid())
					{
						Value = fromRaw(Convert.ToInt32(obj.GetPropertyValue(_propName)));
						init = true;
					}
					else
					{
						throw new InvalidWmiInstanceException();
					}
				}
			});

			if (!init)
			{
				throw new InvalidWmiInstanceException();
			}
		}

		public BasicModeModel()
		{
			_searcher = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM MSI_System");
			_propName = "System";

			RefreshFromDb();
		}

		public void Commit()
		{
			Util.WmiQueryForeach(_searcher, (obj, index) => MsiWmiInstance.FromWmi(index, _propName, obj), (obj, instance) =>
			{
				if (instance.Index == 10)
				{
					if (instance.IsValid())
					{
						obj.SetPropertyValue(_propName, toRaw(Value));
						obj.Put();
					}
					else
					{
						throw new InvalidWmiInstanceException();
					}
				}
			});

			RefreshFromDb();
		}

		public override string ToString()
		{
			return "Current control value: " + Value.ToString();
		}
	}

	class FanBasicControlMode
	{

		public static void applyProfile(int newValue)
		{
			var model = new BasicModeModel();
			model.Value = newValue;
			model.Commit();
		}
	}
}
