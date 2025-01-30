using System;
using UnityEngine;

public class RopeCreator : MonoBehaviour
{
    [SerializeField, Range(2, 50)] private int segmentCount = 12;
    public Vector2 start;
    public Vector2 end;
    public HingeJoint2D hingePrefab;
    
    [HideInInspector] public Transform[] segments;

    public bool hasPower = false;
    public GameObject powerPoleIn;
    public GameObject powerPoleOut;
    public bool ropeCreated = false;
    
    Vector2 GetSegmentPosition(int segmentIndex)
    {
        if(segmentIndex==segments.Length-1)
            return end;
        return Vector2.Lerp(start, end, (float)segmentIndex / segmentCount);
    }

    public void GenerateRope()
    {
        if(start == null || end == null)
            return;
        segments = new Transform[segmentCount+5];
        for (int i = 0; i < segments.Length; i++)
        {
            var currJoint = Instantiate(hingePrefab, GetSegmentPosition(i), Quaternion.identity, this.transform);
            segments[i] = currJoint.transform;
            if (i > 0)
            {
                currJoint.connectedBody = segments[i-1].GetComponent<Rigidbody2D>();
            }

            if (i == segments.Length - 1)
            {
                var lastRb = currJoint.GetComponent<Rigidbody2D>();
                lastRb.constraints = RigidbodyConstraints2D.FreezeAll;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(start == null || end == null)
            return;
        for (int i = 0; i < segments.Length; i++)
        {
            Vector2 posAtIndex= GetSegmentPosition(i);
            Gizmos.DrawSphere(posAtIndex, 0.1f);
        }
    }
}
