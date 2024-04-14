using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeInventoryController : MonoBehaviour
{

    [SerializeField] private GameObject ingredientUIPrefab;
    [SerializeField] private GameObject scrollViewContent;

    [SerializeField] private Image ingredientsDescription;

    [SerializeField] private Image mixResultImage;

    [SerializeField] private Sprite endRecipeDescription;

    private int maxIngredientValue = 3;

    private bool isSelected = false;

    private List<Ingredient> currentIngredients = new List<Ingredient>();

    private Action RecipeFinishedEvent;

    public void Init()
    {
        // Cleaning
        currentIngredients.Clear();

        for (int i = scrollViewContent.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(scrollViewContent.transform.GetChild(i).gameObject);
        }
        //Get
        foreach(Ingredient ingredient in MainGameManager.Instance.GetAllOwnedIngredients())
        {
            var item_go = Instantiate(ingredientUIPrefab);
            // do something with the instantiated item -- for instance
            IngredientUI ui = item_go.GetComponentInChildren<IngredientUI>();
            //parent the item to the content container
            item_go.transform.SetParent(scrollViewContent.transform);
            //reset the item's scale -- this can get munged with UI prefabs
            item_go.transform.localScale = Vector2.one;

            ui.SetController(this);
            ui.SetIngredient(ingredient);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void UpdateImage()
    {
        IngredientValue value = GetMixIngredientValues();

        int step = 85; // because 3 values
        float r = PreventGoingUnder(255 - (value.bleuValue * step), 0);
        float g = PreventGoingUnder(255 - (value.bleuValue * step) - (value.rougeValue * step), 0);
        float b = PreventGoingUnder(255 - (value.jauneValue * step) - (value.rougeValue * step), 0);

        Color newTint = new Color(r/255f, g / 255f, b / 255f); //Color is not by 255 but 1
        mixResultImage.color = newTint;
    }
     
    float PreventGoingUnder(float var, float x)
    {
        if (var < x)
            var = x;
        return var;
    }

    IngredientValue GetMixIngredientValues()
    {
        IngredientValue value = new IngredientValue();
        foreach(Ingredient ingredient in currentIngredients)
        {
            value.Add(ingredient.ingredientValue);
        }
        if (value.rougeValue >= maxIngredientValue)
            value.rougeValue = maxIngredientValue;
        if (value.jauneValue >= maxIngredientValue)
            value.jauneValue = maxIngredientValue;
        if (value.bleuValue >= maxIngredientValue)
            value.bleuValue = maxIngredientValue;
        
        return value;
    }

    public void AddIngredient(Ingredient ingredient)
    {
        currentIngredients.Add(ingredient);
        UpdateImage();
    }

    public void RemoveIngredient(Ingredient ingredient)
    {
        currentIngredients.Remove(ingredient); 
        UpdateImage();
    }

    public void DiplayIngredientDescription(Sprite description)
    {
        ingredientsDescription.sprite = description;
        ingredientsDescription?.gameObject.SetActive(true);
    }

    public void HideIngredientDescription()
    {
        ingredientsDescription?.gameObject.SetActive(false);
    }

    public void DiplayEndRecipeDescription()
    {
        ingredientsDescription.sprite = endRecipeDescription;
        ingredientsDescription?.gameObject.SetActive(true);
    }

    public void HideEndRecipeDescription()
    {
        ingredientsDescription?.gameObject.SetActive(false);
    }

    public void OnClickEndRecipeDescription()
    {
        RecipeFinishedEvent?.Invoke();
    }

    public void SuscribeToRecipeFinishedEvent(Action callback)
    {
        RecipeFinishedEvent += callback;
    }


}
