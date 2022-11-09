using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using API.Utility;
using Cysharp.Threading.Tasks;
using EasyButtons;
using UnityEngine;

namespace Argyle.UnclesToolkit
{
    public class ArgyleComponent : MonoBehaviour
    {
        private bool _isReferenceSet = false;
        private Transform _tForm;

        public bool IsAwakeFinished { get; private set; } = false;

        protected void Awake()
        {
            CancelObjectToken = CancelObjectSource.Token;
            WaitForPostStart();
            IsAwakeFinished = true;
        }

        protected UniTask WaitUntilAwakeIsFinishedAsync() => Timing.WaitFor(() => IsAwakeFinished);


        /// <summary>
        /// If using, make sure to call base.Start() last in any child classes. 
        /// </summary>
        public bool IsStartFinished { get; private set; } = false;

        /// <summary>
        /// If any functionality is waiting for start to be finished, make sure you call base.Start() last.
        /// </summary>
        private void Start()
        {
            IsStartFinished = true;
        }

        /// <summary>
        /// If using, make sure to call base.Start() last in any child classes. 
        /// </summary>
        public UniTask WaitUntilStartIsFinishedAsync() => Timing.WaitFor(() => IsStartFinished);


        private async void WaitForPostStart()
        {
            await UniTask.NextFrame();
            PostStart();
        }
        
        /// <summary>
        /// Runs after first frame (so after awake, onEnable, and start) for functions that rely on established setup.
        /// </summary>
        protected virtual async void PostStart()
        {
        }

        /// <summary>
        /// Pre-populated transform reference for improved performance.
        /// </summary>
        public Transform TForm
        {
            get
            {
                if(!_isReferenceSet)
                    SetReferences();
                
                return _tForm;
            }
        }
        
        
        private GameObject _go;

        /// <summary>
        /// Pre-populated Gameobject reference for improved performance.
        /// </summary>
        public GameObject GO
        {
            get
            {
                if (!_isReferenceSet)
                    SetReferences();

                return _go;
            }
        }


        private void SetReferences()
        {
            _tForm = transform;
            _go = gameObject;

            _isReferenceSet = true;
        }
        
        /// <summary>
        /// A list of all transforms in hierarchy from this to top level.
        /// NOTE: It is recalculated each call so avoid calling in loops. 
        /// </summary>
        public List<Transform> Ancestry
        {
            get
            {
                _ancestry = new List<Transform>();

                Transform t = TForm;
                while (true)
                {
                    _ancestry.Add(t);
                    if (t.parent == null)
                        break;

                    t = t.parent;
                }

                return _ancestry;
            }
        }
        private List<Transform> _ancestry;

        /// <summary>
        /// Simple universal gameobject Toggle. 
        /// </summary>
        public void ToggleGameObject()
        {
            GO.SetActive(!GO.activeSelf);
        }

        /// <summary>
        /// Simple universal component toggle. 
        /// </summary>
        public void ToggleComponent()
        {
            enabled = !enabled;
        }



        #region ==== Cancellation ====-----------------

        private static CancellationTokenSource CancelClassSource = new CancellationTokenSource();
        protected static CancellationToken CancelClassToken = CancelClassSource.Token;

        private CancellationTokenSource CancelObjectSource = new CancellationTokenSource();
        protected CancellationToken CancelObjectToken;

        /// <summary>
        /// If any of the cancellation tokens that apply to this objecty are cancelled, returns true.
        /// Recommended for clean stopping of async processes that might have multiple reasons to stop.
        /// NOTE: Only the default tokens from ArgyleComponent are included. Custom tokens must be considered seperately. 
        /// </summary>
        protected bool CancelAny => ThreadingUtility.QuitToken.IsCancellationRequested || 
                                    CancelClassToken.IsCancellationRequested ||
                                    CancelObjectToken.IsCancellationRequested;
        

        /// <summary>
        /// Cancel async functions for all objects of this class (inherited classes).
        /// Can access cancellation bia CancelClassToken or CancelAny.
        /// </summary>
        public void CancelClass()
        {
            CancelClassSource.Cancel();
        }

        /// <summary>
        /// Cancel async functions for this particular object (instance).
        /// Can access cancellation bia CancelObjectToken or CancelAny.
        /// </summary>
        public void CancelObject()
        {
            CancelObjectSource.Cancel();
        }

        
        #endregion ------------------/Cancellation ====
    }
}