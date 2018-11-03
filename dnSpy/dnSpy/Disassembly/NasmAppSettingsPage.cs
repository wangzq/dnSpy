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
using Iced.Intel;

namespace dnSpy.Disassembly {
	sealed class NasmAppSettingsPage : X86DisassemblyCodeStyleAppSettingsPage {
		public override double Order => CodeStyleConstants.CODESTYLE_NASM_ORDER;
		public override Guid Guid => new Guid("929A1758-C8CC-450E-A214-90B390A846DF");
		public override string Title => CodeStyleConstants.NASM_NAME;

		NasmDisassemblySettings NasmSettings => (NasmDisassemblySettings)Settings;

		public NasmAppSettingsPage(NasmDisassemblySettings disassemblySettings)
			: base(disassemblySettings, disassemblySettings.Clone(), new NasmFormatter(new NasmFormatterOptions(), SymbolResolver.Instance)) { }

		protected override void InitializeFormatterOptionsCore(FormatterOptions options) {
			var nasm = (NasmFormatterOptions)options;
			nasm.ShowSignExtendedImmediateSize = NasmSettings.ShowSignExtendedImmediateSize;
		}

		public override void OnApply() =>
			((NasmDisassemblySettings)disassemblySettings).CopyTo((NasmDisassemblySettings)_global_disassemblySettings);
	}
}
