using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class PowerPole : MonoBehaviour
{
    public bool hasPower = false;

    [SerializeField]
    public Vector2 powerInTransform;
    [SerializeField]
    public Vector2 powerOutTransform;
    
    public bool isStart = false;
    public bool isEnd = false;
    public GameObject powerLineIn;
    public GameObject powerLineOut;

    private Light2D _light2D;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _light2D = GetComponent<Light2D>();
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
        if (isStart)
        {
            hasPower = true;
            var collider = gameObject.GetComponent<BoxCollider2D>();
            powerOutTransform = transform.position + new Vector3(collider.offset.x, collider.offset.y, 0);
        }
        if (isEnd)
        {
            hasPower = false;
            var collider = gameObject.GetComponent<BoxCollider2D>();
            powerInTransform = transform.position + new Vector3(collider.offset.x, collider.offset.y, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(powerLineIn != null)
        {
            
        }
        _light2D.enabled = hasPower;
    }
}
