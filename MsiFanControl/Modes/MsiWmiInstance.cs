using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace MsiFanControl.Modes
{
	class MsiWmiInstance
	{
		public uint Value { get; set; }
		public uint Index { get; set; }
		public string InstanceName { get; set; }
		public bool IsActive { get; set; }

		public Func<uint, bool> AdditionalValidator { get; set; } = (x) => { return true; };

		public bool IsValid()
		{
			try
			{
				return int.Parse(InstanceName.Split('_').Last()) == Index && AdditionalValidator(Value);
			}
			catch (Exception)
			{
				return false;
			}
		}

		public static MsiWmiInstance FromWmi(uint index, string propertyName, ManagementObject obj, Func<uint, bool> validator = null)
		{
			validator = validator ?? ((x) => { return true; });

			return new MsiWmiInstance
			{
				Index = index,
				Value = Convert.ToUInt32(obj.GetPropertyValue(propertyName)),
				InstanceName = Convert.ToString(obj.GetPropertyValue(AdvancedModeModel.NAME_KEY)),
				IsActive = Convert.ToBoolean(obj.GetPropertyValue(AdvancedModeModel.ACTIVE_KEY)),
				AdditionalValidator = validator
			};
		}
	}
}
