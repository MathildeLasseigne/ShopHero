using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class JSONSerializer : MonoBehaviour
{
    public List<IngredientData> data;

    public DialogueSystemVariablesData dataDialogueVars;

    public CharacterData characterData;

    [ContextMenu("Print Json")]
    public void PrintJson()
    {
        string json = JsonConvert.SerializeObject(characterData);
        Debug.Log(json);
    }
}
