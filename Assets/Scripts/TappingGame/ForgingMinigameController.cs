using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgingMinigameController : MonoBehaviour
{
    [Header("Forging")]
    [SerializeField] List<TapGameController> laneOtherControllersList = new List<TapGameController>();
    [SerializeField] private TapGameController laneRouge;
    [SerializeField] private TapGameController laneBleue;
    [SerializeField] private TapGameController laneJaune;

    [SerializeField] private MidiGameController laneRougeMidi;
    [SerializeField] private MidiGameController laneBleueMidi;
    [SerializeField] private MidiGameController laneJauneMidi;

    [SerializeField] GameObject forgingTable;
    [SerializeField] GameObject tapingSword;

    [Header("Reciping")]
    [SerializeField] RecipeInventoryController inventoryController;
    [SerializeField] GameObject recipeTable;

    [Header("Dialogue")]
    [SerializeField] DialogueController dialogueController;
    [SerializeField] GameObject dialogueObject;

    [Header("End")]
    [SerializeField] GameObject endGameObject;

    Character currentCharacter;

    IngredientValue currentIngredientValue = new IngredientValue();

    private  enum Step { Reciping, Forging, EndEvent};

    private Step currentStep;

    public void Init(Character character)
    {
        #region  lanes setup
        foreach (TapGameController controller in laneOtherControllersList)
        {
            setupLane(controller);
        }

        setupLane(laneRouge);
        setupLane(laneBleue);
        setupLane(laneJaune);

        laneRouge.SetKeyCode(Data.mainInstance.mainConfig.laneRougeTappingKey);
        laneBleue.SetKeyCode(Data.mainInstance.mainConfig.laneBleueTappingKey);
        laneJaune.SetKeyCode(Data.mainInstance.mainConfig.laneJauneTappingKey);

        laneRouge.SetMidiController(laneRougeMidi);
        laneBleue.SetMidiController(laneBleueMidi);
        laneJaune.SetMidiController(laneJauneMidi);

        void setupLane(TapGameController controller)
        {
            controller.SuscribeToTileTouchEvent(TileTouchedMinigame);
            if (tapingSword)
                controller.SetLaneEndPoint(tapingSword.transform);
        }

        #endregion

        if (! Data.mainInstance.mainConfig.debug && ! Data.mainInstance.mainConfig.debugForgingMiniGameStopAuto)
            dialogueController.SuscribeToDialogueFinishedEvent(NextStep);
        inventoryController.SuscribeToRecipeFinishedEvent((ingredientValue) => 
        {
            SetCurrentIngredientValue(ingredientValue);
            NextStep(); 
        });

        currentIngredientValue.RemoveAll();

        this.currentCharacter = character;
        dialogueController.SetCharacter(character);

        
    }

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

        if (laneRouge.IsShowing())
        {
            laneRouge.SetDifficulty((int)currentIngredientValue.rougeValue);
            laneRouge.Init().StartGame();
        }
        if (laneBleue.IsShowing())
        {
            laneBleue.SetDifficulty((int)currentIngredientValue.bleuValue);
            laneBleue.Init().StartGame();
        }
        if (laneJaune.IsShowing())
        {
            laneJaune.SetDifficulty((int)currentIngredientValue.jauneValue);
            laneJaune.Init().StartGame();
        }

        foreach (TapGameController controller in laneOtherControllersList)
        {
            if (controller.gameObject.activeInHierarchy == false)
                continue;
            controller.Init().StartGame();
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
        laneRouge.Show();
        laneBleue.Show();
        laneJaune.Show();
    }

    void HideForgingTable()
    {
        foreach (TapGameController controller in laneOtherControllersList)
        {
            if (controller.gameObject.activeInHierarchy == false)
                continue;
            controller.StopGame().Hide();
        }
        laneRouge.StopGame().Hide();
        laneBleue.StopGame().Hide();
        laneJaune.StopGame().Hide();

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

    private void SetCurrentIngredientValue(IngredientValue ingredient)
    {
        this.currentIngredientValue.Copy(ingredient);
    }
}
