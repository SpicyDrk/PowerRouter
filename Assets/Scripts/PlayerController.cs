using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerContoller : MonoBehaviour
{
    [SerializeField] private float _maxSpeed = 4.0f;

    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float friction = 0.9f;
    public ItemInHands itemInHands = ItemInHands.None; 
    private Vector3 _velocity;
    
    [SerializeField] private GameObject powerPoleSprite;
    [SerializeField] private GameObject powerLineSprite;
    [SerializeField] private BoxCollider2D placementBox;
    [SerializeField] private GameObject powerPolePrefab;
    [SerializeField] private BoxCollider2D playerHitBox; 

    public List<GameObject> powerPoleInstances = new();
    public List<GameObject> powerLineInstances = new();
    private Rigidbody2D _rigidbody;
    private LineRenderer _lineRenderer;
    private Vector2 _start;
    public bool connectionActive = false;
    private RopeLine[] _ropeLines;
    
    
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.enabled = false;
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
            powerPoleSprite.GetComponent<SpriteRenderer>().enabled = true;
            powerLineSprite.GetComponent<SpriteRenderer>().enabled = false;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            itemInHands = ItemInHands.PowerLine;
            powerPoleSprite.GetComponent<SpriteRenderer>().enabled = false;
            powerLineSprite.GetComponent<SpriteRenderer>().enabled = true;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            itemInHands = ItemInHands.None;
            powerPoleSprite.GetComponent<SpriteRenderer>().enabled = false;
            powerLineSprite.GetComponent<SpriteRenderer>().enabled = false;
            connectionActive = false;
            BreakConnection();
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            if (itemInHands == ItemInHands.PowerPole)
            {
                if (placementBox.IsTouchingLayers(LayerMask.GetMask("Ground")))
                {
                    var powerPoleInstance = Instantiate(powerPolePrefab, transform.position + new Vector3(0,0.4f,0), Quaternion.identity);
                    powerPoleInstance.GetComponent<SpriteRenderer>().enabled = true;
                    powerPoleInstance.transform.localScale = new Vector3(3f, 3f, 0f);
                    powerPoleInstances.Add(powerPoleInstance);
                }
                else
                {
                    Debug.Log("Cannot place power pole here");
                }
            }
        }
        if (itemInHands == ItemInHands.PowerLine && playerHitBox.IsTouchingLayers(LayerMask.GetMask("PowerStart")) && !connectionActive)
        {
            connectionActive = true;
            //TODO get the center of the connection...
            _start = playerHitBox.ClosestPoint(transform.position);
            _lineRenderer.enabled = true;
        }

        if (connectionActive)
        {
            DrawTempConnection();
            if(playerHitBox.IsTouchingLayers(LayerMask.GetMask("PowerEnd")))
            {
                CreatePermanentConnection();
            }
        }
    }

    private void CreatePermanentConnection()
    {
        connectionActive = false;
        _lineRenderer.enabled = false;
        var ropeCreator = new RopeCreator();
        ropeCreator.end = transform;
        ropeCreator.start = new Transform(new Vector3(_start.x, _start.y, 0), Quaternion.identity, new Vector3(1, 1, 1));
    }

    private void DrawTempConnection()
     {
        _lineRenderer.enabled = true;
        _lineRenderer.SetPosition(0, _start);
        _lineRenderer.SetPosition(1, transform.position);
    }

    private void BreakConnection()
    {
        connectionActive = false;
        _lineRenderer.enabled = false;
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
            _rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        _velocity *= friction;
        
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

