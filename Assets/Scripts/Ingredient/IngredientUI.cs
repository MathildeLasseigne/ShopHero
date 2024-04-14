using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class IngredientUI : MonoBehaviour
{

    [SerializeField] private bool _IsSelected = false;

    private Button _Button;
    public Image _Image;
    public TextMeshProUGUI quantityValue;

    public Color _ColorSelected = new Color();
    public Color _ColorUnselected = new Color();

    private RecipeInventoryController controller;

    [SerializeField] private Ingredient ingredient;
    void Start()
    {
        _Button = gameObject.GetComponent<Button>();
        _Button.onClick.AddListener(ButtonClicked);

        //setColor();
    }

    public void SetController(RecipeInventoryController controller)
    {
        this.controller = controller;
    }

    public void SetIngredient(Ingredient ingredient)
    {
        this.ingredient = ingredient;
        _Image.sprite = ingredient.recipeSprite;
        quantityValue?.SetText(ingredient.nbInInventory + "");
        SetColor();
    }



    public Ingredient getIngredient()
    {
        return ingredient;
    }


    private void ButtonClicked()
    {
        if (!_IsSelected)
        {
            _IsSelected = true;
            controller.AddIngredient(ingredient);
        }
        else
        {
            _IsSelected = false;
            controller.RemoveIngredient(ingredient);
        }

        SetColor();
    }

    void SetColor()
    {
        if(_IsSelected)
        {
            _Image.color = _ColorSelected;
        } else
        {
            _Image.color = _ColorUnselected;
        }
    }

    public void OnPointerEnter()
    {
        controller.DiplayIngredientDescription(ingredient.descriptionSprite);
    }

    public void OnPointerExit()
    {
        controller.HideIngredientDescription();
    }
}
