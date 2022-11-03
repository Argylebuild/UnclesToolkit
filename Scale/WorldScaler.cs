using System.Collections.Generic;
using Argyle.Utilities;
using UnityEngine;

namespace Argyle.UnclesToolkit.Scale
{
    /// <summary>
    /// Utility to scale the hologram to match the world. Intended to accomodate hardware imperfections.
    /// Should be updated to be able to be set by a "calibrate" function, and be stored in the user prefs file.
    /// </summary>
    public class WorldScaler : Manager<WorldScaler>
    {
        [SerializeField] private float scaleFactor = 1.006944f;

        /// <summary>
        /// factor by which the app will universally scale content.
        /// </summary>
        public static float ScaleFactor => Instance.scaleFactor;

        [SerializeField] [Tooltip("Drag and drop the objects to be scaled. ")]
        private List<Transform> scaledObjects = new List<Transform>();

        [SerializeField]
        [Tooltip("These objects must be reversed scaled for proper results. Should include parents of cameras. ")]
        private List<Transform> inverseScaledObjects = new List<Transform>();

        // Start is called before the first frame update
        void Start()
        {
            ApplyScale(); 
        }

        [ContextMenu("Grab Cameras")]
        void GrabCameras()
        {
            List<Transform> camParents = new List<Transform>();
            var cameras = FindObjectsOfType<Camera>();
            foreach (var cam in cameras)
            {
                var camParent = cam.transform.parent;
                if(camParent != null && !inverseScaledObjects.Contains(camParent))
                    inverseScaledObjects.Add(cam.transform.parent);
            }
        }

        [ContextMenu("Apply Scale")]
        public void ApplyScale()
        {
            //GrabCameras();
            foreach (var thing in scaledObjects)
            {
                thing.localScale = Vector3.one * scaleFactor;
            }

            foreach (var thing in inverseScaledObjects)
            {
                thing.localScale = Vector3.one / scaleFactor;
            }
        }

    }
}