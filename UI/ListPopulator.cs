using Argyle.UnclesToolkit;
using Argyle.UnclesToolkit.Control.MrtkPlaceholder;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Argyle.Utilities.UI
{
    public abstract class ListPopulator : ArgyleComponent
    {
        // ScrollingObjectCollection-related Components
        [SerializeField] protected ScrollingObjectCollection _scrollingCollection;  // Serialized for manually adding the component in the Inspector
        protected virtual Transform _collectionContainer => _scrollingCollection.transform.Find( "Container" );

        // MonoBehaviour Methods
        private void Awake() => Initialize();

        // Virtual Methods (Task return types cannot be abstract)
        protected virtual async UniTask MakeListAsync() { }   // Override to create a list
        protected virtual async UniTask ClearListAsync() { }  // Override to reset the list
        protected virtual async UniTask RefreshListAsync()
        { 
            await ClearListAsync();
            await MakeListAsync();
        }

        // Public Methods
        public void MakeList() => MakeListAsync();  // Created for event subscription (cannot add Task return types to void return type events)
        public void ClearList() => ClearListAsync();
        public void RefreshList() => RefreshListAsync();

        // Abstract Methods
        public abstract void Initialize();  // Override for subscriptions, initializations. Invoked at Awake()
    }
}