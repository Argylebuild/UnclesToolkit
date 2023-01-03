using System.Collections;
using System.Collections.Generic;
using Argyle.UnclesToolkit;
using Argyle.UnclesToolkit.Geometry;
using UnityEngine;

public class SimpleTether : ArgyleComponent
{
    public Transform _headObject;
    public Transform _tailObject;

    [Header("Line Options")]
    public float _lineThickness = .001f;
    public float _lineDepth = .001f;
    public float _linePadding = 0;
    public Material _lineMaterial;
    
    private Transform _lineBox;
    private MeshRenderer _lineRend;
    
    // Start is called before the first frame update
    void Start()
    {
        _lineBox = GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
        _lineBox.parent = TForm;
        _lineRend = _lineBox.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 _point1Position = _headObject.position;
        Vector3 _point2Position = _tailObject.position;
        
        //scale
        _lineBox.localScale = new Vector3(
            _lineThickness,
            _lineDepth,
            Mathf.Clamp(Vector3.Distance(_point1Position, _point2Position) - _linePadding, .01f, 100f)
        );

        _lineBox.position = _point1Position.HalfWayTo(_point2Position);
        _lineBox.rotation = Quaternion.LookRotation(_point2Position - _point1Position, Vector3.up);
        
        //paint box
        if (_lineMaterial != null )
            if(_lineRend.sharedMaterial != _lineMaterial)
                _lineRend.sharedMaterial = _lineMaterial;
        
    }
}
