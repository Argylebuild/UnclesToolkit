using System;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using UnityEngine;

namespace Argyle.UnclesToolkit
{
	public class SceneLogger : ArgyleComponent
	{
		public TextMeshProUGUI textUI;
		//public Scrollbar scrollbar;
		//public Text textUI;
		public bool displayStackTrace;
		public bool displayLogs;
		public bool displayWarnings;
		public bool displayErrors;
		public bool clearOnEnable;

		public bool isPaused;

		// Start is called before the first frame update
		void Awake()
		{
			
			textUI.text = "";
		}
		private void OnEnable()
		{
			if(clearOnEnable)
				Clear();
			
			Application.logMessageReceived += LogHandler;
		}
		private void OnDisable()
		{
			Application.logMessageReceived -= LogHandler;
		}

		public void LogHandler(string message, string stackTrace, LogType logtype)
		{
			if(isPaused)
				return;
			
			string displayString = "";
			string stackTraceString = "";
			//Add a prefix to differentiate log types. 
			switch (logtype)
			{
				case LogType.Error:
					if (!displayErrors) return;
					displayString = "! ERROR: ";
					stackTraceString = "...stack trace: " + stackTrace;
					break;
				case LogType.Assert:
					if (!displayLogs) return;
					displayString = "Assert: ";
					break;
				case LogType.Warning:
					if (!displayWarnings) return;
					displayString = "WARNING: ";
					break;
				case LogType.Log:
					if(!displayLogs) return;
					displayString = "Log: ";
					break;
				case LogType.Exception:
					if (!displayErrors) return;
					displayString = "!!! EXCEPTION: ";
					stackTraceString = "...stack trace: " + stackTrace;
					break;
				default:
					break;
			}

			displayString += message + Environment.NewLine;

			if (displayStackTrace)
			{
				displayString += stackTraceString;
			}


			Log(displayString);
		}

		public void Log(string logString)
		{
			textUI.text += logString + Environment.NewLine;
			//scrollbar.value = 0;
		}

		public void Clear()
		{
			textUI.text = "";
		}

		public void ToggleLog(Interactable checkbox)
		{
			displayLogs = checkbox.IsToggled;
		}

		public void ToggleWarning(Interactable checkbox)
		{
			displayWarnings = checkbox.IsToggled;
		}

		public void ToggleError(Interactable checkbox)
		{
			displayErrors = checkbox.IsToggled;
		}

		public void TogglePause(Interactable checkbox)
		{
			isPaused = checkbox.IsToggled;
		}
	}

}

