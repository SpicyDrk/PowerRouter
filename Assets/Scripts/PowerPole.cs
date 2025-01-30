using UnityEngine;
using UnityEngine.Rendering.Universal;

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

    [SerializeField] private Sprite powerOffSprite;
    [SerializeField] private Sprite powerOnSprite;
    [SerializeField] private GameObject seletedGameObject;
    
    public bool isSelected  = false; 

    private SpriteRenderer _spriteRenderer;
    private Light2D _light2D;
    
    public float timeSinceSelected = 0;
    
    private SoundManager _soundManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
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
            powerInTransform = new Vector2(-100, -100);
            var collider = gameObject.GetComponent<BoxCollider2D>();
            powerOutTransform = transform.position + new Vector3(collider.offset.x, collider.offset.y, 0);
        }
        if (isEnd)
        {
            hasPower = false;
            powerOutTransform = new Vector2(-100, -100);
            var collider = gameObject.GetComponent<BoxCollider2D>();
            powerInTransform = transform.position + new Vector3(collider.offset.x, collider.offset.y, 0);
        }
        _soundManager = SoundManager.instance;
        if (_soundManager == null)
        {
            Debug.LogError("No SoundManager found in scene");
        }
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceSelected += Time.deltaTime;

        if (!isStart && !isEnd )
        {
            if(timeSinceSelected > 1.0f)
            {
                isSelected = false;
                seletedGameObject.SetActive(false);
            }
            seletedGameObject.SetActive(isSelected);
        }

        _light2D.enabled = hasPower;
        if(!isStart && !isEnd)
        {
            _spriteRenderer.sprite = hasPower ? powerOnSprite : powerOffSprite;
        }
    }
}
