using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayUI : MonoBehaviour
{
    private PlayerContoller playerController;

    [SerializeField] private TMP_Text bottomText;
    [SerializeField] private TMP_Text cancelText;

    public static bool GamePaused = false;
    public GameObject pauseMenuUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerController = GameObject.Find("Player").GetComponent<PlayerContoller>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (playerController.itemInHands)
        {
            case PlayerContoller.ItemInHands.Delete:
                bottomText.fontSize = 24;
                bottomText.text = "(E) Delete";
                cancelText.text = "(C) Cancel";
                break;
            case PlayerContoller.ItemInHands.PowerLine:
                bottomText.fontSize = 24;
                bottomText.text = "Power lines flow from green to red connections";
                cancelText.text = "(C) Cancel";
                break;
            case PlayerContoller.ItemInHands.PowerPole:
                bottomText.fontSize = 24;
                bottomText.text = "(E) place power poles";
                cancelText.text = "(C) Cancel";
                break;
            default:
                bottomText.text = "";
                cancelText.text = "";
                break;
        }
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (GamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        Time.timeScale = 1;
        GamePaused = false;
        pauseMenuUI.SetActive(false);
    }
    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0;
        GamePaused = true;
    }
    
    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 0;
        GamePaused = false;
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 0;
        GamePaused = true;
    }
}
