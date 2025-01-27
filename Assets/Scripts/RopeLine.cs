using UnityEngine;
using UnityEngine.Serialization;

public class RopeLine : MonoBehaviour
{
    private RopeCreator _ropeCreator;
    private LineRenderer _lineRenderer;
    
    void Start()
    {
        _ropeCreator = GetComponent<RopeCreator>();
        _lineRenderer = GetComponent<LineRenderer>();
        
        _lineRenderer.enabled = true;
        _lineRenderer.positionCount = _ropeCreator.segments.Length;
    }
    
    void Update()
    {
        if(_ropeCreator.end == null)
        {
            return;
        }
        if(_lineRenderer.positionCount != _ropeCreator.segments.Length)
        {
            _lineRenderer.positionCount = _ropeCreator.segments.Length;
        }
        for(int i=0; i<_ropeCreator.segments.Length; i++)
        {
            _lineRenderer.SetPosition(i, _ropeCreator.segments[i].position);
        }

        _lineRenderer.positionCount++;
        _lineRenderer.SetPosition(_lineRenderer.positionCount-1, _ropeCreator.end);
    }
}
