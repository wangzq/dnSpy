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
using System.ComponentModel;
using System.ComponentModel.Composition;
using dnSpy.Contracts.Disassembly;
using dnSpy.Contracts.Settings;

namespace dnSpy.Disassembly {
	class NasmDisassemblySettings : DisassemblySettings, INasmDisassemblySettings {
		public NasmDisassemblySettings() {
			HexSuffix = "h";
			OctalSuffix = "o";
			BinarySuffix = "b";
		}

		public bool ShowSignExtendedImmediateSize {
			get => showSignExtendedImmediateSize;
			set {
				if (value != showSignExtendedImmediateSize) {
					showSignExtendedImmediateSize = value;
					OnPropertyChanged(nameof(ShowSignExtendedImmediateSize));
				}
			}
		}
		bool showSignExtendedImmediateSize;

		public NasmDisassemblySettings Clone() => CopyTo(new NasmDisassemblySettings());

		public NasmDisassemblySettings CopyTo(NasmDisassemblySettings other) {
			if (other == null)
				throw new ArgumentNullException(nameof(other));
			base.CopyTo(other);
			other.ShowSignExtendedImmediateSize = ShowSignExtendedImmediateSize;
			return other;
		}
	}

	[Export(typeof(INasmDisassemblySettings))]
	[Export(typeof(NasmDisassemblySettingsImpl))]
	sealed class NasmDisassemblySettingsImpl : NasmDisassemblySettings {
		static readonly Guid SETTINGS_GUID = new Guid("2F066064-741B-454E-9D21-B04BCF802018");

		readonly ISettingsService settingsService;

		[ImportingConstructor]
		NasmDisassemblySettingsImpl(ISettingsService settingsService) {
			this.settingsService = settingsService;

			var sect = settingsService.GetOrCreateSection(SETTINGS_GUID);
			ReadSettings(sect);
			ShowSignExtendedImmediateSize = sect.Attribute<bool?>(nameof(ShowSignExtendedImmediateSize)) ?? ShowSignExtendedImmediateSize;

			PropertyChanged += OnPropertyChanged;
		}

		void OnPropertyChanged(object sender, PropertyChangedEventArgs e) => Save();

		void Save() {
			var sect = settingsService.RecreateSection(SETTINGS_GUID);
			WriteSettings(sect);
			sect.Attribute(nameof(ShowSignExtendedImmediateSize), ShowSignExtendedImmediateSize);
		}
	}
}
