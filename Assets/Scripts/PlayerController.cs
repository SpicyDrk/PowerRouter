using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class PlayerContoller : MonoBehaviour
{
    [SerializeField] private float _maxSpeed = 4.0f;

    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float friction = 0.9f;
    [SerializeField] private float maxConnectionDistance = 6.0f;
    public ItemInHands itemInHands = ItemInHands.None; 
    private Vector3 _velocity;
    
    [SerializeField] private GameObject powerPoleSprite;
    [SerializeField] private GameObject powerLineSprite;
    [SerializeField] private GameObject deleteSprite;
    [SerializeField] private BoxCollider2D placementBox;
    [SerializeField] private GameObject powerPolePrefab;
    [SerializeField] private BoxCollider2D playerHitBox; 
    [SerializeField] private GameObject powerLinePrefab;
    [SerializeField] private TMP_Text powerPolesText;
    [SerializeField] private TMP_Text restartText;
    [SerializeField] private LevelLoader _levelLoader;
    
    [SerializeField] private int maxPowerPoles = 4;
    private int _powerPoles;
    
    private Rigidbody2D _rigidbody;
    private LineRenderer _lineRenderer;
    private Vector2 _connectionStart;
    private bool _connectionActive = false;
    private RopeLine[] _ropeLines;
    public GameObject gamePlay;
    private GamePlay _gamePlay;
    
    public GameObject _powerPoleIn;

    private float _timeSinceLastConnection = 0;
    
    private SoundManager _soundManager;
    void Start()
    {
        _powerPoles = maxPowerPoles;
        powerPolesText.text = _powerPoles+"/" + maxPowerPoles;
        _gamePlay = gamePlay.GetComponent<GamePlay>();
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.enabled = false;
        _rigidbody = GetComponent<Rigidbody2D>();
        _soundManager = SoundManager.instance;
        _levelLoader = gamePlay.GetComponent<LevelLoader>();
        if (_soundManager == null)
        {
            Debug.LogError("No SoundManager found in scene");
        }
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
            deleteSprite.GetComponent<SpriteRenderer>().enabled = false;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            itemInHands = ItemInHands.PowerLine;
            powerPoleSprite.GetComponent<SpriteRenderer>().enabled = false;
            powerLineSprite.GetComponent<SpriteRenderer>().enabled = true;
            deleteSprite.GetComponent<SpriteRenderer>().enabled = false;
        }
        if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            itemInHands = ItemInHands.Delete;
            powerPoleSprite.GetComponent<SpriteRenderer>().enabled = false;
            powerLineSprite.GetComponent<SpriteRenderer>().enabled = false;
            deleteSprite.GetComponent<SpriteRenderer>().enabled = true;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            itemInHands = ItemInHands.None;
            powerPoleSprite.GetComponent<SpriteRenderer>().enabled = false;
            powerLineSprite.GetComponent<SpriteRenderer>().enabled = false;
            deleteSprite.GetComponent<SpriteRenderer>().enabled = false;
            _connectionActive = false;
            BreakConnection();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            _levelLoader.LoadLevel(SceneManager.GetActiveScene().buildIndex);
        }
        if(Input.GetKeyDown(KeyCode.E) && itemInHands == ItemInHands.Delete)
        {
            foreach (var powerPole in _gamePlay.powerPoleInstances)
            {
                var pp = powerPole.GetComponent<PowerPole>();
                if (pp.isStart || pp.isEnd)
                {
                    continue;
                }
                if(Vector2.Distance(powerPole.transform.position, transform.position) < 1.0f)
                {
                    _gamePlay.RemovePowerPoleInstance(powerPole);
                    _soundManager.PlaySound("Delete");
                    _powerPoles++;
                    powerPolesText.text = _powerPoles + "/" + maxPowerPoles;
                    break;
                }
            }
        }
        if(Input.GetKeyDown(KeyCode.E) && itemInHands == ItemInHands.PowerPole && _powerPoles > 0)
        {
            if (placementBox.IsTouchingLayers(LayerMask.GetMask("Ground")))
            {
                var powerPoleInstance = Instantiate(powerPolePrefab, transform.position + new Vector3(0,0.4f,0), Quaternion.identity);
                powerPoleInstance.GetComponent<SpriteRenderer>().enabled = true;
                powerPoleInstance.transform.localScale = new Vector3(3f, 3f, 0f);
                _gamePlay.powerPoleInstances.Add(powerPoleInstance);
                _soundManager.PlaySound("PlacePole");
                _powerPoles--;
                powerPolesText.text = _powerPoles + "/" + maxPowerPoles;
            }
        }
        if (itemInHands == ItemInHands.PowerLine && 
            playerHitBox.IsTouchingLayers(LayerMask.GetMask("PowerStart")) &&
            !_connectionActive)
        {
            
            foreach (var start in _gamePlay.powerPoleInstances)
            {
                PowerPole powerPoleIn = start.GetComponent<PowerPole>();

                if (Vector2.Distance(powerPoleIn.powerOutTransform, transform.position) < 1.0f && powerPoleIn.powerLineOut == null)
                {
                    _connectionActive = true;
                    _powerPoleIn = powerPoleIn.gameObject;
                    
                    _connectionStart = powerPoleIn.powerOutTransform;
                    _lineRenderer.enabled = true;
                    _soundManager.PlaySound("Pop");
                    break;
                }
            }
        }

        if (_connectionActive)
        {
            DrawTempConnection();
            if(playerHitBox.IsTouchingLayers(LayerMask.GetMask("PowerEnd")) && _timeSinceLastConnection > 1.0f && _powerPoleIn != null)
            {
                _timeSinceLastConnection = 0;
                CreatePermanentConnection(_powerPoleIn);
            }
            else
            {
                _timeSinceLastConnection += Time.deltaTime;
            }
        }
        if(itemInHands == ItemInHands.Delete)
        {
            foreach (var powerPole in _gamePlay.powerPoleInstances)
            {
                if(Vector2.Distance(powerPole.transform.position, transform.position) < 1.0f)
                {
                    var powerPoleComponent = powerPole.GetComponent<PowerPole>();
                    powerPoleComponent.isSelected = true;
                    powerPoleComponent.timeSinceSelected = 0;
                } else
                {
                    powerPole.GetComponent<PowerPole>().isSelected = false;
                }
            }
        }
    }

    private void CreatePermanentConnection(GameObject powerPoleIn)
    {
        foreach (var powerStart in _gamePlay.powerPoleInstances)
        {
            PowerPole powerPole = powerStart.GetComponent<PowerPole>();
            if (Vector2.Distance(powerPole.powerInTransform, transform.position) < 1.0f && powerPole.powerLineIn == null)
            {
                var powerLineInstance = Instantiate(powerLinePrefab, _connectionStart, Quaternion.identity);
                var ropeCreator = powerLineInstance.GetComponent<RopeCreator>();
                ropeCreator.start = _connectionStart;
                ropeCreator.end = powerPole.powerInTransform;
                ropeCreator.GenerateRope();
                ropeCreator.powerPoleOut = powerPoleIn;
                ropeCreator.powerPoleIn = powerPole.gameObject;
                powerPole.powerLineIn = powerLineInstance;
                powerPoleIn.gameObject.GetComponent<PowerPole>().powerLineOut = powerLineInstance;
                _gamePlay.CalculatePower();
                
                powerLineInstance.GetComponent<LineRenderer>().enabled = true;
                _gamePlay.powerLineInstances.Add(powerLineInstance);
                _connectionActive = false;
                _lineRenderer.enabled = false;
                _gamePlay.CalculatePower();
                _soundManager.PlaySound("Pop");
                break;
            }
        }
        if(_connectionActive)
        {
            //BreakConnection();
        }
    }

    private void DrawTempConnection()
     {
         var distance = _connectionStart - (Vector2)transform.position;
        _lineRenderer.enabled = true;
        _lineRenderer.SetPosition(0, _connectionStart);
        _lineRenderer.SetPosition(1, transform.position);
        if(distance.magnitude > maxConnectionDistance * 0.75f)
        {
            _lineRenderer.startColor = Color.red;
            _lineRenderer.endColor = Color.red;
        }
        else if(distance.magnitude > maxConnectionDistance * 0.5f)
        {
            _lineRenderer.startColor = new Color(243/255f,119/255f,19/255f);
            _lineRenderer.endColor = new Color(243/255f,119/255f,19/255f);
        }
        else
        {
            _lineRenderer.startColor = new Color(235/255f,213/255f,19/255f);
            _lineRenderer.endColor = new Color(235/255f,213/255f,19/255f, 255);
        }
        if(distance.magnitude > maxConnectionDistance)
        {
            BreakConnection();
        }
    }

    private void BreakConnection()
    {
        _connectionActive = false;
        _lineRenderer.enabled = false;
        _soundManager.PlaySound("RopeBreak");
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Pit"))
        {
            _soundManager.PlaySound("Death");
            ShowRestartText();
        }
    }
    
    private void ShowRestartText()
    {
        restartText.GetComponent<TextMeshProUGUI>().enabled = true;
    }
    public enum ItemInHands
    {
        None,
        PowerPole,
        PowerLine,
        Delete
    }
}

