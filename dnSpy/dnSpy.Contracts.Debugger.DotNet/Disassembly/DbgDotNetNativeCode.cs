﻿/*
    Copyright (C) 2014-2018 de4dot@gmail.com

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
using dnSpy.Contracts.Disassembly;

namespace dnSpy.Contracts.Debugger.DotNet.Disassembly {
	/// <summary>
	/// Contains the code that will be disassembled
	/// </summary>
	public readonly struct DbgDotNetNativeCode {
		/// <summary>
		/// Gets the code kind
		/// </summary>
		public NativeCodeKind Kind { get; }

		/// <summary>
		/// Gets the optimization kind
		/// </summary>
		public NativeCodeOptimization Optimization { get; }

		/// <summary>
		/// All blocks to disassemble
		/// </summary>
		public DbgDotNetNativeCodeBlock[] Blocks { get; }

		/// <summary>
		/// Extra optional info, or null if none
		/// </summary>
		public X86NativeCodeInfo CodeInfo { get; }

		/// <summary>
		/// Method name or null
		/// </summary>
		public string MethodName { get; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="kind">Code kind</param>
		/// <param name="optimization">Optimization kind</param>
		/// <param name="blocks">All blocks to disassemble</param>
		/// <param name="codeInfo">Extra code info or null</param>
		/// <param name="methodName">Method name or null</param>
		public DbgDotNetNativeCode(NativeCodeKind kind, NativeCodeOptimization optimization, DbgDotNetNativeCodeBlock[] blocks, X86NativeCodeInfo codeInfo, string methodName) {
			Kind = kind;
			Optimization = optimization;
			Blocks = blocks ?? throw new ArgumentNullException(nameof(blocks));
			CodeInfo = codeInfo;
			MethodName = methodName;
		}
	}
}
