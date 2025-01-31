using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;

    // Update is called once per frame
    void Update()
    {
            
    }

    public void LoadLevel(int? levelIndex=null)
    {
        StartCoroutine(CRLoadLevel(levelIndex ?? SceneManager.GetActiveScene().buildIndex + 1));
    }
    
    IEnumerator CRLoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(levelIndex);
    }
}
