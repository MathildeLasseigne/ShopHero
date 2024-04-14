using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgingMinigameController : MonoBehaviour
{

    [SerializeField] List<TapGameController> gameControllersList = new List<TapGameController>();

    void StartMinigame()
    {
        foreach (TapGameController controller in gameControllersList)
        {
            if (controller.gameObject.active == false)
                continue;
            controller.Init();
            controller.StartGame();
        }
    }


    void TileTouchedMinigame(bool isDoublePoint)
    {
        if (! isDoublePoint)
        {
            MainGameManager.Instance.AddToScore(DataDetail.SCORE_INCREASE_SIMPLE);
        } else
        {
            MainGameManager.Instance.AddToScore(DataDetail.SCORE_INCREASE_DOUBLE);
        }
        MainGameManager.Instance.SoundBoard.SourceTappingGame.PlayOneShot(MainGameManager.Instance.SoundBoard.ClipTileTaped);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
