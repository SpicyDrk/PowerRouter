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
    [SerializeField] private GameObject powerLinePrefab;
    [SerializeField] private GameObject powerStart;
    [SerializeField] private GameObject powerEnd;
    
    private Rigidbody2D _rigidbody;
    private LineRenderer _lineRenderer;
    private Vector2 _connectionStart;
    public bool connectionActive = false;
    private RopeLine[] _ropeLines;
    public GameObject gamePlay;
    private GamePlay _gamePlay;
    
    float timeSinceLastConnection = 0;
    
    
    void Start()
    {
        _gamePlay = gamePlay.GetComponent<GamePlay>();
        _gamePlay.powerLineInstances.Add(powerStart);
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
                    _gamePlay.powerLineInstances.Add(powerPoleInstance);
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
            
            foreach (var powerStart in _gamePlay.powerPoleInstances)
            {
                if (Vector2.Distance(powerStart.transform.position, transform.position) < 1.0f)
                {
                    _connectionStart = powerStart.transform.position;
                    _lineRenderer.enabled = true;
                    break;
                }
            }
        }

        if (connectionActive)
        {
            DrawTempConnection();
            if(playerHitBox.IsTouchingLayers(LayerMask.GetMask("PowerEnd")) && timeSinceLastConnection > 1.0f)
            {
                timeSinceLastConnection = 0;
                CreatePermanentConnection();
            }
            else
            {
                timeSinceLastConnection += Time.deltaTime;
            }
        }
    }

    private void CreatePermanentConnection()
    {
        connectionActive = false;
        _lineRenderer.enabled = false;
        //find the closest gameObject on layer PowerIn
        //foreach (var powerStart in _gamePlay.powerPoleInstances)
        //{
        //    if (Vector2.Distance(powerStart.transform.position, transform.position) < 1.0f)
        //    {
        //        _connectionStart = powerStart.transform.position;
        //        _lineRenderer.enabled = true;
        //        break;
        //    }
        //}
        
        var powerLineInstance = Instantiate(powerLinePrefab, _connectionStart, Quaternion.identity);
        powerLineInstance.GetComponent<RopeCreator>().start = _connectionStart;
        powerLineInstance.GetComponent<RopeCreator>().end = playerHitBox.ClosestPoint(transform.position);
        powerLineInstance.GetComponent<RopeCreator>().GenerateRope();
        powerLineInstance.GetComponent<LineRenderer>().enabled = true;
        _gamePlay.powerLineInstances.Add(powerLineInstance);
    }

    private void DrawTempConnection()
     {
        _lineRenderer.enabled = true;
        _lineRenderer.SetPosition(0, _connectionStart);
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

