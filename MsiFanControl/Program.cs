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
			int exitCode = Parser.RunConsole<FanControlVerbs>(args);
			Environment.Exit(exitCode);
		}
	}

	class FanControlVerbs
	{
		public static void OutputLine(string line, bool quiet)
		{
			if (!quiet)
			{
				Console.WriteLine(line);
			}
		}

		[Empty, Help]
		public static void Help(string help)
		{
			Console.WriteLine(help);
			Console.WriteLine("   Examples: ");
			Console.WriteLine("\tmsifancontrol auto");
			Console.WriteLine("\tmsifancontrol basic /value:-10");
			Console.WriteLine("\tmsifancontrol advanced /cpu:20,45,55,65,70,75 /gpu:0,20,40,60,80,80");
		}

		public static bool InstallationNeeded(bool quiet)
		{
			if (!Util.isInstalled())
			{
				OutputLine("It seems that MOF data is not installed in system, " +
					"you should run \"msifancontrol install\" first", quiet);

				return true;
			}

			return false;
		}

		[Verb(Description = "Gets info about currently active control profile and its properties (if any)")]
		public static void Status(bool quiet)
		{
			if (InstallationNeeded(quiet))
			{
				return;
			}

			var mode = Modes.ModeChanger.GetCurrentMode();

			OutputLine("Current control mode: " + mode.ToString(), quiet);

			if (mode == ControlMode.advanced)
			{
				var cpu = new Modes.AdvancedModeModel(FanType.cpu);
				var gpu = new Modes.AdvancedModeModel(FanType.gpu);

				OutputLine(cpu.ToString(), quiet);
				OutputLine(gpu.ToString(), quiet);
			}
			else if (mode == ControlMode.basic)
			{
				var model = new Modes.BasicModeModel();

				OutputLine(model.ToString(), quiet);
			}
		}

		[Verb(Description = "Default behavior of fans. In this profile they will work as if no custom control software were ever used")]
		public static void Auto(
			[Aliases("")]
			[Description("Quiet mode, not stdout output")]
			[DefaultValue(false)]
			bool quiet
			)
		{
			if (InstallationNeeded(quiet))
			{
				return;
			}

			OutputLine("Changing mode to auto", quiet);
			Modes.ModeChanger.ChangeMode(ControlMode.auto);

			OutputLine("All done", quiet);
			Status(quiet);
		}

		[Verb(Description = "Basic control mode allows to adjust overall speed of fans with one \"offset\" value")]
		public static void Basic(
			[Aliases("")]
			[Description("Control value for basic fan speed adjustment, allegedly affects both CPU and GPU fans")]
			[BasicModeValueValidation]
			int? value,

			[Aliases("")]
			[Description("Quiet mode, not stdout output")]
			[DefaultValue(false)]
			bool quiet
			)
		{
			if (InstallationNeeded(quiet))
			{
				return;
			}

			if (value.HasValue)
			{
				OutputLine("Setting basic profile control value", quiet);
				Modes.FanBasicControlMode.applyProfile(value.Value);
			}

			OutputLine("Changing mode to basic", quiet);
			Modes.ModeChanger.ChangeMode(ControlMode.basic);

			OutputLine("All done", quiet);
			Status(quiet);
		}

		[Verb(Description = "Advanced control mode allows to adjust the curve which represents fan speed function of temperature. " +
			"Exact temperatures at which next tier kicks in are unknown, those tiers are possibly hardware model dependent. " + 
			"What is known, is that they go from most cool to most hot.")]
		public static void Advanced(
			[Aliases("")]
			[Description("")]
			[AdvancedModeValuesValidation]
			int[] cpu,

			[Aliases("")]
			[Description("")]
			[AdvancedModeValuesValidation]
			int[] gpu,

			[Aliases("")]
			[Description("Quiet mode, not stdout output")]
			[DefaultValue(false)]
			bool quiet
			)
		{
			if (InstallationNeeded(quiet))
			{
				return;
			}
			
			if (cpu != null)
			{
				OutputLine("Setting CPU fan values", quiet);
				Modes.FanAdvancedControlMode.applyProfile(FanType.cpu, cpu);
			}

			if (gpu != null)
			{
				OutputLine("Setting GPU fan values", quiet);
				Modes.FanAdvancedControlMode.applyProfile(FanType.gpu, gpu);
			}

			OutputLine("Changing mode to advanced", quiet);
			Modes.ModeChanger.ChangeMode(ControlMode.advanced);

			OutputLine("All done", quiet);
			Status(quiet);
		}

		[Verb(Description = "Sadly, this utility needs to perform some minor changes in the system in order for it to work. " +
			"This is needed only once, and changes are system-wide (meaning it's not necessary to do this for each user profile, just once will be enough")]
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
