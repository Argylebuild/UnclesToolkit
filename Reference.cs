using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Argyle.Utilities
{
    public static class Reference
    {
        private static float playerHeight = 1.5f;

        public static float PlayerHeight
        {
            get => playerHeight;
            set => playerHeight = value;
        }

        

        #region ======== Camera ============
        
        private static bool isCameraReferenceSet = false;
        
        private static Camera mainCamera;

        public static Camera MainCamera
        {
            get
            {
                if(!isCameraReferenceSet)
                    SetCameraReference();

                return mainCamera;
            }
        }

        private static Transform mainCameraTransform;

        public static Transform MainCameraTransform
        {
            get
            {
                if (!isCameraReferenceSet)
                    SetCameraReference();
                
                return mainCameraTransform;
            }
        }

        private static void SetCameraReference()
        {
            mainCamera = Camera.main;
            mainCameraTransform = mainCamera.transform;

            isCameraReferenceSet = true;
        }

        

        #endregion


    }
}