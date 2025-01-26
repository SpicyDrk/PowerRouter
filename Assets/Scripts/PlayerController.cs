using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerContoller : MonoBehaviour
{
    [SerializeField] private float _maxSpeed = 4.0f;

    [SerializeField] private float _jumpForce = 5.0f;
    [SerializeField] private float _friction = 0.9f;
    public ItemInHands itemInHands = ItemInHands.None; 
    private Vector3 _velocity;
    
    [SerializeField] private GameObject powerPole;
    [SerializeField] private GameObject powerLine;
    [SerializeField] private BoxCollider2D placementBox;
    
    private Rigidbody2D _rigidbody;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovement();
        UpdateCommands();
    }
    
    private void UpdateCommands()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            itemInHands = ItemInHands.PowerPole;
            powerPole.GetComponent<SpriteRenderer>().enabled = true;
            powerLine.GetComponent<SpriteRenderer>().enabled = false;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            itemInHands = ItemInHands.PowerLine;
            powerPole.GetComponent<SpriteRenderer>().enabled = false;
            powerLine.GetComponent<SpriteRenderer>().enabled = true;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            itemInHands = ItemInHands.None;
            powerPole.GetComponent<SpriteRenderer>().enabled = false;
            powerLine.GetComponent<SpriteRenderer>().enabled = false;
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            if (itemInHands == ItemInHands.PowerPole)
            {
                var powerPoleInstance = Instantiate(powerPole, transform.position, Quaternion.identity);
                
            }
            if (itemInHands == ItemInHands.PowerLine)
            {
                var powerLineInstance = Instantiate(powerLine, transform.position, Quaternion.identity); ;
            }
        }
    }
    
    private void UpdateMovement()
    {
        
        var movement = Input.GetAxis("Horizontal");
        Vector3 acceleration = new Vector3(movement*.5f, 0, 0);
        _velocity += acceleration;
        _velocity = Vector3.ClampMagnitude(_velocity, _maxSpeed);
        transform.position += _velocity * Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space) && Mathf.Abs(_rigidbody.linearVelocity.y) < 0.001f)
        {
            _rigidbody.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        }
        _velocity *= _friction;
        
        if (_velocity.x > 0.1f)
        {
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
        else if (_velocity.x < -0.1f)
        {
            transform.localScale = new Vector3(-0.5f, 0.5f, 0.5f);
        }
    }
    public enum ItemInHands
    {
        None,
        PowerPole,
        PowerLine
    }
}

