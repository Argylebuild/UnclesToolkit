using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMatch : MonoBehaviour
{
    public Camera Source
    {
        get
        {
            if(_source == null)
                _source = Camera.main;

            return _source;
        }
    }
    [SerializeField] private Camera _source;
    
    private Camera _self;

    public bool continuous; 
    public Camera Self
    {
        get
        {
            if (_self == null)
                _self = GetComponent<Camera>();

            return _self;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(continuous)
            MatchSource();
    }

    public void MatchSource()
    {
        Self.aspect = Source.aspect;
        Self.depth = Source.depth;
        Self.orthographic = Source.orthographic;
        Self.rect = Source.rect;
        Self.focalLength = Source.focalLength;
        Self.gateFit = Source.gateFit;
        Self.fieldOfView = Source.fieldOfView;
    }
    
}
