using UnityEngine;
using Vector2 = System.Numerics.Vector2;

public class Gate : MonoBehaviour
{
    [SerializeField] private float openDistance = 4f;
    public bool powered = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (powered)
        {
            //move this object up by 0.1f every frame until openDistance is reached
            if (transform.position.y < openDistance)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + 0.005f, transform.position.z);
            }
        }
    }
}
