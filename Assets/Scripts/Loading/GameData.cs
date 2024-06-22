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


    public void Load(Action callback, string jsonDataPath, string ingredientDataPath, string ingredientSpriteFolderPath)
    {
        string pathIngredients = jsonDataPath + ingredientDataPath;
        TextAsset jsonObj = Resources.Load(pathIngredients) as TextAsset;
        if(jsonObj == null)
        {
            Debug.Log("[GameData] Could not Load Ingredient Json at path : " + pathIngredients);
            return;
        }
        List<IngredientData> ingredientsDataList = new List<IngredientData>();
        ingredientsDataList = JsonConvert.DeserializeObject<List<IngredientData>>(jsonObj.text);
        //IngredientData ingData = JsonUtility.FromJson<IngredientData>(jsonObj.text);
        //IngredientData ingData = JsonUtility.D<IngredientData>(jsonObj.text);

        foreach(IngredientData ingData in ingredientsDataList)
        {
            Ingredient ing = new Ingredient(ingData);
            ing.LoadIngredient(ingredientSpriteFolderPath);
            ingredientsList.Add(ing);
        }

        callback?.Invoke();
    }
}
