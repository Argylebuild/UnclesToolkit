using UnityEngine;

namespace Argyle.UnclesToolkit.Testing
{
	public class MatrixTest : ArgyleComponent
	{
		public Transform _thing; 
		
		[SerializeField] Matrix4x4 _matrix;


		#region ==== Readout ====------------------

		[Header("Local to World Point")] 
		public Vector3 LocalToWorldPointByTform;
		public Vector3 LocalToWorldPointByMatrix;
		public Vector3 GlobalPoint;

		[Header("World to Local Point")]
		public Vector3 WorldToLocalPointByTform;
		public Vector3 WorldToLocalPointByMatrix;
		public Vector3 LocalPoint;
		
		[Header("Local to World Direction")]
		public Vector3 LocalToWorldDirectionByTform;
		public Vector3 LocalToWorldDirectionByMatrix;
		public Vector3 GlobalDirection;
		
		[Header("World to Local Direction")]
		public Vector3 WorldToLocalDirectionByTform;
		public Vector3 WorldToLocalDirectionByMatrix;
		public Vector3 LocalDirection;
		
		#endregion -----------------/Readout ====
		
		
		
		
		public void Update()
		{
			_matrix.SetTRS(TForm.position, TForm.rotation, TForm.localScale);
    
			// Local to World Point - working
			LocalToWorldPointByTform = TForm.TransformPoint(_thing.localPosition);
			LocalToWorldPointByMatrix = _matrix.TransformPoint(_thing.localPosition);
			GlobalPoint = _thing.position;
    
			// World to Local Point - corrected
			WorldToLocalPointByTform = TForm.InverseTransformPoint(_thing.position);
			WorldToLocalPointByMatrix = _matrix.TransformPointInverse(_thing.position); // Corrected method usage
			LocalPoint = _thing.localPosition;
    
			// Local to World Direction - corrected
			LocalToWorldDirectionByTform = TForm.TransformDirection(_thing.localRotation * Vector3.forward);
			LocalToWorldDirectionByMatrix = _matrix.TransformDirection(_thing.localRotation * Vector3.forward);
			GlobalDirection = _thing.forward;
    
			// World to Local Direction - corrected
			WorldToLocalDirectionByTform = TForm.InverseTransformDirection(_thing.forward);
			WorldToLocalDirectionByMatrix = _matrix.TransformDirectionInverse(_thing.forward);
			LocalDirection = _thing.localRotation * Vector3.forward;
		}

		
		
	}
	
	public static class MatrixExtensions
	{
		public static Vector3 TransformPoint(this Matrix4x4 matrix, Vector3 point)
		{
			return matrix.MultiplyPoint3x4(point);
		}

		public static Vector3 TransformPointInverse(this Matrix4x4 matrix, Vector3 point)
		{
			var inverseMatrix = matrix.inverse;
			return inverseMatrix.MultiplyPoint3x4(point);
		}

		public static Vector3 TransformDirection(this Matrix4x4 matrix, Vector3 direction)
		{
			return matrix.MultiplyVector(direction).normalized;
		}

		public static Vector3 TransformDirectionInverse(this Matrix4x4 matrix, Vector3 direction)
		{
			var inverseMatrix = matrix.inverse;
			return inverseMatrix.MultiplyVector(direction).normalized;
		}
	}
}