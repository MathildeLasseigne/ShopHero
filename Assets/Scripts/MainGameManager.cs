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
    private void Awake()
    {
        Instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Start main");
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


    [Serializable]
    public class Data
    {
        public List<Ingredient> AllIngredients;
    }

}
