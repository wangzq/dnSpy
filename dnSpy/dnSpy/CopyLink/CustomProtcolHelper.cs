using System.IO;
using System.Reflection;
using Microsoft.Win32;
using static dnSpy.CopyLink.RegMerger;

namespace dnSpy.CopyLink {
	public static class CustomProtcolHelper {
		public static void RegisterThisAssembly(string name) {
			if (!Exists(name)) {
				var asmPath = Path.GetFullPath(Assembly.GetExecutingAssembly().Location);
				Register(name, asmPath);
			}
		}

		public static bool Exists(string name) {
			var subkey = Registry.CurrentUser.OpenSubKey($@"Software\Classes\{name}");
			if (subkey != null) {
				subkey.Close();
				return true;
			}

			return false;
		}

		public static void Register(string name, string program) {
			var data = Reg(
				name, Reg(
					"DefaultIcon", Reg(
						"",
						program
					),
					"shell", Reg(
						"open", Reg(
							"command", Reg(
								"", $@"""{program}"" ""%1"""
							)
						)
					)
				)
			);

			var classes = Registry.CurrentUser.OpenSubKey(@"Software\Classes", true);
			try {
				MergeRegistry(classes, data);
			}
			finally {
				classes.Close();
			}
		}
	}
}
