using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public IngredientValue ingredientValue = new IngredientValue();
    public string nameIngredient = "";
    public string description;
    public int goldValue = 0;

    public int nbInInventory = 0;

    public Sprite recipeSprite;
    public Sprite descriptionSprite;
    public Sprite merchandSprite;


    public void AddToInventory()
    {
        nbInInventory++;
    }

    public void Use()
    {
        nbInInventory--;
    }
}
