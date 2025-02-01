using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private bool followPlayer;

    private Transform playerTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerTransform = GameObject.Find("Player").transform;
        //lerp to keep the player in frame
        if (followPlayer)
        {
            if (playerTransform != null)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(playerTransform.position.x, playerTransform.position.y, transform.position.z), 0.005f);
            }
        }
    }
}
