using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using EasyButtons;
using TMPro;
using UnityEngine;

namespace Argyle.UnclesToolkit
{
	public class ScriptLogger : ArgyleComponent
	{
		public TextMeshProUGUI _textUI;
		public bool _displayStackTrace;
		public bool _displayLogs;
		public bool _displayWarnings;
		public bool _displayErrors;
		public bool _clearOnEnable;

		public bool _isPaused;
		public bool _isSaved;
		public bool _isDisplayed;
		public bool _logAll = false;
		public List<string> Tags = new List<string>();


		private string _fullString;
		private string _newContent = string.Empty;
		private string _fileName = Path.Combine("Logs",$"ScriptLog-{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day} " +
		                                               $"{DateTime.Now.Hour}-{DateTime.Now.Minute}-{DateTime.Now.Second}.txt");
		private SecureStore _store;


		#region ==== Monobehavior ====------------------

		// Start is called before the first frame update
		void Awake()
		{
			_textUI.text = "";
			_store = new SecureStore();
		}
		private void OnEnable()
		{
			if(_clearOnEnable)
				Clear();
			
			Application.logMessageReceived += LogHandler;
		}
		private void OnDisable()
		{
			Application.logMessageReceived -= LogHandler;
		}

		private void Update()
		{
			if (_newContent.Length > 0)
			{
				_fullString += _newContent;
				if(_isDisplayed)
					Display();
				if (_isSaved)
					SaveToFile(_newContent);
				
				_newContent = String.Empty;
			}
		}

		#endregion -----------------/Monobehavior ====
		
		
		public void LogHandler(string message, string stackTrace, LogType logtype)
		{
			if(_isPaused)
				return;

			bool isLogged = false;
			foreach (var tag in Tags)
				if (message.Contains(tag))
					isLogged = true;
			
			if(!_logAll && !isLogged)
				return;
			
			string displayString = "";
			string stackTraceString = "...stack trace: " + stackTrace;
			
			//Add a prefix to differentiate log types. 
			switch (logtype)
			{
				case LogType.Error:
					if (!_displayErrors) return;
					displayString = "! ERROR: ";
					break;
				case LogType.Assert:
					if (!_displayLogs) return;
					displayString = "Assert: ";
					break;
				case LogType.Warning:
					if (!_displayWarnings) return;
					displayString = "WARNING: ";
					break;
				case LogType.Log:
					if(!_displayLogs) return;
					displayString = "Log: ";
					break;
				case LogType.Exception:
					if (!_displayErrors) return;
					displayString = "!!! EXCEPTION: ";
					break;
				default:
					break;
			}

			displayString += message + Environment.NewLine;

			if (_displayStackTrace)
			{
				displayString += stackTraceString;
			}


			UpdateLog(displayString);
		}

		public void UpdateLog(string logString)
		{
			_newContent += logString + Environment.NewLine;
		}

		public void Display()
		{
			if(_textUI == null)
				return;
			
			_textUI.text = _fullString.Length > 10000 ? _fullString.Substring(0, 10000) : _fullString;
		}

		public async void SaveToFile(string newStuff)
		{
			await Timing.WaitFor(() => { return _store != null; });
			_store.StoreAppendAsync(newStuff, _fileName);
		}
		

		public void Clear()
		{
			_fullString = "";
		}

		
		#region ==== Controls ====------------------

		public void ToggleLog() => ToggleLog(!_displayLogs);  
		public void ToggleLog(bool setTo) => _displayLogs = setTo;

		public void ToggleWarning() => ToggleWarning(!_displayWarnings);
		public void ToggleWarning(bool setTo) => _displayWarnings = setTo;

		public void ToggleError() => ToggleError(!_displayErrors);
		public void ToggleError(bool setTo) => _displayErrors = setTo;

		public void TogglePause() => TogglePause(!_isPaused);
		public void TogglePause(bool setTo) => _isPaused = setTo;

		public void ToggleLogAll() => ToggleLogAll(!_logAll);

		public void ToggleLogAll(bool setTo) => _logAll = setTo;

		#endregion -----------------/Controls ====

		[Button]
		public void Ping() => Log("Ping", new List<string>(){"Test"});
	}

}

