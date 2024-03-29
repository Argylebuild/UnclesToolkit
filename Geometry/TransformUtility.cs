﻿using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Argyle.UnclesToolkit.Geometry
{
    public static class TransformUtility 
    {
        /// <summary>
        /// Reset all values to their default local values.
        /// </summary>
        /// <param name="tform"></param>
        public static void ResetLocal(this Transform tform)
        {
            tform.localPosition = Vector3.zero;
            tform.localRotation = Quaternion.identity;
            tform.localScale = Vector3.one;
        }

        /// <summary>
        /// Sets the transforms local parameters according to transformData object.
        /// </summary>
        /// <param name="tForm"></param>
        /// <param name="data"></param>
        public static void SetDataLocal(this Transform tForm, TransformData data)
        {
            tForm.localPosition = data.Position;
            tForm.localEulerAngles = data.RotationEuler;
            tForm.localScale = data.Scale;
        }

        /// <summary>
        /// TransformData built from the parameters of this transform.
        /// </summary>
        /// <param name="tForm"></param>
        /// <returns></returns>
        public static TransformData Data(this Transform tForm)
        {
            return new TransformData(tForm);
        }

        #region ==== Animate ====------------------

        /// <summary>
        /// Translates an object a given vector over a given time in local space
        /// </summary>
        /// <param name="tform"></param>
        /// <param name="translation"></param>
        /// <param name="duration"></param>
        /// <param name="isLocal">Is the position passed a localPosition?</param>
        public async static void AnimateTranslate(this Transform tform, Vector3 translation, float duration, bool isLocal = true)
        {
            Vector3 destination;
            
            if (isLocal)
                destination = tform.position + translation;
            else
                destination = tform.position + tform.InverseTransformDirection(translation);
            
            await tform.AnimateTranslateTo(destination, duration, isLocal);
        }

        /// <summary>
        /// Translates an object to a given position over a given time in local or world space
        /// </summary>
        /// <param name="tform"></param>
        /// <param name="duration"></param>
        /// <param name="isLocal">Is the position passed a localPosition?</param>
        public async static UniTask AnimateTranslateTo(this Transform tform, Vector3 destination, float duration, bool isLocal = true)
        {
            Vector3 startPosition;
            if (isLocal)
                startPosition = tform.localPosition;
            else
                startPosition = tform.position;
            
            float portionComplete = 0;
            
            
            while (portionComplete <= 1)
            {
                if(tform == null)
                    return;
                
                if(isLocal)
                    tform.localPosition = Vector3.Lerp(startPosition, destination, portionComplete);
                else
                    tform.position = Vector3.Lerp(startPosition, destination, portionComplete);
                portionComplete += Time.deltaTime / duration;
                await UniTask.NextFrame();
            }
            
            //finalize
            if(isLocal)
                tform.localPosition = destination;
            else
                tform.position = destination;

        }


        public async static UniTask AnimateScale(this Transform tform, Vector3 factor, float duration)
        {
            Vector3 destinationScale = new Vector3(
                tform.localScale.x * factor.x,
                tform.localScale.y * factor.y,
                tform.localScale.z * factor.z);
            await tform.AnimateScaleTo(destinationScale, duration);
        }

        public async static UniTask AnimateScaleTo(this Transform tform, Vector3 factor, float duration)
        {
            Vector3 startScale;
            startScale = tform.localScale;
            
            float portionComplete = 0;
            
            
            while (portionComplete <= 1)
            {
                tform.localScale = Vector3.Lerp(startScale, factor, portionComplete);
                
                portionComplete += Time.deltaTime / duration;
                await UniTask.NextFrame();
            }
            
            //finalize
            tform.localScale = factor;

        }
        
        
        public async static UniTask AnimateRotate(this Transform tform, Vector3 rotation, float duration, bool isLocal = true)
        {
            Quaternion startRotation = isLocal ? tform.localRotation : tform.rotation;
            Quaternion endRotation = Quaternion.Euler(rotation) * startRotation;

            await tform.AnimateRotateTo(endRotation, duration, isLocal);
        }

        public async static UniTask AnimateRotateTo(this Transform tform, Quaternion endRotation, float duration, bool isLocal = true)
        {
            Quaternion startRotation = isLocal ? tform.localRotation : tform.rotation;
            float portionComplete = 0;

            while (portionComplete <= 1)
            {
                if(tform == null)
                    return;

                Quaternion currentRotation = Quaternion.Lerp(startRotation, endRotation, portionComplete);
                if (isLocal)
                    tform.localRotation = currentRotation;
                else
                    tform.rotation = currentRotation;

                portionComplete += Time.deltaTime / duration;
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            // Finalize
            if (isLocal)
                tform.localRotation = endRotation;
            else
                tform.rotation = endRotation;
        }

        #endregion -----------------/Animate ====


        /// <summary>
        /// Creates a snapshot of the source coordinate system for making transformations. 
        /// </summary>
        /// <returns></returns>
        public static Transform Copy(Transform source)
        {
            Transform copy = new GameObject("CopyTransform").transform;
            copy.parent = source.parent;
            copy.position = source.position;
            copy.rotation = source.rotation;
            copy.localScale = source.localScale;

            return copy;
        }
        
        /// <summary>
        /// Returns the simple pose of the referenced transform
        /// </summary>
        /// <param name="tForm"></param>
        /// <returns></returns>
        public static Pose GetPose(this Transform tForm)
        {
            return new Pose(tForm.position, tForm.rotation);
        }
        
        /// <summary>
        /// Sets the position and rotation of the transform by the given pose data.
        /// </summary>
        /// <param name="tForm"></param>
        /// <param name="pose"></param>
        public static void SetPose(this Transform tForm, Pose pose)
        {
            tForm.position = pose.position;
            tForm.rotation = pose.rotation;
        }
        
    }
}