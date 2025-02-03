using UnityEngine;
using Vector2 = System.Numerics.Vector2;

public class Gate : MonoBehaviour
{
    [SerializeField] private float openDistance = 4f;
    public bool powered = false;
    private bool isOpened = false;

    private SoundManager _soundManager;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _soundManager = SoundManager.instance;
        if (_soundManager == null)
        {
            Debug.LogError("No SoundManager found in scene");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (powered)
        {
            if (!isOpened)
            {
                _soundManager.PlaySound("RocksSliding");
                isOpened = true;
            }
            if(openDistance < 0)
            {
                if (transform.position.y > -openDistance)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y - 0.005f, transform.position.z);
                }
            }
            else
            {
                if (transform.position.y < openDistance)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + 0.005f, transform.position.z);
                }
            }
        }
    }
}
