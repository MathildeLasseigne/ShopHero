using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class JSONSerializer : MonoBehaviour
{
    public List<IngredientData> data;

    [ContextMenu("Print Json")]
    public void PrintJson()
    {
        string json = JsonConvert.SerializeObject(data);
        Debug.Log(json);
    }
}
