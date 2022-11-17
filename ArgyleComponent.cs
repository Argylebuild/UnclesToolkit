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
            WaitForPostStart();
            IsAwakeFinished = true;
        }

        /// <summary>
        /// If any functionality is waiting for start to be finished, make sure you call base.Start() last.
        /// </summary>
        private void Start()
        {
            IsStartFinished = true;
        }

        protected void OnDestroy()
        {
            CancelObject();
        }


        protected UniTask WaitUntilAwakeIsFinishedAsync() => Timing.WaitFor(() => IsAwakeFinished);


        /// <summary>
        /// If using, make sure to call base.Start() last in any child classes. 
        /// </summary>
        public bool IsStartFinished { get; private set; } = false;


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

        // private static HashSet<CancellationTokenSource> CancelClassSources = new HashSet<CancellationTokenSource>();

        private HashSet<CancellationTokenSource> CancelObjectSources = new HashSet<CancellationTokenSource>();
        

        // /// <summary>
        // /// Cancel async functions added to this class (inherited classes).
        // /// </summary>
        // public static void CancelClass()
        // {
        //     foreach (var source in CancelClassSources)
        //     {
        //         source.Cancel();
        //     }
        // }
        //
        // public static CancellationToken AddCancelToClass()
        // {
        //     CancellationTokenSource source = new CancellationTokenSource();
        //     CancelClassSources.Add(source);
        //
        //     return source.Token;
        // }

        /// <summary>
        /// Cancel async functions for this particular object (instance).
        /// Can access cancellation bia CancelObjectToken or CancelAny.
        /// </summary>
        public void CancelObject()
        {
            foreach (var source in CancelObjectSources)
            {
                source.Cancel();
            }
        }

        protected CancellationToken AddCancelToObject()
        {
            CancellationTokenSource source = new CancellationTokenSource();
            CancelObjectSources.Add(source);

            return source.Token;
        }

        protected void ReleaseCancel(CancellationToken token)
        {
            // foreach (var source in CancelClassSources)
            // {
            //     if (source.Token == token)
            //     {
            //         CancelClassSources.Remove(source);
            //         return;
            //     }
            // }
            foreach (var source in CancelObjectSources)
            {
                if (source.Token == token)
                {
                    CancelObjectSources.Remove(source);
                    return;
                }
            }

        }



        #endregion ------------------/Cancellation ====
    }
}