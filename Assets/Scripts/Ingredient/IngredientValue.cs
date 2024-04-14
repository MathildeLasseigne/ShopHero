using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IngredientValue
{
    public int rougeValue = 0;
    public int bleuValue = 0;
    public int jauneValue = 0;

    public void Add(IngredientValue ingredient)
    {
        rougeValue += ingredient.rougeValue;
        bleuValue += ingredient.bleuValue;
        jauneValue += ingredient.jauneValue;
    }
}
