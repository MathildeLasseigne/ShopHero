using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgingMinigameController : MonoBehaviour
{

    [SerializeField] List<TapGameController> gameControllersList = new List<TapGameController>();
    [SerializeField] GameObject forgingTable;

    [SerializeField] RecipeInventoryController inventoryController;
    [SerializeField] GameObject recipeTable;

    private  enum Step { Reciping, Forging, EndEvent};

    private Step currentStep;



    public void StartMinigame()
    {
        StartCoroutine(StartReciping());
        currentStep = Step.Reciping;
        //StartForging();
    }

    public void NextStep()
    {
        if(currentStep == Step.Reciping)
        {
            StartCoroutine(StartForging());
            currentStep = Step.Forging;
        }
        if(currentStep == Step.Forging)
        {
            StartCoroutine(StartEndEvent());
        }
        if(currentStep == Step.EndEvent)
        {
            StartCoroutine(FinishEndEvent());
        }
    }



    IEnumerator StartReciping()

    {
        ShowReceipingTable() ;
        yield return new WaitForSeconds(2); //Time of anim
        inventoryController.Init();
    }

    IEnumerator StartForging()
    {
        HideReceipingTable();
        yield return new WaitForSeconds(2); //Time of anim

        ShowForgingTable();
        yield return new WaitForSeconds(2); //Time of anim

        foreach (TapGameController controller in gameControllersList)
        {
            if (controller.gameObject.activeInHierarchy == false)
                continue;
            controller.Init();
            controller.StartGame();
            controller.SuscribeToTileTouchEvent(TileTouchedMinigame);
        }
    }

    IEnumerator StartEndEvent()
    {
        HideForgingTable();
        yield return new WaitForSeconds(2); //Time of anim

        ShowEndEvent();
        //CalculateEndEvent
    }

    IEnumerator FinishEndEvent()
    {
        HideEndEvent();
        yield return new WaitForSeconds(2); //Time of anim

        //Send data
    }

    void ShowForgingTable()
    {
        forgingTable?.SetActive(true);
    }

    void HideForgingTable()
    {
        foreach (TapGameController controller in gameControllersList)
        {
            if (controller.gameObject.activeInHierarchy == false)
                continue;
            controller.StopGame();
        }

        forgingTable?.SetActive(false);
    }

    void ShowReceipingTable()
    {
        recipeTable?.SetActive(true);
    }

    void HideReceipingTable()
    {
        recipeTable?.SetActive(false);
    }

    void ShowEndEvent()
    {

    }

    void HideEndEvent()
    {

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
