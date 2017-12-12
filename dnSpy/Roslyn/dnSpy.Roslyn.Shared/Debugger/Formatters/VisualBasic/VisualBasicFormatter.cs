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

using System.Globalization;
using System.Threading;
using dnSpy.Contracts.Debugger.CallStack;
using dnSpy.Contracts.Debugger.DotNet.Evaluation;
using dnSpy.Contracts.Debugger.DotNet.Evaluation.Formatters;
using dnSpy.Contracts.Debugger.Evaluation;
using dnSpy.Contracts.Text;
using dnSpy.Debugger.DotNet.Metadata;

namespace dnSpy.Roslyn.Shared.Debugger.Formatters.VisualBasic {
	[ExportDbgDotNetFormatter(DbgDotNetLanguageGuids.VisualBasic)]
	sealed class VisualBasicFormatter : LanguageFormatter {
		public override void FormatType(DbgEvaluationContext context, ITextColorWriter output, DmdType type, DbgDotNetValue value, DbgValueFormatterTypeOptions options, CultureInfo cultureInfo) =>
			new VisualBasicTypeFormatter(output, options.ToTypeFormatterOptions(), cultureInfo).Format(type, value);

		public override void FormatValue(DbgEvaluationContext context, ITextColorWriter output, DbgStackFrame frame, DbgDotNetValue value, DbgValueFormatterOptions options, CultureInfo cultureInfo, CancellationToken cancellationToken) =>
			new VisualBasicValueFormatter(output, context, frame, this, options.ToValueFormatterOptions(), cultureInfo, cancellationToken).Format(value);

		public override void Format(DbgEvaluationContext context, DbgStackFrame frame, ITextColorWriter output, DbgStackFrameFormatterOptions options, DbgValueFormatterOptions valueOptions, CultureInfo cultureInfo, CancellationToken cancellationToken) =>
			new VisualBasicStackFrameFormatter(output, context, this, options, valueOptions.ToValueFormatterOptions(), cultureInfo, cancellationToken).Format(frame);
	}
}
