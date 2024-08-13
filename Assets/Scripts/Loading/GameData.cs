using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.IO;

//Hold and load the game data
public class GameData : MonoBehaviour
{

    public static GameData Instance;

    public Data DynamicData = new Data();


    public List<Character> characterList;

    public List<Ingredient> ingredientsList;


    //Persistent Data TODO bring from the Data class in MainGameManager class


    //TODO system variables



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






    public void Load(Action callback, string jsonDataPath, string ingredientDataPath, string ingredientSpriteFolderPath, 
        string charactersDataFolderPath, string characterSpriteFolderPath, string dialogueVarsNamesDataPath)
    {

        //Ingredients
        string pathIngredients = jsonDataPath + ingredientDataPath;
        TextAsset jsonObj = Resources.Load(pathIngredients) as TextAsset;
        if(jsonObj == null)
        {
            Debug.LogError("[GameData] Could not Load Ingredient Json at path : " + pathIngredients);
            return;
        }
        List<IngredientData> ingredientsDataList = new List<IngredientData>();
        ingredientsDataList = JsonConvert.DeserializeObject<List<IngredientData>>(jsonObj.text);

        foreach(IngredientData ingData in ingredientsDataList)
        {
            Ingredient ing = new Ingredient(ingData);
            ing.LoadIngredient(ingredientSpriteFolderPath);
            ingredientsList.Add(ing);
        }

        Debug.Log("[GameData.Load] Ingredients loaded from JSON");

        //Characters

        string pathCharacterFolder = jsonDataPath + charactersDataFolderPath;
        TextAsset[] charactersJson = Resources.LoadAll<TextAsset>(pathCharacterFolder);
        if(charactersJson == null)
        {
            Debug.LogError("[GameData] Could not Load Characters Json at path : " + pathCharacterFolder);
            return;
        }
        if(charactersJson.Length == 0)
        {
            Debug.LogError("[GameData] No Characters Json present at path : " + pathCharacterFolder);
            return;
        }
        foreach (TextAsset json in charactersJson)
        {
            CharacterData characterData = JsonConvert.DeserializeObject<CharacterData>(json.text);
            Character newCharacter = new Character(characterData);
            newCharacter.Load(characterSpriteFolderPath);
            characterList.Add(newCharacter);
        }

        Debug.Log("[GameData.Load] Characters loaded from JSON");

        //Dialogue system variables names

        string pathDialogueVars = jsonDataPath + dialogueVarsNamesDataPath;
        TextAsset diagVarsNamesObj = Resources.Load(pathIngredients) as TextAsset;
        if (diagVarsNamesObj == null)
        {
            Debug.LogError("[GameData] Could not Load Dialogue system variables names Json at path : " + pathDialogueVars);
            return;
        }
        DialogueSystemVariablesData diagVarsData = new DialogueSystemVariablesData();
        diagVarsData = JsonConvert.DeserializeObject<DialogueSystemVariablesData>(diagVarsNamesObj.text);

        DynamicData.systemVariables = new DialogueSystemVariables(diagVarsData);

        callback?.Invoke();
    }


    #region Debug

    public void DebugAddRandomIngredientsToInventory()
    {
        if (ingredientsList == null)
        {
            Debug.Log("[GameData.DebugAddRandomIngredientsInInventory] Ingredients not loaded");
        } else
        {
            foreach (Ingredient ingredient in GameData.Instance.ingredientsList)
            {
                int randomValue = UnityEngine.Random.Range(0, 100);
                if(randomValue < 30)
                {
                    ingredient.AddToInventory();
                }
            }
        }
    }


    #endregion



}



[Serializable]
public class Data //Persistent data
{
    public static Data mainInstance;

    public List<Ingredient> AllIngredients;

    public List<Character> allCharactersList = new List<Character>();

    public List<string> archivedInterventionsIds = new List<string>(); //The list of the id of all interventions already played

    public DialogueSystemVariables systemVariables;



    public Config mainConfig;

    public int Gold = 0;

    public void SetSelfAsMain()
    {
        mainInstance = this;
    }
}


public class DataDetail
{
    public const int SCORE_INCREASE_SIMPLE = 10;
    public const int SCORE_INCREASE_DOUBLE = 20;
}
