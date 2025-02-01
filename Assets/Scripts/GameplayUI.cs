using TMPro;
using UnityEngine;

public class GameplayUI : MonoBehaviour
{
    private PlayerContoller playerController;

    [SerializeField] private TMP_Text bottomText;
    [SerializeField] private TMP_Text cancelText;
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
                bottomText.fontSize = 32;
                bottomText.text = "(e) to Delete";
                cancelText.text = "(c) to Cancel";
                break;
            case PlayerContoller.ItemInHands.PowerLine:
                bottomText.fontSize = 20;
                bottomText.text = "place powerlines from green to red";
                cancelText.text = "(c) to Cancel";
                break;
            case PlayerContoller.ItemInHands.PowerPole:
                bottomText.fontSize = 32;
                bottomText.text = "(e) place power poles";
                break;
            default:
                bottomText.text = "";
                cancelText.text = "";
                break;
            
        }
    }
}
