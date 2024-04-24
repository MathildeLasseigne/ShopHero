using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgingMinigameController : MonoBehaviour
{

    [SerializeField] List<TapGameController> gameControllersList = new List<TapGameController>();
    [SerializeField] GameObject forgingTable;
    [SerializeField] GameObject tapingSword;

    [SerializeField] RecipeInventoryController inventoryController;
    [SerializeField] GameObject recipeTable;

    [SerializeField] DialogueController dialogueController;
    [SerializeField] GameObject dialogueObject;

    [SerializeField] GameObject endGameObject;

    Character character;

    private  enum Step { Reciping, Forging, EndEvent};

    private Step currentStep;



    public void Init()
    {
        foreach (TapGameController controller in gameControllersList)
        {
            controller.SuscribeToTileTouchEvent(TileTouchedMinigame);
            if (tapingSword)
                controller.SetLaneEndPoint(tapingSword.transform);
        }
        dialogueController.SuscribeToDialogueFinishedEvent(NextStep);
        inventoryController.SuscribeToRecipeFinishedEvent(NextStep);
    }

    public void StartMinigame(Character character)
    {
        this.character = character;
        dialogueController.SetCharacter(character);
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
        else if(currentStep == Step.Forging)
        {
            StartCoroutine(StartEndEvent());
            currentStep = Step.EndEvent;
        }
        else if(currentStep == Step.EndEvent)
        {
            StartCoroutine(FinishEndEvent());
        }
    }

    #region Start / End step

    IEnumerator StartReciping()

    {
        dialogueController.Init();
        //dialogueController.SetEntrance(hasEntrance: true);
        ShowDialogue();
        dialogueController.BeginDialogue();

        ShowReceipingTable() ;
        inventoryController.Init();
        yield return new WaitForSeconds(2); //Time of anim
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
        }
        MainGameManager.Instance.SoundBoard.SourceTappingGame.Play();
    }

    IEnumerator StartEndEvent()
    {
        HideForgingTable();
        HideDialogue();
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

    #endregion

    #region Show / Hide

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
        endGameObject?.SetActive(true);
    }

    void HideEndEvent()
    {
        endGameObject?.SetActive(false);
    }

    void ShowDialogue()
    {
        dialogueController.Show();
    }

    void HideDialogue()
    {
        dialogueController.Hide();
    }

#endregion

    void TileTouchedMinigame(bool isDoublePoint)
    {
        if (! isDoublePoint)
        {
            MainGameManager.Instance.AddToScore(DataDetail.SCORE_INCREASE_SIMPLE);
        } else
        {
            MainGameManager.Instance.AddToScore(DataDetail.SCORE_INCREASE_DOUBLE);
        }
        //MainGameManager.Instance.SoundBoard.SourceTappingGame.PlayOneShot(MainGameManager.Instance.SoundBoard.ClipTileTaped);
    }


}
