// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerilogTraceWriter.cs" company="Hämmer Electronics">
//   Copyright (c) 2020 All rights reserved.
// </copyright>
// <summary>
//   A custom trace writer to write serialization information for Newtonsoft.Json to Serilog.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Darkengines {
	using System;
	using System.Diagnostics;
	using System.Diagnostics.CodeAnalysis;

	using Newtonsoft.Json.Serialization;

	using Serilog;

	public class SerilogTraceWriter : ITraceWriter {
		private static readonly ILogger Logger = Log.ForContext<SerilogTraceWriter>();

		public TraceLevel LevelFilter => TraceLevel.Verbose;
		public void Trace(TraceLevel level, string message, Exception ex) {
			switch (level) {
				case TraceLevel.Info:
					Logger.Information("Message: {@message}, Exception: {@exception}.", message, ex);
					break;
				case TraceLevel.Warning:
					Logger.Warning("Message: {@message}, Exception: {@exception}.", message, ex);
					break;
				case TraceLevel.Verbose:
					Logger.Verbose("Message: {@message}, Exception: {@exception}.", message, ex);
					break;
				case TraceLevel.Error:
					Logger.Error("Message: {@message}, Exception: {@exception}.", message, ex);
					break;
				case TraceLevel.Off: break;
			}
		}
	}
}