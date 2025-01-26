using UnityEngine;

public class RopeLine : MonoBehaviour
{
    
    RoperCreator ropeCreator;
    LineRenderer lineRenderer;
    
    [SerializeField] Material ropeMaterial;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ropeCreator = GetComponent<RoperCreator>();
        lineRenderer = GetComponent<LineRenderer>();
        
        lineRenderer.enabled = true;
        lineRenderer.positionCount = ropeCreator.segments.Length;
    }

    // Update is called once per frame
    void Update()
    {
        if(ropeCreator.end == null)
        {
            return;
        }
        if(lineRenderer.positionCount != ropeCreator.segments.Length)
        {
            lineRenderer.positionCount = ropeCreator.segments.Length;
        }
        for(int i=0; i<ropeCreator.segments.Length; i++)
        {
            lineRenderer.SetPosition(i, ropeCreator.segments[i].position);
        }

        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount-1, ropeCreator.end.position);
    }
}
