using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class LoadingManager : MonoBehaviour
{
    [SerializeField] private GameData GameData;

    [Header("Animation")]

    [SerializeField] private string AnimatorLogoStartName;
    [SerializeField] private string AnimatorLogoEndName;

    [SerializeField] private AnimatorHelper AnimatorHelper;


    [Header("Paths")]

    //In Assets/Resources/Sprites/Ingredients/MyImage.png
    [SerializeField] string IngredientSpriteFolderPath = "Sprites/Ingredients/";

    [SerializeField] string JsonDataPath = "JSONData/";

    [SerializeField] string IngredientDataPath = "IngredientsData";

    [SerializeField] string CharactersDataFolderPath = "Characters"; 

    [SerializeField] string characterSpriteFolderPath = "Sprites/Characters/";







    // Start is called before the first frame update
    void Start()
    {
        //object = JsonUtility.FromJson<Object>(path);
    }

    // Update is called once per frame
    void Update()
    {
        //JsonUtility.FromJson<IngredientData>(AriseConfig.GetTranslationsLanguages());
    }

    [ContextMenu("Load")]
    public void Load()
    {
        GameData.Load(null, JsonDataPath, IngredientDataPath, IngredientSpriteFolderPath, CharactersDataFolderPath, characterSpriteFolderPath);
    }
}
