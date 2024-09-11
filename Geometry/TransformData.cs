using System;
using UnityEngine;

namespace Argyle.UnclesToolkit.Geometry
{
    [Serializable]
    public struct TransformData
    {
        [SerializeField]
        private Vector3 position;
        public Vector3 Position => position;
        [SerializeField]
        public Vector3 RotationEuler => rotationQuaternion.eulerAngles;
        [SerializeField]
        private Quaternion rotationQuaternion;
        public Quaternion RotationQuaternion => rotationQuaternion;
        public Vector3 Scale => scale;
        [SerializeField]
        private Vector3 scale;

        #region == Constructors ==

        /// <summary>
        /// Builds TransformData based on the local data of the transform. 
        /// </summary>
        /// <param name="tform"></param>
        public TransformData(Transform tform, bool local = true)
        {
            if (local)
            {
                position = tform.localPosition;
                rotationQuaternion = tform.localRotation;
                scale = tform.localScale;
            }
            else
            {
                position = tform.position;
                rotationQuaternion = tform.rotation;
                scale = tform.localScale;
            }
        }
        
        public TransformData(Matrix4x4 matrix, float scale = 1)
        {
            //trying the other version for troubleshooting. looks the same.
            position = matrix.GetTranslation();
            rotationQuaternion = matrix.GetRotation();
            this.scale = matrix.GetScale() * scale;

        }

        public TransformData(Vector3 position)
        {
            this.position = position;
            rotationQuaternion = Quaternion.identity;
            scale = Vector3.one;
        }

        public TransformData(Vector3 position, Quaternion rotation)
        {
            this.position = position;
            rotationQuaternion = rotation;
            scale = Vector3.one;
        }
        
        

        #endregion /Constructors ==
        
        
        
        /// <summary>
        /// Set this transform data to its default values.
        /// </summary>
        public void SetToDefaultValues()
        {
            this = Default();
        }
        /// <summary>
        /// Get the default values of transform to set another instance.
        /// </summary>
        /// <returns>TransformData struct with default values set.</returns>
        public static TransformData Default()
        {
            TransformData data;
            data.position = Vector3.zero;
            data.rotationQuaternion = Quaternion.identity;
            data.scale = Vector3.one;
            return data;
        }

        // public TransformData InvertYZ()
        // {
        //     position = new Vector3(Position.x, Position.z, Position.y);
        //     rotationEuler = new Vector3(rotationEuler.x, -rotationEuler.z, rotationEuler.y);
        //
        //     return this;
        // }

        public bool IsDefault()
        {
            return (Position == Vector3.zero &&
                    RotationEuler == Vector3.zero &&
                    Scale == Vector3.one);
        }

        #region == Apply Transformation == 

        /// <summary>
        /// Applies the current transformdata to the target as if target is nested in hierarchy.
        /// Like Transform.TransformPoint but isolated to just this transformation and over an entire transformData.
        /// (Like converting from local to global except only moving up one level. Does not necessarily account for nesting.)
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public TransformData ApplyTransformationTo(TransformData target)
        {
            return ToMatrix().ApplyTransformationTo(target);
        }

        /// <summary>
        /// Applies the current transformdata to the target as if target is nested in hierarchy.
        /// Like Transform.TransformPoint but isolated to just this transformation.
        /// (Like converting from local to global except only moving up one level. Does not necessarily account for nesting.)
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public Vector3 ApplyTransformationTo(Vector3 target)
        {
            return ToMatrix().ApplyTransformationTo(target);
        }

        public Bounds ApplyTransformationTo(Bounds target)
        {
            var min = ApplyTransformationTo(target.min);
            var max = ApplyTransformationTo(target.max);

            target.min = min;
            target.max = max;

            return target;
        }

        /// <summary>
        /// Removes the current transformdata from the target. Used to find relative transformation
        /// Like Transform.InverseTransformPoint but isolated to just this transformation and over an entire transformData.
        /// (Like converting from global to local except only moving down one level. Does not necessarily account for nesting.)
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public TransformData ApplyInverseTransformationTo(TransformData target)
        {
            var step1 = ToMatrix();
            var step2 = step1.ApplyTransformationTo(target);
            return ToMatrix().ApplyInverseTransformationTo(target);
        }

        /// <summary>
        /// Removes the current transformdata from the target. Used to find relative transformation
        /// Like Transform.InverseTransformPoint but isolated to just this transformation.
        /// (Like converting from global to local except only moving down one level. Does not necessarily account for nesting.)
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public Vector3 ApplyInverseTransformationTo(Vector3 target)
        {
            return ToMatrix().ApplyInverseTransformationTo(target);
        }
        
        
        #endregion /Apply Transformation ==
        
        
        /// <summary>
        /// UNTESTED
        /// Generates an inverse transformdata.
        /// If applied as transformation, will reverse original.
        /// Also used to replace things like inversetransformpoint.
        /// </summary>
        /// <returns></returns>
        public TransformData Invert()
        {
            return ToMatrix().inverse.ToTransformData();
        }

        public Matrix4x4 ToMatrix()
        {
            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetTRS(Position, RotationQuaternion, Scale);

            return matrix;
        }

        public Transform ToTransform()
        {
            Transform tform = new GameObject().transform;
            tform.localPosition = Position;
            tform.localRotation = RotationQuaternion;
            tform.localScale = Scale;

            return tform;
        }

        public float[] ToFlatArray()
        {
            Matrix4x4 matrix = ToMatrix();
            float[] array = new float[16];
            for (int i = 0; i < 16; i++)
            {
                array[i] = matrix[i];
            }
            
            return array;
        }
    }
}