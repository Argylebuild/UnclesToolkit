using System.Collections.Generic;
using Argyle.UnclesToolkit.Geometry;
using EasyButtons;
using UnityEngine;

namespace Argyle.UnclesToolkit.Testing
{
    public class TransformDataTests : MonoBehaviour
    {
        public Dictionary<string, bool> Results = new Dictionary<string, bool>();

        [Button]
        public void RunAllTests()
        {
            //constructors
            Results.Add("Constructor Transform", TestCtorTform()); 
            Results.Add("Constructor Matrix", TestCtorMatrix());
            Results.Add("Constructor vector3", TestCtorPosition());
            Results.Add("ApplyTransformationTo vector3", TestTransvec3());
        
            //tosomethings
            Results.Add("ToMatrix", TestToMatrix());

            bool allPassed = true;
            foreach (var result in Results)
            {
                if (result.Value == false)
                {
                    Debug.LogWarning($"{0} failed!");
                    allPassed = false;
                }
            }

            if (allPassed)
                Debug.Log("All the tests passed!");
        }

        #region Constructors
    
        bool TestCtorTform()
        {
            var start = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
            start.localPosition = Vector3.one;
            var data = new TransformData(start);
        
            return data.Position == Vector3.one;
        }

        bool TestCtorMatrix()
        {
            Matrix4x4 mat = Matrix4x4.identity;
            TransformData tdat = new TransformData(mat);

            return tdat.Scale == Vector3.one;
        }

        bool TestCtorPosition()
        {
            TransformData tdat = new TransformData(Vector3.one);

            return tdat.Position == Vector3.one;
        }
        #endregion

        #region ToSomething

        bool TestToMatrix()
        {
            var tdat = new TransformData(Vector3.one);
            var mat = tdat.ToMatrix();

            return mat.GetTranslation() == Vector3.one;
        }

        #endregion

        #region Transformation

        private bool TestTransvec3()
        {
            TransformData trformer = new TransformData(Vector3.one);
            Vector3 start = Vector3.one;
            bool result = trformer.ApplyTransformationTo(start) == Vector3.one * 2;

            return result;
        }




        #endregion

    }
}
