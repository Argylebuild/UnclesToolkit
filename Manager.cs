using Argyle.Utilities;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Argyle.UnclesToolkit
{
    /// <summary>
    /// Manager is used as a base class for classes that would want to be static
    /// but need to interface with the Unity scene and Inspector.
    /// Must be placed in the scene before an instance can be referenced. 
    /// </summary>
    /// <typeparam name="T">Derived class</typeparam>
    public class Manager<T> : ArgyleComponent where T : ArgyleComponent
    {
        protected SecureStore _store;

        public SecureStore Store
        {
            get
            {
                if (_store == null)
                    _store = new SecureStore();

                return _store;
            }
        }
        
        /// <summary>
        /// The single instance of this class.
        /// Must be setup in scene to reference the object from the static class.
        /// </summary>
        public static T Instance
        {
            get
            {
                if(_instance != null)
                    return _instance;
                
                
                //else
                Debug.LogError($"No instance found for {typeof(T)}. " +
                               $"Place instance in scene and setup parameters before continuing.");

                return null;
            }
        }
        private static T _instance;

        /// <summary>
        /// For very early calls where instance might not have been setup (like OnEnable),
        /// this lets us skip the call or wait until it's ready. 
        /// </summary>
        public static bool IsInitialized => _isInitialized;
        private static bool _isInitialized;

        /// <summary>
        /// Async method for getting instance can be called at any time and will return when it's ready. 
        /// </summary>
        /// <returns></returns>
        public async static UniTask<T> GetInstanceAsync()
        {
            while (_isInitialized == false)
            {
                await UniTask.NextFrame();
            }

            return Instance;
        }
        
        
        protected new void Awake()
        {
            base.Awake();
            Setup();
        }

        private void Setup()
        {
            if(_instance == null)
                _instance = this as T;
            _isInitialized = true;

            _store = new SecureStore();
        } 
    }
}
