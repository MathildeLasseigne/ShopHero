using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[Serializable]
public class IngredientValue
{
    [field: SerializeField, Min(0)]
    public  float rougeValue { get; private set; } = 0;
    [field: SerializeField, Min(0)]
    public float bleuValue { get; private set; } = 0;
    [field: SerializeField, Min(0)]
    public float jauneValue { get; private set; } = 0;

    public IngredientValue(float rougeValue = 0, float bleuValue = 0, float jauneValue = 0)
    {
        this.rougeValue = rougeValue;
        this.bleuValue = bleuValue;
        this.jauneValue = jauneValue;
        CheckAndRemoveNegativeValues();
    }

    public void Add(IngredientValue ingredient)
    {
        rougeValue += ingredient.rougeValue;
        bleuValue += ingredient.bleuValue;
        jauneValue += ingredient.jauneValue;
    }

    public void Remove(IngredientValue ingredient)
    {
        rougeValue -= ingredient.rougeValue;
        bleuValue -= ingredient.bleuValue;
        jauneValue -= ingredient.jauneValue;
        CheckAndRemoveNegativeValues();
    }

    public void AddRouge(float addedValue)
    {
        rougeValue += addedValue;
        CheckAndRemoveNegativeValues();
    }

    public void AddBleu(float addedValue)
    {
        bleuValue += addedValue;
        CheckAndRemoveNegativeValues();
    }

    public void AddJaune(float addedValue)
    {
        jauneValue += addedValue;
        CheckAndRemoveNegativeValues();
    }

    public void SetValues(float rougeValue, float bleuValue, float jauneValue)
    {
        this.rougeValue = rougeValue;
        this.bleuValue = bleuValue;
        this.jauneValue = jauneValue;
        CheckAndRemoveNegativeValues();
    }

    public void RemoveAll()
    {
        rougeValue = 0;
        bleuValue = 0;
        jauneValue = 0;
    }

    public void CapValuesAt(float maxValue)
    {
        if (rougeValue > maxValue)
            rougeValue = maxValue;

        if (jauneValue > maxValue)
            jauneValue = maxValue;

        if (bleuValue > maxValue)
            bleuValue = maxValue;
    }

    private void CheckAndRemoveNegativeValues()
    {
        if (rougeValue < 0)
            rougeValue = 0;
        if (bleuValue < 0)
            bleuValue = 0;
        if (jauneValue < 0)
            jauneValue = 0;
    }
}
