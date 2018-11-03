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

namespace dnSpy.Contracts.Disassembly {
	/// <summary>
	/// GNU assembler (AT&amp;T syntax) disassembly settings
	/// </summary>
	public interface IGasDisassemblySettings : IDisassemblySettings {
		/// <summary>
		/// If true, the formatter doesn't add '%' to registers, eg. %eax vs eax
		/// </summary>
		bool NakedRegisters { get; set; }

		/// <summary>
		/// Shows the mnemonic size suffix, eg. 'mov %eax,%ecx' vs 'movl %eax,%ecx'
		/// </summary>
		bool ShowMnemonicSizeSuffix { get; set; }

		/// <summary>
		/// Add a space after the comma if it's a memory operand, eg. '(%eax,%ecx,2)' vs '(%eax, %ecx, 2)'
		/// </summary>
		bool SpaceAfterMemoryOperandComma { get; set; }
	}
}
