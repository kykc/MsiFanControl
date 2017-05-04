using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLAP;

namespace MsiFanControl
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.RunConsole<FanControlVerbs>(args);
        }
    }

    class FanControlVerbs
    {
        [Empty, Help]
        public static void Help(string help)
        {
            help = "For now only advanced mode is implemented, example:\n" + 
                "\tmsifancontrol advanced -t:cpu -v:20,30,40,50,60,70\n" + 
                help;

            Console.WriteLine(help);
        }

        [Verb]
        public static void Advanced(
            [Aliases("t")]
            [Description("")]
            [Required]
            FanType type,

            [Aliases("v")]
            [Description("")]
            [Required]
            [AdvancedModeValuesValidation]
            int[] values)
        {
            int v0 = values[0];
            int v1 = values[1];
            int v2 = values[2];
            int v3 = values[3];
            int v4 = values[4];
            int v5 = values[5];

            bool isCpu = type == FanType.cpu;

            Modes.FanAdvancedControlMode.applyProfile(isCpu, v0, v1, v2, v3, v4, v5);

            Console.WriteLine("done");
        }

        [Verb]
        public static void Install(
            [DefaultValue(false)]
            [Aliases("y")]
            bool yes)
        {
            // TODO: installation routine:
            // 0. Convirmation text and prompt if no --yes flag
            // 1. Copy compiled MOF definitions to SysWOW64
            // 2. Add MsiWmiAcpiMof.reg to registry
            // 3. Output text of success/failure and advice to restart PC
        }
    }
}
