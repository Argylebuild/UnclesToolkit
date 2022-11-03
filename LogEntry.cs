using System;
using UnityEngine;

namespace Argyle.UnclesToolkit
{
	[Serializable]
	class LogEntry
	{
		[SerializeField] public DateTime dateTime;
		[SerializeField] public string logType;
		[SerializeField] public string message;
		[SerializeField] public string stackTrace;

		public LogEntry(string message, string stackTrace, LogType logtype)
		{
			dateTime = DateTime.Now;
			this.message = message;
			this.logType = logtype.ToString();

			if (logtype == LogType.Error || logtype == LogType.Exception)
				this.stackTrace = stackTrace;
			
		}
	}
}