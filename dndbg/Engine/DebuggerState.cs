﻿/*
    Copyright (C) 2014-2016 de4dot@gmail.com

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

using System.Collections.Generic;
using dndbg.COM.CorDebug;

namespace dndbg.Engine {
	public sealed class DebuggerState {
		/// <summary>
		/// Current process or null
		/// </summary>
		public readonly DnProcess Process;

		/// <summary>
		/// Current AppDomain or null
		/// </summary>
		public readonly DnAppDomain AppDomain;

		/// <summary>
		/// Current thread or null
		/// </summary>
		public readonly DnThread Thread;

		/// <summary>
		/// Event args or null if it was an event that wasn't generated by the CLR debugger, eg.
		/// <see cref="DebuggerPauseReason.UserBreak"/>.
		/// </summary>
		public readonly DebugCallbackEventArgs EventArgs;

		/// <summary>
		/// Gets the first IL frame in <see cref="Thread"/> or null if none
		/// </summary>
		public CorFrame ILFrame {
			get {
				if (Thread == null)
					return null;
				// Never cache the result since set-ip could be called which will invalidate all frames
				foreach (var chain in Thread.Chains) {
					foreach (var frame in chain.Frames) {
						if (frame.IsILFrame)
							return frame;
					}
				}
				return null;
			}
		}

		internal ICorDebugController Controller {
			get {
				ICorDebugController controller = null;
				if (controller == null && EventArgs != null)
					controller = EventArgs.CorDebugController;
				if (controller == null && AppDomain != null)
					controller = AppDomain.CorAppDomain.RawObject;
				if (controller == null && Process != null)
					controller = Process.CorProcess.RawObject;
				return controller;
			}
		}

		public DebuggerPauseState[] PauseStates {
			get { return pauseStates; }
			internal set { pauseStates = value ?? new DebuggerPauseState[0]; }
		}
		DebuggerPauseState[] pauseStates;

		public DebuggerState(DebugCallbackEventArgs e)
			: this(e, null, null, null) {
		}

		public DebuggerState(DebugCallbackEventArgs e, DnProcess process, DnAppDomain appDomain, DnThread thread) {
			this.EventArgs = e;
			this.PauseStates = null;
			this.Process = process;
			this.AppDomain = appDomain;
			this.Thread = thread;
		}

		public DebuggerPauseState GetPauseState(DebuggerPauseReason reason) {
			foreach (var state in pauseStates) {
				if (state.Reason == reason)
					return state;
			}
			return null;
		}

		public IEnumerable<DebuggerPauseState> GetPauseStates(DebuggerPauseReason reason) {
			foreach (var state in pauseStates) {
				if (state.Reason == reason)
					yield return state;
			}
		}
	}
}
