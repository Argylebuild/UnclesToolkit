using System;
using UnityEngine;

/* Much of this Copied from https://forum.unity.com/threads/how-to-assign-matrix4x4-to-transform.121966/
 * By numberkruncher oct 31, 2014. Happy halloweeeeeen!
 */

namespace Argyle.Utilities.Geometry
{
    public static class MatrixUtility 
    {

	    #region ==== By NumberKruncher ====

	    /// <summary>
	    /// Extract position, rotation and scale from TRS matrix.
	    /// </summary>
	    /// <param name="matrix">Transform matrix. This parameter is passed by reference
	    /// to improve performance; no changes will be made to it.</param>
	    /// <param name="localPosition">Output position.</param>
	    /// <param name="localRotation">Output rotation.</param>
	    /// <param name="localScale">Output scale.</param>
	    public static void DecomposeMatrix(ref Matrix4x4 matrix, out Vector3 localPosition,
		    out Quaternion localRotation, out Vector3 localScale)
	    {
		    localPosition = GetTranslation(ref matrix);
		    localRotation = GetRotation(ref matrix);
		    localScale = GetScale(ref matrix);
	    }

	    /// <summary>
	    /// Set transform component from TRS matrix.
	    /// </summary>
	    /// <param name="transform">Transform component.</param>
	    /// <param name="matrix">Transform matrix. This parameter is passed by reference
	    /// to improve performance; no changes will be made to it.</param>
	    public static void SetTransformFromMatrix(Transform transform, ref Matrix4x4 matrix)
	    {
		    transform.localPosition = GetTranslation(ref matrix);
		    transform.localRotation = GetRotation(ref matrix);
		    transform.localScale = GetScale(ref matrix);
	    }


	    // EXTRAS!

	    /// <summary>
	    /// Identity quaternion.
	    /// </summary>
	    /// <remarks>
	    /// <para>It is faster to access this variation than <c>Quaternion.identity</c>.</para>
	    /// </remarks>
	    public static readonly Quaternion IdentityQuaternion = Quaternion.identity;

	    /// <summary>
	    /// Identity matrix.
	    /// </summary>
	    /// <remarks>
	    /// <para>It is faster to access this variation than <c>Matrix4x4.identity</c>.</para>
	    /// </remarks>
	    public static readonly Matrix4x4 IdentityMatrix = Matrix4x4.identity;

	    /// <summary>
	    /// Get translation matrix.
	    /// </summary>
	    /// <param name="offset">Translation offset.</param>
	    /// <returns>
	    /// The translation transform matrix.
	    /// </returns>
	    public static Matrix4x4 TranslationMatrix(Vector3 offset)
	    {
		    Matrix4x4 matrix = IdentityMatrix;
		    matrix.m03 = offset.x;
		    matrix.m13 = offset.y;
		    matrix.m23 = offset.z;
		    return matrix;
	    }

	    #endregion /By NumberKruncher ====

	    #region ==== In-house ====

	    /// <summary>
	    /// Convert 1D array representing column-major 4x4 matrix into a unity Matrix4x4 struct.
	    /// </summary>
	    /// <param name="array">2d array</param>
	    /// <returns></returns>
	    public static Matrix4x4 ArrayToMatrix(float[] array)
	    {
		    if (array.Length == 16)
		    {
			    return new Matrix4x4(
				    new Vector4(array[0], array[1], array[2], array[3]),
				    new Vector4(array[4], array[5], array[6], array[7]),
				    new Vector4(array[8], array[9], array[10], array[11]),
				    new Vector4(array[12], array[13], array[14], array[15])
			    );
		    }
		    throw new Exception("Incorrect array length. Array length 16 required for conversion to Matrix4x4.");
		    return Matrix4x4.zero; //Too forgiving.
	    }

	    
	            /// <summary>
        /// Applies transformation matrix to convert from OpenGL's (glTF) right handed coordinate system
        /// into Unity's left-handed system. 
        /// </summary>
        /// <param name="original">matrix to be coverted</param>
        /// <returns>Converted matrix</returns>
        public static Matrix4x4 OpenGL2Unity(this Matrix4x4 original)
        {
            // Matrix4x4 reverser = Matrix4x4.identity;
            // reverser.m22 = -1;
            // return reverser * original;
            return new Matrix4x4(
                new Vector4(original.m00, original.m10, -original.m20, original.m30),
                new Vector4(original.m01, original.m11, -original.m21, original.m31),
                new Vector4(-original.m02, -original.m12, original.m22, -original.m32),
                new Vector4(original.m03, original.m13, -original.m23, original.m33));
        }

        /// <summary>
        /// Converts a 4x4 column-major transform array into the 3 Vector3s 
        /// Position (translation), rotation (Euler Angles), and scale;
        /// </summary>
        /// <param name="matrix">4x4Matrix representing M = T*R*S. </param>
        /// <param name="scale">Optional scale factor to apply to conversion</param>
        /// <returns>Transform data for postion, rotation, and scale in vector3s. </returns>
        public static TransformData ToTransformData(this Matrix4x4 matrix, float scale = 1)
        {
            TransformData transformData = new TransformData(matrix);

            return transformData;
        }


        #region == Get Component ==

        /// <summary>
        /// Extract translation from transform matrix.
        /// </summary>
        /// <param name="matrix">Transform matrix. This parameter is passed by reference
        /// to improve performance; no changes will be made to it.</param>
        /// <returns>
        /// Translation offset.
        /// </returns>
        public static Vector3 GetTranslation(ref this Matrix4x4 matrix)
        {
            Vector3 translate;
            translate.x = matrix.m03;
            translate.y = matrix.m13;
            translate.z = matrix.m23;
            return translate;
        }

        /// <summary>
        /// Extract rotation quaternion from transform matrix.
        /// </summary>
        /// <param name="matrix">Transform matrix. This parameter is passed by reference
        /// to improve performance; no changes will be made to it.</param>
        /// <returns>
        /// Quaternion representation of rotation transform.
        /// </returns>
        public static Quaternion GetRotation(ref this Matrix4x4 matrix)
        {
            Vector3 forward;
            forward.x = matrix.m02;
            forward.y = matrix.m12;
            forward.z = matrix.m22;

            Vector3 upwards;
            upwards.x = matrix.m01;
            upwards.y = matrix.m11;
            upwards.z = matrix.m21;

            return Quaternion.LookRotation(forward, upwards);
        }

        /// <summary>
        /// Extract scale from transform matrix.
        /// </summary>
        /// <param name="matrix">Transform matrix. This parameter is passed by reference
        /// to improve performance; no changes will be made to it.</param>
        /// <returns>
        /// Scale vector.
        /// </returns>
        public static Vector3 GetScale(ref this Matrix4x4 matrix)
        {
            Vector3 scale;
            scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
            scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
            scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
            return scale;
        }

        #endregion /Get Component ==



        #region == Apply Transformation ==

        /// <summary>
        /// Applies the current matrix transformation to the target. Used to find relative transformation
        /// Like Transform.TransformPoint but isolated to just this transformation and over an entire matrix.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Matrix4x4 ApplyTransformationTo(this Matrix4x4 matrix, Matrix4x4 target)
        {
            return matrix * target;
        }

        /// <summary>
        /// UNTESTED
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Matrix4x4 ApplyInverseTransformationTo(this Matrix4x4 matrix, Matrix4x4 target)
        {
            return matrix.inverse.ApplyTransformationTo(target);
        }

        /// <summary>
        /// Applies the current matrix transformation to the target. Used to find relative transformation
        /// Like Transform.TransformPoint but isolated to just this transformation and over an entire transformData.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static TransformData ApplyTransformationTo(this Matrix4x4 matrix, TransformData target)
        {
            return matrix.ApplyTransformationTo(target.ToMatrix()).ToTransformData();
        }
        
        /// <summary>
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static TransformData ApplyInverseTransformationTo(this Matrix4x4 matrix, TransformData target)
        {
            return matrix.inverse.ApplyTransformationTo(target);
        }


        /// <summary>
        /// Applies the current matrix transformation to the target. Used to find relative transformation
        /// Like Transform.TransformPoint but isolated to just this transformation.
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Vector3 ApplyTransformationTo(this Matrix4x4 matrix, Vector3 position)
        {
            return matrix.ApplyTransformationTo(new TransformData(position)).Position;
        }
        
        /// <summary>
        /// UNTESTED
        /// </summary>
        /// <param name="matrix"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Vector3 ApplyInverseTransformationTo(this Matrix4x4 matrix, Vector3 target)
        {
            return matrix.inverse.ApplyTransformationTo(target);
        }


        #endregion /Apply Transformation ==

	    

	    #endregion /In-house ====
    }
}