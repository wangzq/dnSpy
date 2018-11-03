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
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using dnSpy.Contracts.Disassembly;
using dnSpy.Contracts.Disassembly.Viewer;
using Iced.Intel;

namespace dnSpy.Disassembly.Viewer {
	[Export(typeof(X86DisassemblyContentProviderFactoryDependencies))]
	sealed class X86DisassemblyContentProviderFactoryDependencies {
		public DisassemblyContentSettings DisasmSettings { get; }
		public IMasmDisassemblySettings MasmSettings { get; }
		public INasmDisassemblySettings NasmSettings { get; }
		public IGasDisassemblySettings GasSettings { get; }

		[ImportingConstructor]
		X86DisassemblyContentProviderFactoryDependencies(DisassemblyContentSettingsImpl disasm, MasmDisassemblySettingsImpl masm, NasmDisassemblySettingsImpl nasm, GasDisassemblySettingsImpl gas) {
			DisasmSettings = disasm;
			MasmSettings = masm;
			NasmSettings = nasm;
			GasSettings = gas;
		}
	}

	readonly struct X86DisassemblyContentProviderFactory {
		readonly X86DisassemblyContentProviderFactoryDependencies deps;
		readonly int bitness;
		readonly DisassemblyContentFormatterOptions formatterOptions;
		readonly Contracts.Disassembly.ISymbolResolver symbolResolver;
		readonly string header;
		readonly NativeCodeOptimization optimization;
		readonly NativeCodeBlock[] blocks;
		readonly X86NativeCodeInfo codeInfo;
		readonly NativeVariableInfo[] variableInfo;
		readonly string methodName;

		public X86DisassemblyContentProviderFactory(X86DisassemblyContentProviderFactoryDependencies deps, int bitness, DisassemblyContentFormatterOptions formatterOptions, Contracts.Disassembly.ISymbolResolver symbolResolver, string header, NativeCodeOptimization optimization, NativeCodeBlock[] blocks, NativeCodeInfo codeInfo, NativeVariableInfo[] variableInfo, string methodName) {
			if (blocks == null)
				throw new ArgumentNullException(nameof(blocks));
			this.deps = deps ?? throw new ArgumentNullException(nameof(deps));
			this.bitness = bitness;
			this.formatterOptions = formatterOptions;
			this.symbolResolver = symbolResolver;
			this.header = header;
			this.optimization = optimization;
			this.blocks = blocks ?? throw new ArgumentNullException(nameof(blocks));
			this.codeInfo = codeInfo as X86NativeCodeInfo;
			this.variableInfo = variableInfo;
			this.methodName = methodName;
		}

		public DisassemblyContentProvider Create() {
			var blocks = X86BlockFactory.Create(bitness, this.blocks);
			var cachedSymResolver = new CachedSymbolResolver();
			if (symbolResolver != null) {
				var addresses = GetPossibleSymbolAddresses(blocks);
				if (addresses.Length != 0) {
					var symbolResolverResults = new SymbolResolverResult[addresses.Length];
					symbolResolver.Resolve(addresses, symbolResolverResults);
					cachedSymResolver.AddSymbols(addresses, symbolResolverResults, fakeSymbol: false);
				}
			}
			foreach (var block in blocks) {
				if (!string.IsNullOrEmpty(block.Label))
					cachedSymResolver.AddSymbol(block.Address, new SymbolResolverResult(SymbolKindUtils.ToSymbolKind(block.LabelKind), block.Label, block.Address), fakeSymbol: true);
			}
			return new X86DisassemblyContentProvider(bitness, cachedSymResolver, deps.DisasmSettings, deps.MasmSettings, deps.NasmSettings, deps.GasSettings, formatterOptions, header, optimization, blocks, codeInfo, variableInfo, methodName);
		}

		static ulong[] GetPossibleSymbolAddresses(X86Block[] blocks) {
			var addresses = new HashSet<ulong>();
			foreach (var block in blocks) {
				addresses.Add(block.Address);
				foreach (var instrInfo in block.Instructions) {
					var instr = instrInfo.Instruction;
					int opCount = instr.OpCount;
					// Find all 16/32/64-bit immediates, and all branch targets
					for (int i = 0; i < opCount; i++) {
						switch (instr.GetOpKind(i)) {
						case OpKind.NearBranch16:
						case OpKind.NearBranch32:
						case OpKind.NearBranch64:
							addresses.Add(instr.NearBranchTarget);
							break;

						case OpKind.FarBranch16:
						case OpKind.FarBranch32:
							addresses.Add(instr.FarBranch32);
							addresses.Add(instr.FarBranchSelector);
							break;

						case OpKind.Immediate16:
						case OpKind.Immediate32:
						case OpKind.Immediate8to16:
						case OpKind.Immediate8to32:
						case OpKind.Immediate64:
						case OpKind.Immediate8to64:
						case OpKind.Immediate32to64:
							addresses.Add(instr.GetImmediate(i));
							break;

						case OpKind.Memory64:
							addresses.Add(instr.MemoryAddress64);
							break;

						case OpKind.Memory:
							switch (instr.MemoryDisplSize) {
							case 2:
							case 4:
								addresses.Add(instr.MemoryDisplacement);
								break;

							case 8:
								addresses.Add((ulong)(int)instr.MemoryDisplacement);
								break;
							}
							break;
						}
					}
				}
			}
			return addresses.Count == 0 ? Array.Empty<ulong>() : addresses.ToArray();
		}
	}
}
