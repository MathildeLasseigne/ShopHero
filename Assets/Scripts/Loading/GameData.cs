using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.IO;

public class GameData : MonoBehaviour
{


    public List<Character> characterList;

    public List<Ingredient> ingredientsList;


    public void Load(Action callback, string jsonDataPath, string ingredientDataPath, string ingredientSpriteFolderPath, 
        string charactersDataFolderPath, string characterSpriteFolderPath)
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


        callback?.Invoke();
    }
}
