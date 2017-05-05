using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace MsiFanControl.Modes
{
	class AdvancedModeInvalidException : Exception
	{
		public AdvancedModeInvalidException() : base() { }
		public AdvancedModeInvalidException(string msg) : base(msg) { }
	}

	class AdvancedModeModel
	{
		private readonly string _propertyName;
		private readonly string _tableName;
		private readonly List<uint> _indexWhiteList = new List<uint> { 11, 12, 13, 14, 15, 16 };
		private readonly FanType _fanType;
		private ManagementObjectSearcher _searcher;
		private List<MsiWmiInstance> _instances;

		public const string NAME_KEY = "InstanceName";
		public const string ACTIVE_KEY = "Active";

		private void RefreshFromDb()
		{
			_instances = new List<MsiWmiInstance>();

			Util.WmiQueryForeach(
				_searcher,
				(obj, index) => MsiWmiInstance.FromWmi(index, _propertyName, obj, (x) => x <= 150),
				(obj, model) => _instances.Add(model));

			_instances = _instances.Where((x) => _indexWhiteList.Contains(x.Index)).ToList();

			if (!IsValid())
			{
				throw new AdvancedModeInvalidException();
			}
		}

		public AdvancedModeModel(FanType type)
		{
			_propertyName = type == FanType.cpu ? "CPU" : "VGA";
			_tableName = type == FanType.cpu ? "MSI_CPU" : "MSI_VGA";
			_fanType = type;

			_searcher = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM " + _tableName);

			RefreshFromDb();	
		}

		public bool IsValid()
		{
			return _instances.All((x) => x.IsValid()) && _instances.Count == _indexWhiteList.Count;
		}

		public void Commit()
		{
			if (!IsValid())
			{
				throw new AdvancedModeInvalidException();
			}

			Util.WmiQueryForeach(
				_searcher,
				(obj, index) => MsiWmiInstance.FromWmi(index, _propertyName, obj, (x) => x <= 150),
				(obj, model) =>
				{
					if (_indexWhiteList.Contains(model.Index))
					{
						int value = (int)_instances.Where((x) => x.Index == model.Index).First().Value;
						obj.SetPropertyValue(_propertyName, value);
						obj.Put();
					}
				}
			);

			RefreshFromDb();
		}

		public IEnumerable<MsiWmiInstance> Enumerate()
		{
			if (!IsValid())
			{
				throw new AdvancedModeInvalidException();
			}

			return _instances.AsEnumerable();
		}

		public MsiWmiInstance this[uint i]
		{
			get
			{
				if (!IsValid())
				{
					throw new AdvancedModeInvalidException();
				}
				else if (i > 6)
				{
					throw new IndexOutOfRangeException(i.ToString());
				}
				else
				{
					return _instances[(int)i];
				}
			}
		}
	}

	class FanAdvancedControlMode
	{
		public static void applyProfile(FanType type, int[] values)
		{
			AdvancedModeModel model = new AdvancedModeModel(type);

			foreach (var item in model.Enumerate().Select((value, i) => new { i, value }))
			{
				item.value.Value = (uint)values[item.i];
			}

			model.Commit();
		}
	}
}
