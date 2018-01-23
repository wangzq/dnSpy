using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace dnSpy.CopyLink {
	public static class RegMerger {
		/// <summary>
		/// Helper to create nested registry data object. Suggest to use "using static RegMerger" to use
		/// this method directly like following:
		/// var data = Reg("my", Reg("DefaultIcon", Reg("", @"c:\tools\my.exe"), "shell", Reg("open", Reg("command", Reg("", @"c:\tools\my.exe ""%1""")))));
		/// This data can then be passed to MergeRegistry to merge into a parent key.
		/// </summary>
		public static Dictionary<string, object> Reg(params object[] values) {
			var dict = new Dictionary<string, object>();
			for (var i = 0; i < values.Length; i += 2) {
				var key = (string)values[i];
				dict.Add(key, values[i + 1]);
			}
			return dict;
		}

		/// <summary>
		/// Merge a nested registry data object under the specified parent key; note the parent key must be opened for writing.
		/// </summary>
		public static void MergeRegistry(RegistryKey parent, Dictionary<string, object> data) {
			foreach (var pair in data) {
				MergeKey(parent, pair.Key, pair.Value);
			}
		}

		private static void MergeKey(RegistryKey parent, string name, object value) {
			if (value == null) {
				throw new ArgumentNullException(nameof(value));
			}

			var data = value as Dictionary<string, object>;
			if (data != null) {
				var subkey = EnsureRegKey(parent, name);
				MergeRegistry(subkey, data);
				subkey.Close();
				return;
			}

			parent.SetValue(name, value);
		}

		private static RegistryKey EnsureRegKey(RegistryKey parent, string name) {
			if (parent == null) throw new ArgumentNullException(nameof(parent));
			if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

			var subkey = parent.OpenSubKey(name, true);
			if (subkey == null) {
				subkey = parent.CreateSubKey(name);
			}
			return subkey;
		}
	}
}
