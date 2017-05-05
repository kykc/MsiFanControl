using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLAP;
using CLAP.Validation;
using System.Reflection;

namespace MsiFanControl
{
	class InvalidWmiInstanceException : Exception
	{
		public InvalidWmiInstanceException() : base() { }
		public InvalidWmiInstanceException(string msg) : base(msg) { }
	}

	enum ControlMode
	{
		auto = 0,
		basic = 1,
		advanced = 2
	}

	enum FanType
	{
		gpu = 0,
		cpu = 1
	}

	interface IDescriptionProvider
	{
		string GetDescription();
	}

	class AdvancedModeValuesValidator : IValueValidator, IDescriptionProvider
	{
		public const string DESCRIPTION = "Advanced mode values argument should be exatly six integers in range [0;150], "
			+ "for example 20,30,40,50,60,70";

		public AdvancedModeValuesValidator()
		{
		}

		public string GetDescription() { return DESCRIPTION; }

		public void Validate(ValueInfo info)
		{
			try
			{
				int[] values = (int[])info.Value;

				bool valid = values.Length == 6 && values.All((x) => x >= 0 && x <= 150);

				if (!valid)
				{
					throw new Exception();
				}
			}
			catch (Exception ex)
			{
				throw new ValidationException(DESCRIPTION, ex);
			}
		}
	}

	class BasicModeValueValidator : IValueValidator, IDescriptionProvider
	{
		public const string DESCRIPTION = "Basic mode control value must be integer in range [-15;15]";

		public BasicModeValueValidator() { }

		public string GetDescription() { return DESCRIPTION; }

		public void Validate(ValueInfo info)
		{
			try
			{
				int value = (int)info.Value;

				bool valid = value >= -15 && value <= 15;

				if (!valid)
				{
					throw new Exception();
				}
			}
			catch (Exception ex)
			{
				throw new ValidationException(DESCRIPTION, ex);
			}
		}
	}

	class AdvancedModeValuesValidationAttribute : ValidationAttribute
	{
		public override string Description
		{
			get
			{
				return AdvancedModeValuesValidator.DESCRIPTION;
			}
		}

		public override IValueValidator GetValidator()
		{
			return new AdvancedModeValuesValidator();
		}
	}

	class BasicModeValueValidationAttribute : ValidationAttribute
	{
		public override string Description
		{
			get
			{
				return BasicModeValueValidator.DESCRIPTION;
			}
		}

		public override IValueValidator GetValidator()
		{
			return new BasicModeValueValidator();
		}
	}
}
