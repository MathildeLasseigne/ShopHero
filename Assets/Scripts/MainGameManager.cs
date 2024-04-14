using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{

    public static MainGameManager Instance;

    public SoundBoard SoundBoard;

    [SerializeField] TextMeshProUGUI textScore;


    [SerializeField] List<TapGameController> gameControllersList = new List<TapGameController>();


    [SerializeField] private int currentScore = 0;
    private void Awake()
    {
        Instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start main");
        foreach(TapGameController controller in gameControllersList)
        {
            if (controller.gameObject.active == false)
                continue;
            controller.Init();
            controller.StartGame();
        }
    }

    public void AddToScore(int addition)
    {
        currentScore += addition;
        string text = "Score : " + currentScore;
        if (textScore)
        {
            textScore.SetText(text);
        }
            
    }


}
