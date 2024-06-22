using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class Ingredient
{
    [SerializeField, Tooltip("The name to be displayed in the editor")] private string nameForEditor = "";


    [SerializeField] private IngredientData ingredientData;

    public IngredientValue ingredientValue
    {
        set { ingredientValue = value; }
        get { return ingredientData.ingredientValue; }
    }


    public string nameIngredient 
    {
        set { nameIngredient = value; } 
        get { return ingredientData.nameIngredient; }
    }
    public string description
    {
        set { description = value; }
        get { return ingredientData.description; }
    }
    public int goldValue
    {
        set { goldValue = value; }
        get { return ingredientData.goldValue > 0 ? ingredientData.goldValue : 0; }
    }

    [Header("Play variables")]

    public int nbInInventory = 0;

    public Sprite recipeSprite;
    public Sprite descriptionSprite;
    public Sprite merchandSprite;


    public Ingredient(IngredientData ingredientData)
    {
        this.ingredientData = ingredientData;
        nameForEditor = ingredientData.nameIngredient;
    }

    /// <summary>
    /// Load data from Ingredient data, like sprites
    /// </summary>
    public void LoadIngredient(string ingredientSpriteFolderPath)
    {
        //In Assets/Resources/Sprites/Ingredients/MyImage.png
        
        recipeSprite = Resources.Load<Sprite>(Path.Combine(ingredientSpriteFolderPath, ingredientData.recipeSpritePath));
        descriptionSprite = Resources.Load<Sprite>(Path.Combine(ingredientSpriteFolderPath, ingredientData.descriptionSpritePath));
        merchandSprite = Resources.Load<Sprite>(Path.Combine(ingredientSpriteFolderPath, ingredientData.merchandSpritePath));
    }

    public void AddToInventory()
    {
        nbInInventory++;
    }

    public void Use()
    {
        nbInInventory--;
    }

    public bool Equal(Ingredient ingredient)
    {
        return this.ingredientData.uniqueID == ingredient.ingredientData.uniqueID;
    }

    
}

[Serializable]
public class IngredientData
{
    public string uniqueID;
    public string nameIngredient;
    public string description;

    public int goldValue = 0;

    [Header("Paths")]

    public string recipeSpritePath;
    public string descriptionSpritePath;
    public string merchandSpritePath;

    [Header("Value")]

    public IngredientValue ingredientValue;
}
