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
	sealed class GasAppSettingsPage : X86DisassemblyCodeStyleAppSettingsPage {
		public override double Order => CodeStyleConstants.CODESTYLE_GAS_ORDER;
		public override Guid Guid => new Guid("B2A9B538-925A-4029-9158-8C2FE632764D");
		public override string Title => CodeStyleConstants.GAS_NAME;

		public X86DisasmBooleanSetting NakedRegisters { get; }
		public X86DisasmBooleanSetting ShowMnemonicSizeSuffix { get; }
		public X86DisasmBooleanSetting SpaceAfterMemoryOperandComma { get; }

		GasDisassemblySettings GasSettings => (GasDisassemblySettings)Settings;

		public GasAppSettingsPage(GasDisassemblySettings disassemblySettings)
			: base(disassemblySettings, disassemblySettings.Clone(), new GasFormatter(new GasFormatterOptions(), SymbolResolver.Instance)) {
			NakedRegisters = AddDisasmBoolSetting(() => GasSettings.NakedRegisters, value => GasSettings.NakedRegisters = value, Instruction.Create(Code.Xchg_r64_RAX, Register.RSI, Register.RAX));
			ShowMnemonicSizeSuffix = AddDisasmBoolSetting(() => GasSettings.ShowMnemonicSizeSuffix, value => GasSettings.ShowMnemonicSizeSuffix = value, Instruction.Create(Code.Xchg_r64_RAX, Register.RSI, Register.RAX));
			SpaceAfterMemoryOperandComma = AddDisasmBoolSetting(() => GasSettings.SpaceAfterMemoryOperandComma, value => GasSettings.SpaceAfterMemoryOperandComma = value, Instruction.Create(Code.Mov_rm64_r64, new MemoryOperand(Register.RAX, Register.RDI, 4, 0x12345678, 8), Register.RCX));
		}

		protected override void InitializeFormatterOptionsCore(FormatterOptions options) {
			var gas = (GasFormatterOptions)options;
			gas.NakedRegisters = GasSettings.NakedRegisters;
			gas.ShowMnemonicSizeSuffix = GasSettings.ShowMnemonicSizeSuffix;
			gas.SpaceAfterMemoryOperandComma = GasSettings.SpaceAfterMemoryOperandComma;
		}

		public override void OnApply() =>
			((GasDisassemblySettings)disassemblySettings).CopyTo((GasDisassemblySettings)_global_disassemblySettings);
	}
}
