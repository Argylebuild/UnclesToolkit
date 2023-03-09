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
		
		
		/// <summary>
		/// Fills in the path for a given filename according to the platform
		/// and creates the directory if it doesn't exist.
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		private string FullPath(string fileName)
		{
			string fullpath = Path.Combine(DirectoryPath(), fileName);

			try
			{
				string directory = Path.GetDirectoryName(fullpath);
				if(!Directory.Exists(directory))
					Directory.CreateDirectory(directory);
			}
			catch (Exception e)
			{
				DialogueManager.ErrorDialogue.Show($"SecureStore.FullPath() Unable to create directory. \n" +
				                                   $"{e.Message} \n {e.StackTrace}");
			}
			
			return fullpath;
		}

		private static readonly JsonSerializerSettings serializeSettings = new JsonSerializerSettings()
			{ReferenceLoopHandling = ReferenceLoopHandling.Ignore};
		
		private static readonly JsonSerializerSettings deserializeSettings = new JsonSerializerSettings()
		{ 
			TypeNameHandling = TypeNameHandling.All,
			NullValueHandling = NullValueHandling.Ignore,
		};

		
		/// <summary>
		/// The base directory for the secure store. Does not include subdirectories or filename.
		/// </summary>
		/// <returns></returns>
		public string DirectoryPath()
		{
			try
			{
				if (!Directory.Exists(_directoryPath))
					Directory.CreateDirectory(_directoryPath);
			}
			catch (Exception e)
			{
				DialogueManager.ErrorDialogue.Show("SecureStore.FullPath() Unable to create directory.");
			}
			
			return _directoryPath;
		}

		/// <summary>
		/// The base directory for the secure store. Does not include subdirectories or filename.
		/// </summary>
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
		
		
		/// <summary>
		/// Ctor sets up the paths that don't like being or auto.
		/// </summary>
		public SecureStore()
		{
			_directoryPath = $"{Application.persistentDataPath}{Path.DirectorySeparatorChar}" +
			                 $"{SecureFolder}{Path.DirectorySeparatorChar}";
		}

		#region ==== Store ====------------------


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
		/// Simple save object as sjon
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
			try
			{
				File.WriteAllText(FullPath(fileName), thing);
			}
			catch (Exception e)
			{
				DialogueManager.ErrorDialogue.Show($"{e.Message} \n {e.Source} \n {e.Data} \n {e.StackTrace}");
			}
		}

		/// <summary>
		/// Store nativeArray as binary on disk.
		/// StoreBinaryAsync is preferred.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public bool StoreBinary(NativeArray<byte> data, string fileName)
		{
			bool success = false;

			try
			{
				var file = File.Open(FullPath(fileName), FileMode.Create);
				var writer = new BinaryWriter(file);
				writer.Write(data.ToArray());
				writer.Close();
				data.Dispose();
				Debug.Log($"{fileName} written to {FullPath(fileName)}");
				
				success = true;
			}
			catch (Exception e)
			{
				DialogueManager.ErrorDialogue.Show($"{e.Message} \n {e.Source} \n {e.Data} \n {e.StackTrace}");
			}

			return success;
		}
		
		/// <summary>
		/// Saves byte array as binary on disk.
		/// Filename may contain subdirectories. Method confirms directory path before saving. 
		/// </summary>
		/// <param name="data"></param>
		/// <param name="fileName"></param>
		/// <returns></returns>
		public async UniTask<bool> StoreBinaryAsync(byte[] data, string fileName)
		{
			Debug.Log($"ARWorldMap has {data.Length} bytes.");

			bool success = false;

			try
			{
				using (FileStream fs = new FileStream(FullPath(fileName), FileMode.Create))
				{
					await fs.WriteAsync(data, 0, data.Length);
					success = true;
				}
			}
			catch (Exception e)
			{
				DialogueManager.ErrorDialogue.Show($"{e.Message} \n {e.Source} \n {e.Data} \n {e.StackTrace}");
			}

			return success;
		}

		/// <summary>
		/// Saves new info to existing file on disk.
		/// Useful for logs and other appendable data.
		/// </summary>
		/// <param name="thing"></param>
		/// <param name="fileName"></param>
		public async UniTask StoreAppendAsync(string thing, string fileName) => 
			await File.AppendAllTextAsync(FullPath(fileName), thing);
		

		#endregion -----------------/Store ====

		
		
		#region ==== Retrieve ====------------------


		public async UniTask<byte[]> RetrieveBinaryAsync(string fileName)
		{
			string fullPath = FullPath(fileName);

			if (!File.Exists(fullPath))
			{
				Debug.Log($"File {fullPath} does not exist.");
				return null;
			}

			var file = File.Open(fullPath, FileMode.Open);

			Debug.Log($"Reading {fullPath}...");
			var binaryReader = new BinaryReader(file);
			var bytes = await UniTask.RunOnThreadPool(()=> binaryReader.ReadBytes((int) file.Length));
			binaryReader.Dispose();

			return bytes;
		}

		public async UniTask<NativeArray<byte>> RetrieveNativeBinaryAsync(string fileName)
		{
			var bytes = await RetrieveBinaryAsync(fileName);
			
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

		#endregion -----------------/Retrieve ====

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
