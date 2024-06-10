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

    public Data DynamicData = new Data();

    [SerializeField] TextMeshProUGUI textScore;


    [SerializeField] private int currentScore = 0;

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
        DynamicData.SetSelfAsMain();
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

        Character character = DynamicData.allCharactersList[UnityEngine.Random.Range(0, DynamicData.allCharactersList.Count - 1)]; // Choose character at random
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
        foreach (Ingredient ingredient in DynamicData.AllIngredients)
        {
            if(ingredient.nbInInventory >  0)
                inventory.Add(ingredient);
        }
        return inventory;
    }

    void AddGold(int gold)
    {
        DynamicData.Gold += gold;
    }

    public void NextStep()
    {
        //forgingMinigameController.NextStep();
    }


}

[Serializable]
public class Data
{
    public static Data mainInstance;

    public List<Ingredient> AllIngredients;

    public List<Character> allCharactersList = new List<Character>();

    public Config mainConfig;

    public int Gold = 0;

    public void SetSelfAsMain()
    {
        mainInstance = this;
    }
}
