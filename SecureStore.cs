using System;
using System.IO;
using System.Linq;
using Argyle.APIClient;
using Argyle.UI.Dialogue;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Collections;
using UnityEngine;

namespace Argyle.UnclesToolkit
{
	/// <summary>
	/// At this point it stores in each platform but doesn't really encrypt or secure the data.
	/// TODO: figure out how to encrypt the data. Still needed.
	/// </summary>
	public class SecureStore : ISecureStore
	{
		private const string SecureFolder = "Secure";
	
		private string _directoryPath;
		private string FullPath(string fileName) => $"{DirectoryPath()}" +
		                                            $"{fileName}";

		private static readonly JsonSerializerSettings serializeSettings = new JsonSerializerSettings()
			{ReferenceLoopHandling = ReferenceLoopHandling.Ignore};
		
		private static readonly JsonSerializerSettings deserializeSettings = new JsonSerializerSettings()
		{ 
			TypeNameHandling = TypeNameHandling.All,
			NullValueHandling = NullValueHandling.Ignore,
		};

		public string DirectoryPath()
		{
			if (!Directory.Exists(_directoryPath))
				Directory.CreateDirectory(_directoryPath);
			
			return _directoryPath;
		}

		public DirectoryInfo DirectoryInfo
		{
			get
			{
				if (_directoryInfo == null)
					_directoryInfo = new DirectoryInfo(DirectoryPath());

				return _directoryInfo;
			}
		}

		private DirectoryInfo _directoryInfo;
		
		public SecureStore()
		{
			_directoryPath = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}" +
			                 $"{SecureFolder}{Path.DirectorySeparatorChar}";
		}


		/// <summary>
		/// Simple save object as encrypted json in binary.
		/// </summary>
		/// <param name="thing"></param>
		/// <param name="fileName"></param>
		public void Store(object thing, string fileName) //TODO: update interface to implement
		{
			Store(JsonConvert.SerializeObject(thing, serializeSettings), fileName);
		}
		
		/// <summary>
		/// Simple save object as encrypted json in binary.
		/// includes type to improve retrieval.
		/// </summary>
		/// <param name="thing"></param>
		/// <param name="type"></param>
		/// <param name="fileName"></param>
		public void Store(object thing, string fileName, Type type = null)
		{
			if(type == null)
				Store(JsonConvert.SerializeObject(thing, serializeSettings), fileName);
			else
				Store(JsonConvert.SerializeObject(thing, type, serializeSettings), fileName);
		}

		public void Store(string thing, string fileName)
		{
			//make sure the location exists
			Directory.CreateDirectory(FullPath(""));
			
			File.WriteAllText(FullPath(fileName), thing);
			
			// //write to new file there
			// using (BinaryWriter bw = new BinaryWriter(
			// 	File.Open(FullPath(fileName), FileMode.Create)))
			// {
			// 	bw.Write(thing);//.EnCrypt());
			// }
		}

		public void StoreBinary(NativeArray<byte> data, string fileName)
		{
			Debug.Log($"ARWorldMap has {data.Length} bytes.");

			var file = File.Open(FullPath(fileName), FileMode.Create);
			var writer = new BinaryWriter(file);
			writer.Write(data.ToArray());
			writer.Close();
			data.Dispose();
			Debug.Log($"ARWorldMap written to {FullPath(fileName)}");

		}

		public async UniTask<NativeArray<byte>> RetrieveBinaryAsync(string fileName)
		{
			var file = File.Open(FullPath(fileName), FileMode.Open);

			Debug.Log($"Reading {FullPath(fileName)}...");

			var binaryReader = new BinaryReader(file);
			var bytes = await UniTask.RunOnThreadPool(()=> binaryReader.ReadBytes((int) file.Length));

			var data = new NativeArray<byte>(bytes.Length, Allocator.Temp);
			data.CopyFrom(bytes.ToArray());
			
			return data;
		}

		public T Retrieve<T>(string fileName) where T : class
		{
			var results = Retrieve(fileName);
			if(results != null)
				return JsonConvert.DeserializeObject<T>(results, deserializeSettings);
			
			//if not there or empty
			return null;
		}

		public string Retrieve(string fileName)
		{
			if (File.Exists(FullPath(fileName)))
				return File.ReadAllText(FullPath(fileName));
			
			//else
			Debug.LogWarning($"{FullPath(fileName)} not found in filestructure. Returning null");
			return null;
		}

		public bool Exists(string fileName)
		{
			return File.Exists(FullPath(fileName));
		}

		public bool Delete(string fileName)
		{
			if(Exists(fileName))
			{
				File.Delete(FullPath(fileName));
				return true;
			}

			return false;
		}

		/// <summary>
		/// Extreme measure to delete all store content and start over. 
		/// </summary>
		/// <returns>Was the delete successful?</returns>
		public bool Clear()
		{
			Directory.Delete(DirectoryPath(),true);

			if(Directory.GetFiles(DirectoryPath()).Length == 0)
				return true;
			else
			{
				DialogueManager.ErrorDialogue.Show(
					"Unable to fully delete app data. " +
					"Contents might be protected by the OS. Uninstall may be required.");
				return false;
			}
		}
	}
}
