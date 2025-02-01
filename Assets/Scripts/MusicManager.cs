using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private bool playingMusic = true;
    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("music");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }
}
