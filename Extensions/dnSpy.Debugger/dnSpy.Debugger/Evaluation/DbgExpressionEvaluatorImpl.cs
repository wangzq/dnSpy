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
using System.Diagnostics;
using System.Threading;
using dnSpy.Contracts.Debugger;
using dnSpy.Contracts.Debugger.CallStack;
using dnSpy.Contracts.Debugger.Engine.Evaluation;
using dnSpy.Contracts.Debugger.Evaluation;

namespace dnSpy.Debugger.Evaluation {
	sealed class DbgExpressionEvaluatorImpl : DbgExpressionEvaluator {
		public override DbgLanguage Language { get; }

		readonly Guid runtimeKindGuid;
		readonly DbgEngineExpressionEvaluator engineExpressionEvaluator;

		public DbgExpressionEvaluatorImpl(DbgLanguage language, Guid runtimeKindGuid, DbgEngineExpressionEvaluator engineExpressionEvaluator) {
			Language = language ?? throw new ArgumentNullException(nameof(language));
			this.runtimeKindGuid = runtimeKindGuid;
			this.engineExpressionEvaluator = engineExpressionEvaluator ?? throw new ArgumentNullException(nameof(engineExpressionEvaluator));
		}

		DbgEvaluationResult CreateResult(DbgRuntime runtime, DbgEngineEvaluationResult result) {
			if (result.Error != null)
				return new DbgEvaluationResult(PredefinedEvaluationErrorMessagesHelper.GetErrorMessage(result.Error), result.Flags);
			try {
				var value = new DbgValueImpl(runtime, result.Value);
				runtime.CloseOnContinue(value);
				return new DbgEvaluationResult(value, result.Flags);
			}
			catch {
				runtime.Process.DbgManager.Close(result.Value);
				throw;
			}
		}

		DbgEEAssignmentResult CreateResult(DbgEngineEEAssignmentResult result) => new DbgEEAssignmentResult(result.Flags, PredefinedEvaluationErrorMessagesHelper.GetErrorMessage(result.Error));

		public override object CreateExpressionEvaluatorState() => engineExpressionEvaluator.CreateExpressionEvaluatorState();

		public override DbgEvaluationResult Evaluate(DbgEvaluationContext context, DbgStackFrame frame, string expression, DbgEvaluationOptions options, object state, CancellationToken cancellationToken) {
			if (context == null)
				throw new ArgumentNullException(nameof(context));
			if (!(context is DbgEvaluationContextImpl))
				throw new ArgumentException();
			if (context.Language != Language)
				throw new ArgumentException();
			if (context.Runtime.RuntimeKindGuid != runtimeKindGuid)
				throw new ArgumentException();
			if (expression == null)
				throw new ArgumentNullException(nameof(expression));
			Debug.Assert((context.Options & DbgEvaluationContextOptions.NoMethodBody) == 0, "Missing method debug info");
			return CreateResult(context.Runtime, engineExpressionEvaluator.Evaluate(context, frame, expression, options, state, cancellationToken));
		}

		public override DbgEEAssignmentResult Assign(DbgEvaluationContext context, DbgStackFrame frame, string expression, string valueExpression, DbgEvaluationOptions options, CancellationToken cancellationToken) {
			if (context == null)
				throw new ArgumentNullException(nameof(context));
			if (!(context is DbgEvaluationContextImpl))
				throw new ArgumentException();
			if (context.Language != Language)
				throw new ArgumentException();
			if (context.Runtime.RuntimeKindGuid != runtimeKindGuid)
				throw new ArgumentException();
			if (expression == null)
				throw new ArgumentNullException(nameof(expression));
			if (valueExpression == null)
				throw new ArgumentNullException(nameof(valueExpression));
			var result = engineExpressionEvaluator.Assign(context, frame, expression, valueExpression, options, cancellationToken);
			return CreateResult(result);
		}
	}
}
