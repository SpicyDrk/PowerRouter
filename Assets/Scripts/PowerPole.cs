using UnityEngine;

public class PowerPole : MonoBehaviour
{
    public bool hasPower = false;

    [SerializeField]
    public Vector3 powerInTransform;
    [SerializeField]
    public Vector3 powerOutTransform;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var children = gameObject.GetComponentsInChildren<Transform>();
        foreach (var child in children)
        {
            if (child.name == "PowerIn")
            {
                powerInTransform = child.position;
            }
            if (child.name == "PowerOut")
            {
                powerOutTransform = child.position;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
