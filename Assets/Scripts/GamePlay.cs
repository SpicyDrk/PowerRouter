using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [SerializeField] private Vector3 _electrictyStart;
    [SerializeField] private Vector3 _electrictyEnd;
    [SerializeField] private GameObject _linkPrefab;
    private GameObject[] links;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateLinks();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    //Create links between two points
    private void CreateLinks()
    {
        Vector3 direction = _electrictyEnd - _electrictyStart;
        float distance = direction.magnitude;
        int linksCount = (int)(distance / 0.1f);
        links = new GameObject[linksCount];
        for (int i = 0; i < linksCount; i++)
        {
            Vector3 position = _electrictyStart + direction.normalized * (distance / linksCount) * i;
            links[i] = Instantiate(_linkPrefab, position, Quaternion.identity);
        }
    }
}


