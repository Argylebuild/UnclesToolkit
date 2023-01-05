using System;
using System.Collections.Generic;
using System.Linq;
using Argyle.UnclesToolkit.Geometry;
using EasyButtons;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace Argyle.UnclesToolkit.Testing
{
    public class LatticeTest : ArgyleComponent
    {
        public Transform ThingContainer;
        private List<Transform> _things;
        public GameObject _visBoxPrefab;
        public float _cellSize = 1;
        public Transform pov;

        public Lattice<GameObject> _lattice;
        private List<GameObject> _visBoxes = new List<GameObject>();
        private GameObject boundsVis;
        private Dictionary<Cell<GameObject>, CellTest> CellTestDic = new Dictionary<Cell<GameObject>, CellTest>();



        #region ==== Monobehavior ====------------------

        
        private void Update()
        {
            HighlightClosest();
        }

        #endregion -----------------/Monobehavior ====
        
        
        
        [Button]
        public void ShowLattice()
        {
            Clear();
            _lattice = new Lattice<GameObject>(_cellSize);

            _things = ThingContainer.GetComponentsInChildren<Transform>().ToList();

            
            foreach (var thing in _things)
            {
                if(thing == ThingContainer)
                    continue;
                if (!thing.GetComponent<LatticeThingTest>())
                    thing.AddComponent<LatticeThingTest>();
                
                Bounds b = thing.GetComponent<MeshRenderer>().bounds;
                _lattice.AddByMinMax(TForm.InverseTransformPoint(b.min), TForm.InverseTransformPoint(b.max), thing.gameObject);
            }
            CellTestDic = new Dictionary<Cell<GameObject>, CellTest>();
            
            
            //visualize boxes
            foreach (var cell in _lattice.Cells)
            {
                GameObject cellVis = Instantiate(_visBoxPrefab, TForm);
                _visBoxes.Add(cellVis);
                cellVis.transform.localPosition = _lattice.CellPositionToPoint(cell.Key);
                cellVis.transform.localRotation = quaternion.identity;
                cellVis.transform.localScale = Vector3.one * _cellSize;
                cellVis.name = cell.Key.ToString();
                var cellTest = cellVis.AddComponent<CellTest>();
                CellTestDic.Add(cell.Value, cellTest);
                cellTest.Cell = cell.Value;
                foreach (var thing in cell.Value.Things)
                    cellTest._things.Add(thing);
            }
            

            
            //Visualize bounds
            boundsVis = Instantiate(_visBoxPrefab, TForm);
            Vector3 minCorner = _lattice.CellPositionToPoint(_lattice.Min) - Vector3.one * _cellSize / 2;
            Vector3 maxCorner = _lattice.CellPositionToPoint(_lattice.Max) + Vector3.one * _cellSize / 2;

            boundsVis.transform.localPosition = (minCorner + maxCorner) / 2;
            boundsVis.transform.localScale = minCorner - maxCorner;
            boundsVis.transform.localRotation = Quaternion.identity;
            
            Debug.Log($"Finished showing. Good spot to Debug. But here's a summary: \n" +
                      $"Cells: {_lattice.Cells.Count} \n" +
                      $"");
        }

        private void Clear()
        {
            foreach (var visBox in _visBoxes)
                Destroy(visBox);

            if(boundsVis)
                Destroy(boundsVis);
            boundsVis = null;
            
            
            _visBoxes = new List<GameObject>();
        }

        public void HighlightClosest()
        {
            if(_lattice == null)
                return;
            //gather
            Cell<GameObject> closest = _lattice.ClosestCell(TForm.InverseTransformPoint(pov.position));
            if(closest == null)
                return;
            var surrounding1 = _lattice.Surrounding(closest.CellPosition, 1);
            var surrounding2 = _lattice.Surrounding(closest.CellPosition, 2);
            
            //scale
            foreach (var cellTest in CellTestDic.Values)
            {
                cellTest.Scale(1);
                cellTest.GetComponent<MeshRenderer>().material.color = Color.black;
            }   
            foreach (var cell in surrounding2)
            {
                CellTestDic[cell].Scale(1.1f);
                CellTestDic[cell].GetComponent<MeshRenderer>().material.color = new Color(0,.1f,.3f);
            }            
            foreach (var cell in surrounding1)
            {
                CellTestDic[cell].Scale(1.2f);
                CellTestDic[cell].GetComponent<MeshRenderer>().material.color = new Color(0, .2f, .4f);
            }            
            CellTestDic[closest].Scale(1.4f);
            CellTestDic[closest].GetComponent<MeshRenderer>().material.color = new Color(0, .4f, 1);

        }

        public void HighlightNearby()
        {
             
        }
        
        
    }
}


