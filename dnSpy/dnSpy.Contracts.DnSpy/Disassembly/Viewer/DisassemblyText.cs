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

namespace dnSpy.Contracts.Disassembly.Viewer {
	/// <summary>
	/// Text and color
	/// </summary>
	public readonly struct DisassemblyText {
		/// <summary>
		/// Text
		/// </summary>
		public string Text { get; }

		/// <summary>
		/// Colr
		/// </summary>
		public object Color { get; }

		/// <summary>
		/// Gets the reference or null
		/// </summary>
		public object Reference { get; }

		/// <summary>
		/// Gets the flags
		/// </summary>
		public DisassemblyReferenceFlags ReferenceFlags { get; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="color">Color</param>
		/// <param name="text">Text</param>
		/// <param name="reference">Reference or null</param>
		/// <param name="referenceFlags">Reference flags</param>
		public DisassemblyText(object color, string text, object reference, DisassemblyReferenceFlags referenceFlags) {
			Color = color ?? throw new ArgumentNullException(nameof(color));
			Text = text ?? throw new ArgumentNullException(nameof(text));
			Reference = reference;
			ReferenceFlags = referenceFlags;
		}
	}

	/// <summary>
	/// <see cref="DisassemblyText"/> reference flags
	/// </summary>
	public enum DisassemblyReferenceFlags {
		/// <summary>
		/// No bit is set
		/// </summary>
		None						= 0,

		/// <summary>
		/// It's a definition if set, else it's a reference to the definition
		/// </summary>
		Definition					= 0x00000001,

		/// <summary>
		/// It's a local definition or reference, eg. a label
		/// </summary>
		Local						= 0x00000002,
	}
}
