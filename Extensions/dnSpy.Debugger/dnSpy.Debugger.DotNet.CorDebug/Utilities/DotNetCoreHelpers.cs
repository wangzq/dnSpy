﻿/*
    Copyright (C) 2014-2017 de4dot@gmail.com

    This file is part of dnSpy

    dnSpy is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    dnSpy is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with dnSpy.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.IO;
using System.Reflection;

namespace dnSpy.Debugger.DotNet.CorDebug.Utilities {
	static class DotNetCoreHelpers {
		public const string DotNetExeName = "dotnet.exe";

		public static string GetPathToDotNetExeHost(int bitness) {
			if (bitness != 32 && bitness != 64)
				throw new ArgumentOutOfRangeException(nameof(bitness));
			var pathEnvVar = Environment.GetEnvironmentVariable("PATH");
			if (pathEnvVar == null)
				return null;
			foreach (var tmp in pathEnvVar.Split(new[] { Path.PathSeparator }, StringSplitOptions.RemoveEmptyEntries)) {
				var path = tmp.Trim();
				if (!Directory.Exists(path))
					continue;
				try {
					var file = Path.Combine(path, DotNetExeName);
					if (!File.Exists(file))
						continue;
					if (GetPeFileBitness(file) == bitness)
						return file;
				}
				catch {
				}
			}
			return null;
		}

		static int GetPeFileBitness(string file) {
			using (var f = File.OpenRead(file)) {
				var r = new BinaryReader(f);
				if (r.ReadUInt16() != 0x5A4D)
					return -1;
				f.Position = 0x3C;
				f.Position = r.ReadUInt32();
				if (r.ReadUInt32() != 0x4550)
					return -1;
				f.Position += 0x14;
				ushort magic = r.ReadUInt16();
				if (magic == 0x10B)
					return 32;
				if (magic == 0x20B)
					return 64;
				return -1;
			}
		}

		public static string GetDebugShimFilename(int bitness) {
			var basePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
			basePath = Path.Combine(basePath, "debug", "core");
			const string filename = "dbgshim.dll";
			switch (bitness) {
			case 32:	return Path.Combine(basePath, "x86", filename);
			case 64:	return Path.Combine(basePath, "x64", filename);
			default:	throw new ArgumentOutOfRangeException(nameof(bitness));
			}
		}
	}
}
