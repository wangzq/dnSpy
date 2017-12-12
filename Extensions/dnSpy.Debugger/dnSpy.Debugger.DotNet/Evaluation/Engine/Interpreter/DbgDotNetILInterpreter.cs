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
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Threading;
using dnSpy.Contracts.Debugger;
using dnSpy.Contracts.Debugger.CallStack;
using dnSpy.Contracts.Debugger.DotNet.Evaluation;
using dnSpy.Contracts.Debugger.Engine.Evaluation;
using dnSpy.Contracts.Debugger.Evaluation;
using dnSpy.Debugger.DotNet.Interpreter;
using dnSpy.Debugger.DotNet.Metadata;

namespace dnSpy.Debugger.DotNet.Evaluation.Engine.Interpreter {
	abstract class DbgDotNetILInterpreter {
		public abstract DbgDotNetILInterpreterState CreateState(byte[] assembly);

		public DbgDotNetValueResult Execute(DbgEvaluationContext context, DbgStackFrame frame, DbgDotNetILInterpreterState state, string typeName, string methodName, DbgEvaluationOptions options, out DmdType expectedType, CancellationToken cancellationToken) {
			var frameMethod = frame.Runtime.GetDotNetRuntime().GetFrameMethod(context, frame, cancellationToken) ?? throw new InvalidOperationException();
			var genericTypeArguments = frameMethod.ReflectedType.GetGenericArguments();
			var genericMethodArguments = frameMethod.GetGenericArguments();
			return Execute(context, frame, genericTypeArguments, genericMethodArguments, null, null, state, typeName, methodName, options, out expectedType, cancellationToken);
		}

		public abstract DbgDotNetValueResult Execute(DbgEvaluationContext context, DbgStackFrame frame, IList<DmdType> genericTypeArguments, IList<DmdType> genericMethodArguments, VariablesProvider argumentsProvider, VariablesProvider localsProvider, DbgDotNetILInterpreterState state, string typeName, string methodName, DbgEvaluationOptions options, out DmdType expectedType, CancellationToken cancellationToken);
	}

	abstract class DbgDotNetILInterpreterState {
	}

	[Export(typeof(DbgDotNetILInterpreter))]
	sealed class DbgDotNetILInterpreterImpl : DbgDotNetILInterpreter {
		sealed class DbgDotNetILInterpreterStateImpl : DbgDotNetILInterpreterState {
			public ILVM ILVM { get; }

			readonly List<(int appDomainId, string typeName, string methodName, IList<DmdType> genericTypeArguments, IList<DmdType> genericMethodArguments, MethodInfoState methodState)> methodStates;
			readonly List<(int appDomainId, DmdAssembly assembly)> assemblies;
			readonly byte[] assemblyBytes;

			public DbgDotNetILInterpreterStateImpl(byte[] assemblyBytes) {
				ILVM = ILVMFactory.Create();
				methodStates = new List<(int, string, string, IList<DmdType>, IList<DmdType>, MethodInfoState)>();
				assemblies = new List<(int, DmdAssembly)>();
				this.assemblyBytes = assemblyBytes ?? throw new ArgumentNullException(nameof(assemblyBytes));
			}

			public MethodInfoState GetOrCreateMethodInfoState(DbgAppDomain appDomain, string typeName, string methodName, IList<DmdType> genericTypeArguments, IList<DmdType> genericMethodArguments) {
				int appDomainId = appDomain.Id;
				foreach (var info in methodStates) {
					if (methodName == info.methodName && typeName == info.typeName && info.appDomainId == appDomainId && Equals(info.genericTypeArguments, genericTypeArguments) && Equals(info.genericMethodArguments, genericMethodArguments))
						return info.methodState;
				}

				var methodState = new MethodInfoState();
				methodStates.Add((appDomain.Id, typeName, methodName, genericTypeArguments, genericMethodArguments, methodState));
				return methodState;
			}

			bool Equals(IList<DmdType> a, IList<DmdType> b) {
				if (a == b)
					return true;
				if (a.Count != b.Count)
					return false;
				for (int i = 0; i < a.Count; i++) {
					if (a[i] != b[i])
						return false;
				}
				return true;
			}

			public DmdAssembly GetOrCreateReflectionAssembly(DbgAppDomain appDomain) {
				foreach (var info in assemblies) {
					if (info.appDomainId == appDomain.Id)
						return info.assembly;
				}
				var reflectionAppDomain = appDomain.GetReflectionAppDomain();
				if (reflectionAppDomain == null)
					throw new InvalidOperationException();

				const bool isInMemory = true;
				const bool isDynamic = false;
				var fullyQualifiedName = DmdModule.GetFullyQualifiedName(isInMemory, isDynamic, fullyQualifiedName: null);
				Func<DmdLazyMetadataBytes> getMetadata = () => new DmdLazyMetadataBytesArray(assemblyBytes, isFileLayout: true);
				var assembly = reflectionAppDomain.CreateSyntheticAssembly(getMetadata, isInMemory, isDynamic, fullyQualifiedName, string.Empty, null);

				assemblies.Add((appDomain.Id, assembly));
				return assembly;
			}
		}

		readonly DebuggerRuntimeFactory debuggerRuntimeFactory;

		[ImportingConstructor]
		DbgDotNetILInterpreterImpl(DebuggerRuntimeFactory debuggerRuntimeFactory) =>
			this.debuggerRuntimeFactory = debuggerRuntimeFactory;

		sealed class MethodInfoState {
			public ILVMExecuteState ILVMExecuteState { get; set; }
			public DmdType ExpectedType { get; set; }
			public DmdMethodBody RealMethodBody { get; set; }
		}

		public override DbgDotNetILInterpreterState CreateState(byte[] assembly) {
			if (assembly == null)
				throw new ArgumentNullException(nameof(assembly));
			return new DbgDotNetILInterpreterStateImpl(assembly);
		}

		public override DbgDotNetValueResult Execute(DbgEvaluationContext context, DbgStackFrame frame, IList<DmdType> genericTypeArguments, IList<DmdType> genericMethodArguments, VariablesProvider argumentsProvider, VariablesProvider localsProvider, DbgDotNetILInterpreterState state, string typeName, string methodName, DbgEvaluationOptions options, out DmdType expectedType, CancellationToken cancellationToken) {
			var stateImpl = (DbgDotNetILInterpreterStateImpl)state;

			var debuggerRuntime = debuggerRuntimeFactory.Create(context.Runtime);
			debuggerRuntime.Runtime.Dispatcher.VerifyAccess();

			var appDomain = frame.AppDomain;
			if (appDomain == null)
				throw new ArgumentException("No AppDomain available");

			var reflectionAssembly = stateImpl.GetOrCreateReflectionAssembly(appDomain);
			var methodState = stateImpl.GetOrCreateMethodInfoState(appDomain, typeName, methodName, genericTypeArguments, genericMethodArguments);

			DbgDotNetValueResult result = default;
			using (reflectionAssembly.AppDomain.AddTemporaryAssembly(reflectionAssembly)) {
				var ilvmState = methodState.ILVMExecuteState;
				if (ilvmState == null) {
					// This could fail so get it first
					var realMethod = context.Runtime.GetDotNetRuntime().GetFrameMethod(context, frame, cancellationToken) ?? throw new InvalidOperationException();

					var type = reflectionAssembly.GetTypeThrow(typeName);
					Debug.Assert(type.GetGenericArguments().Count == genericTypeArguments.Count);
					if (genericTypeArguments.Count != 0)
						type = type.MakeGenericType(genericTypeArguments);
					const DmdBindingFlags bindingFlags = DmdBindingFlags.Static | DmdBindingFlags.Public | DmdBindingFlags.NonPublic;
					var method = type.GetMethod(methodName, bindingFlags) ?? throw new InvalidOperationException();
					Debug.Assert(method.GetGenericArguments().Count == genericMethodArguments.Count);
					if (genericMethodArguments.Count != 0)
						method = method.MakeGenericMethod(genericMethodArguments);

					methodState.ExpectedType = method.ReturnType;
					ilvmState = stateImpl.ILVM.CreateExecuteState(method);
					methodState.ILVMExecuteState = ilvmState;
					methodState.RealMethodBody = realMethod.GetMethodBody();
				}

				expectedType = methodState.ExpectedType;
				debuggerRuntime.Initialize(context, frame, methodState.RealMethodBody, argumentsProvider, localsProvider, (options & DbgEvaluationOptions.NoFuncEval) == 0, cancellationToken);
				try {
					var execResult = stateImpl.ILVM.Execute(debuggerRuntime, ilvmState);
					var resultValue = debuggerRuntime.GetDotNetValue(execResult, expectedType);
					if (expectedType == expectedType.AppDomain.System_Void) {
						resultValue.Dispose();
						resultValue = new NoResultValue(expectedType.AppDomain);
					}
					result = new DbgDotNetValueResult(resultValue, valueIsException: false);
				}
				catch (InterpreterException ie) {
					result = new DbgDotNetValueResult(GetErrorMessage(ie.Kind));
				}
				catch (InterpreterMessageException ime) {
					result = new DbgDotNetValueResult(ime.Message);
				}
				catch (InterpreterThrownExceptionException thrownEx) {
					Debug.Assert(thrownEx.ThrownValue is DbgDotNetValue);
					if (thrownEx.ThrownValue is DbgDotNetValue thrownValue)
						result = new DbgDotNetValueResult(thrownValue, valueIsException: true);
					else
						result = new DbgDotNetValueResult(PredefinedEvaluationErrorMessages.InternalDebuggerError);
				}
				catch (Exception ex) when (ExceptionUtils.IsInternalDebuggerError(ex)) {
					result = new DbgDotNetValueResult(PredefinedEvaluationErrorMessages.InternalDebuggerError);
				}
				finally {
					debuggerRuntime.Clear(result.Value);
				}
			}

			return result;
		}

		sealed class NoResultValue : DbgDotNetValue {
			public override DmdType Type { get; }
			public NoResultValue(DmdAppDomain appDomain) => Type = appDomain.System_Void;
			public override DbgDotNetRawValue GetRawValue() => new DbgDotNetRawValue(DbgSimpleValueType.Void);
		}

		static string GetErrorMessage(InterpreterExceptionKind kind) {
			switch (kind) {
			case InterpreterExceptionKind.TooManyInstructions:
			case InterpreterExceptionKind.InvalidMethodBody:
			case InterpreterExceptionKind.InstructionNotSupported:
			default:
				return PredefinedEvaluationErrorMessages.InternalDebuggerError;
			}
		}
	}
}
