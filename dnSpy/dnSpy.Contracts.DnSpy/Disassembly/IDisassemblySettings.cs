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

using System.ComponentModel;

namespace dnSpy.Contracts.Disassembly {
	/// <summary>
	/// x86/x64 disassembly settings
	/// </summary>
	public interface IDisassemblySettings : INotifyPropertyChanged {
		/// <summary>
		/// Prefixes are upper cased
		/// </summary>
		bool UpperCasePrefixes { get; set; }

		/// <summary>
		/// Mnemonics are upper cased
		/// </summary>
		bool UpperCaseMnemonics { get; set; }

		/// <summary>
		/// Registers are upper cased
		/// </summary>
		bool UpperCaseRegisters { get; set; }

		/// <summary>
		/// Keywords are upper cased (eg. BYTE PTR, SHORT)
		/// </summary>
		bool UpperCaseKeywords { get; set; }

		/// <summary>
		/// Upper case other stuff, eg. {z}, {sae}, {rd-sae}
		/// </summary>
		bool UpperCaseOther { get; set; }

		/// <summary>
		/// Everything is upper cased, except numbers and their prefixes/suffixes
		/// </summary>
		bool UpperCaseAll { get; set; }

		/// <summary>
		/// Character index (0-based) where the first operand is formatted. Can be set to 0 to format it immediately after the mnemonic.
		/// At least one space or tab is always added betewen the mnemonic and the first operand.
		/// </summary>
		int FirstOperandCharIndex { get; set; }

		/// <summary>
		/// Size of a tab character or &lt;= 0 to use spaces
		/// </summary>
		int TabSize { get; set; }

		/// <summary>
		/// Add a space after the operand separator, eg. "rax, rcx" vs "rax,rcx"
		/// </summary>
		bool SpaceAfterOperandSeparator { get; set; }

		/// <summary>
		/// Add a space between the memory expression and the brackets, eg. "[ rax ]" vs "[rax]"
		/// </summary>
		bool SpaceAfterMemoryBracket { get; set; }

		/// <summary>
		/// Add spaces between memory operand "+" and "-" operators, eg. "[rax + rcx]" vs "[rax+rcx]"
		/// </summary>
		bool SpaceBetweenMemoryAddOperators { get; set; }

		/// <summary>
		/// Add spaces between memory operand "*" operator, eg. "[rax * 4]" vs "[rax*4]"
		/// </summary>
		bool SpaceBetweenMemoryMulOperators { get; set; }

		/// <summary>
		/// Show memory operand scale value before the index register, eg. "[4*rax]" vs "[rax*4]"
		/// </summary>
		bool ScaleBeforeIndex { get; set; }

		/// <summary>
		/// Always show the scale value even if it's *1, eg. "[rax+rcx*1]" vs "[rax+rcx]"
		/// </summary>
		bool AlwaysShowScale { get; set; }

		/// <summary>
		/// Always show the effective segment register. If the option is false, only show the segment register if
		/// there's a segment override prefix. Eg. "ds:[rax]" vs "[rax]"
		/// </summary>
		bool AlwaysShowSegmentRegister { get; set; }

		/// <summary>
		/// Show zero displacements, eg. '[rcx*2+0]' vs '[rcx*2]'
		/// </summary>
		bool ShowZeroDisplacements { get; set; }

		/// <summary>
		/// Hex number prefix or null/empty string, eg. "0x"
		/// </summary>
		string HexPrefix { get; set; }

		/// <summary>
		/// Hex number suffix or null/empty string, eg. "h"
		/// </summary>
		string HexSuffix { get; set; }

		/// <summary>
		/// Size of a digit group
		/// </summary>
		int HexDigitGroupSize { get; set; }

		/// <summary>
		/// Decimal number prefix or null/empty string
		/// </summary>
		string DecimalPrefix { get; set; }

		/// <summary>
		/// Decimal number suffix or null/empty string
		/// </summary>
		string DecimalSuffix { get; set; }

		/// <summary>
		/// Size of a digit group
		/// </summary>
		int DecimalDigitGroupSize { get; set; }

		/// <summary>
		/// Octal number prefix or null/empty string
		/// </summary>
		string OctalPrefix { get; set; }

		/// <summary>
		/// Octal number suffix or null/empty string
		/// </summary>
		string OctalSuffix { get; set; }

		/// <summary>
		/// Size of a digit group
		/// </summary>
		int OctalDigitGroupSize { get; set; }

		/// <summary>
		/// Binary number prefix or null/empty string
		/// </summary>
		string BinaryPrefix { get; set; }

		/// <summary>
		/// Binary number suffix or null/empty string
		/// </summary>
		string BinarySuffix { get; set; }

		/// <summary>
		/// Size of a digit group
		/// </summary>
		int BinaryDigitGroupSize { get; set; }

		/// <summary>
		/// Digit separator or null/empty string
		/// </summary>
		string DigitSeparator { get; set; }

		/// <summary>
		/// Use shortest possible hexadecimal/octal/binary numbers, eg. 0xA/0Ah instead of eg. 0x0000000A/0000000Ah.
		/// This option has no effect on branch targets, use <see cref="ShortBranchNumbers"/>.
		/// </summary>
		bool ShortNumbers { get; set; }

		/// <summary>
		/// Use upper case hex digits
		/// </summary>
		bool UpperCaseHex { get; set; }

		/// <summary>
		/// Small hex numbers (-9 .. 9) are shown in decimal
		/// </summary>
		bool SmallHexNumbersInDecimal { get; set; }

		/// <summary>
		/// Add a leading zero to numbers if there's no prefix and the number begins with hex digits A-F, eg. Ah vs 0Ah
		/// </summary>
		bool AddLeadingZeroToHexNumbers { get; set; }

		/// <summary>
		/// Number base
		/// </summary>
		NumberBase NumberBase { get; set; }

		/// <summary>
		/// Don't add leading zeroes to branch offsets, eg. 'je 123h' vs 'je 00000123h'. Used by call near, call far, jmp near, jmp far, jcc, loop, loopcc, xbegin
		/// </summary>
		bool ShortBranchNumbers { get; set; }

		/// <summary>
		/// Show immediate operands as signed numbers, eg. 'mov eax,FFFFFFFF' vs 'mov eax,-1'
		/// </summary>
		bool SignedImmediateOperands { get; set; }

		/// <summary>
		/// Displacements are signed numbers, eg. 'mov al,[eax-2000h]' vs 'mov al,[eax+0FFFFE000h]'
		/// </summary>
		bool SignedMemoryDisplacements { get; set; }

		/// <summary>
		/// Sign extend memory displacements to the address size (16-bit, 32-bit, 64-bit), eg. 'mov al,[eax+12h]' vs 'mov al,[eax+00000012h]'
		/// </summary>
		bool SignExtendMemoryDisplacements { get; set; }

		/// <summary>
		/// Options that control if the memory size (eg. dword ptr) is shown or not.
		/// This is ignored by the GAS (AT&amp;T) formatter.
		/// </summary>
		MemorySizeOptions MemorySizeOptions { get; set; }

		/// <summary>
		/// true to show RIP relative addresses as '[rip+12345678h]', false to show RIP relative addresses as '[1029384756AFBECDh]'
		/// </summary>
		bool RipRelativeAddresses { get; set; }

		/// <summary>
		/// Shows near, short, etc if it's a branch instruction, eg. 'je short 1234h' vs 'je 1234h'
		/// </summary>
		bool ShowBranchSize { get; set; }

		/// <summary>
		/// Use pseudo instructions, eg. vcmpngesd vs vcmpsd+imm8
		/// </summary>
		bool UsePseudoOps { get; set; }

		/// <summary>
		/// Show the original value after the symbol name, eg. 'mov eax,[myfield (12345678)]' vs 'mov eax,[myfield]'
		/// </summary>
		bool ShowSymbolAddress { get; set; }
	}
}
