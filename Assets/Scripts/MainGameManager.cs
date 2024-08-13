using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{

    public static MainGameManager Instance;

    public SoundBoard SoundBoard;

    public ForgingMinigameController forgingMinigameController;


    [SerializeField] TextMeshProUGUI textScore;


    [SerializeField] private int currentScore = 0; //Score for the current game interaction, influence other rewards like gold and favorability

    [SerializeField] GameObject Intro;
    [SerializeField] Animator animatorLogo;




    private void Awake()
    {
        //Singleton
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start main");
        StartCoroutine(StartGame());
    }

    IEnumerator StartGame()
    {
        if (animatorLogo)
            animatorLogo.SetTrigger("Go");
        yield return new WaitForSeconds(5f);
        Intro.SetActive(false);
        yield return new WaitForSeconds(3f);

        Character character = GameData.Instance.DynamicData.allCharactersList[UnityEngine.Random.Range(0, GameData.Instance.DynamicData.allCharactersList.Count - 1)]; // Choose character at random
        forgingMinigameController.Init(character);
        forgingMinigameController.StartMinigame();
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

    public List<Ingredient> GetAllOwnedIngredients()
    {
        List<Ingredient> inventory = new List<Ingredient>();
        foreach (Ingredient ingredient in GameData.Instance.ingredientsList)
        {
            if(ingredient.nbInInventory >  0)
                inventory.Add(ingredient);
        }
        return inventory;
    }

    void AddGold(int gold)
    {
        GameData.Instance.DynamicData.Gold += gold;
    }

    public void NextStep()
    {
        //forgingMinigameController.NextStep();
    }


}
