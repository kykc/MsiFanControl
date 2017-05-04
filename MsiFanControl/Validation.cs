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
    enum FanType
    {
        gpu = 0,
        cpu = 1
    }

    class AdvancedModeValuesValidator : IValueValidator
    {
        public const string DESCRIPTION = "Advanced mode values argument should be exatly six integers in range [0;150], for example 20,30,40,50,60,70";

        public AdvancedModeValuesValidator()
        {

        }

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
}
