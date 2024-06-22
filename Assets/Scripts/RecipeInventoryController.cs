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

    [SerializeField] private UnityEngine.UI.Button endRecipeButton;


    [SerializeField] private IngredientValue mixIngredientValue;

    private int maxIngredientValue = 3;

    private bool isSelected = false;

    [SerializeField] private List<Ingredient> currentIngredients = new List<Ingredient>();

    private Action<IngredientValue> RecipeFinishedEvent;

    public void Init()
    {
        // Cleaning
        currentIngredients.Clear();
        mixIngredientValue.RemoveAll();

        endRecipeButton.enabled = true;

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
        if (Input.GetKeyDown(Data.mainInstance.mainConfig.debugPassSceneKey))
            RecipeFinishedEvent?.Invoke(new IngredientValue(1,1,1));
    }

    void UpdateImage()
    {
        IngredientValue value = GetMixIngredientValuesCaped();

        int step = 85; //  85 = 255/3 because 3 values
        float r = Utils.PreventGoingUnder(255 - (value.bleuValue * step), 0);
        float g = Utils.PreventGoingUnder(255 - (value.bleuValue * step) - (value.rougeValue * step), 0);
        float b = Utils.PreventGoingUnder(255 - (value.jauneValue * step) - (value.rougeValue * step), 0);

        Color newTint = new Color(r/255f, g / 255f, b / 255f); //Color is not by 255 but 1
        mixResultImage.color = newTint;
    }
     

    IngredientValue GetMixIngredientValuesCaped()
    {
        return this.mixIngredientValue.Copy().CapValuesAt(maxIngredientValue);
    }

    public void AddIngredient(Ingredient ingredient)
    {
        currentIngredients.Add(ingredient);
        mixIngredientValue.Add(ingredient.ingredientValue);
        UpdateImage();
    }

    public void RemoveIngredient(Ingredient ingredient)
    {
        currentIngredients.Remove(ingredient);
        mixIngredientValue.Remove(ingredient.ingredientValue);
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
        if (mixIngredientValue.IsEmpty())
            return;
        else
        {
            endRecipeButton.enabled = false; // prevent double click

            foreach (Ingredient ingredient in currentIngredients)
            {
                ingredient.Use();
            }
            RecipeFinishedEvent?.Invoke(GetMixIngredientValuesCaped());
        }
    }

    public void SuscribeToRecipeFinishedEvent(Action<IngredientValue> callback)
    {
        RecipeFinishedEvent += callback;
    }

}
