using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLAP;
using System.Diagnostics;

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

		public static bool InstallationNeeded()
		{
			if (!Util.isInstalled())
			{
				Console.WriteLine("It seems that MOF data is not installed in system, " +
					"you should run \"msifancontrol install\" first");

				return true;
			}

			return false;
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
			if (InstallationNeeded())
			{
				return;
			}
			
			Modes.FanAdvancedControlMode.applyProfile(type, values);

			Console.WriteLine("done");
		}

		[Verb]
		public static void Install(
			[DefaultValue(false)]
			[Aliases("y")]
			bool yes)
		{
			if (!yes)
			{
				Console.Write("Are you sure to install MSI MOF information for WmiAcpi driver? (y/n) ");
				var keyInfo = Console.ReadKey(false);
				yes = keyInfo.KeyChar == 'y' || keyInfo.KeyChar == 'Y';
				Console.WriteLine("");
			}

			if (yes)
			{
				string syswow64 = Environment.GetFolderPath(Environment.SpecialFolder.SystemX86);
				string filename = "MsiWmiAcpiMof.dll";
				string regname = "MsiWmiAcpiMof.reg";
				string myLocation = System.IO.Path.GetDirectoryName(
					System.Reflection.Assembly.GetEntryAssembly().Location);

				Console.WriteLine("Copying MOF definition file...");
				System.IO.File.Copy(
					myLocation + System.IO.Path.DirectorySeparatorChar + filename, 
					syswow64 + System.IO.Path.DirectorySeparatorChar + filename, true);

				Action<string> runHidden = (string cmd) =>
				{
					Process proc = new Process();
					proc.StartInfo.FileName = "CMD.exe";
					proc.StartInfo.Arguments = "/c " + cmd;
					proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
					proc.Start();
					proc.WaitForExit();
				};

				Console.WriteLine("Importing registry changes for WmiAcpi driver...");
				runHidden("reg.exe import \"" + myLocation + System.IO.Path.DirectorySeparatorChar + regname + "\"");

				Console.WriteLine("Installation completed: success");
				Console.WriteLine("NOTE: It is necessary to restart your PC before you can control fan speeds");
			}
			else
			{
				Console.WriteLine("Installation canceled by user");
			}
		}
	}
}
