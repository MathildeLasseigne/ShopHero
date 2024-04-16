using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;
using UnityEditor;

[Serializable]
public class ColorMixing
{
    [field: SerializeField] public IngredientValue colorIngredientValue { get; private set; } = new IngredientValue();


    [field: SerializeField, Range(0f, 1f)] public float minHSVValue { get; private set; } = 0;

    [field: SerializeField, Min(0)] public float maxIngredientValue { get; private set; } = 0;

    [field: SerializeField] public SecondaryColors SecondaryColor { get; private set; }

    /// <summary>
    /// The regular RGB base = 255. Be careful, unity color base is 1 ! 
    /// </summary>
    /// <see cref="ScaleColorForUnity"/>
    private static readonly float RGBBase = 255f; // The regular 
    private static readonly float HSVBase = 1f; // HSV base is 100 but unity colors scale it to 1. Here, it is only used in the context of unity colors


    public ColorMixing() { 
        SecondaryColor = new SecondaryColors(this);
    }

    #region Script setters

    /// <summary>
    /// Min HSV value for the color mixing.
    /// <br/> 0 <= value <= 1
    /// </summary>
    /// <param name="minHSVVAlue"></param>
    public void SetMinHSVValue(float minHSVVAlue)
    {
        this.minHSVValue = minHSVVAlue < 0 ? 0 : minHSVVAlue > ColorMixing.HSVBase ? ColorMixing.HSVBase : minHSVVAlue;
    }

    /// <summary>
    /// Max ingredient value for the color mixing. Will be considered to be black
    /// <br/> 0 <= value <= 1
    /// </summary>
    /// <param name="minHSVVAlue"></param>
    public void SetmaxIngredientValue(int maxIngredientValue)
    {
        this.maxIngredientValue = maxIngredientValue < 0 ? 0 : maxIngredientValue;
    }

    #endregion

    #region Color calculations
    public static Color GetColorMix(IngredientValue ingredientValue, float minHSVValue = 0, float maxIngredientValue = 0, bool p_showDebugsLogs = false)
    {
        showDebugsLogsStatic = p_showDebugsLogs;

        /* Matrice couleurs : (colors used in game are red, blue & yellow)
         
             r    g    b

        R = 255,  0,   0
        B =  0,   0,  255
        J = 255, 255,  0

         */


        float r = ingredientValue.rougeValue + ingredientValue.jauneValue;
        float g = ingredientValue.jauneValue;
        float b = ingredientValue.bleuValue;

        float maxColorValue;
        if (maxIngredientValue > 0)
            maxColorValue = maxIngredientValue;
        else
            maxColorValue = Utils.MaxFloat(r, g, b);

        if(showDebugsLogsStatic)
            Debug.Log("Without calcul : r = " + r + ", g = " + g + ", b = " + b);

        r = AsColorRGB(r, maxColorValue);
        g = AsColorRGB(g, maxColorValue);
        b = AsColorRGB(b, maxColorValue);

        if (showDebugsLogsStatic)
            Debug.Log("With calcul : r = " + r + ", g = " + g + ", b = " + b);

        
        return InverseRGBScale(ColorMixing.ScaleColorForUnity(new Color(r, g, b)), minHSVValue);
    }

    public Color GetColorMix()
    {
        return ColorMixing.GetColorMix(colorIngredientValue, minHSVValue, maxIngredientValue, showDebugsLogs); //Color is not by 255 but 1
    }


    /// <summary>
    /// Inverse -> white become 0,0,0 and black 255,255,255.
    /// Darken original colors
    /// </summary>
    /// <param name="originalColor"> Must be unity colors, which do not have base 255 but 1 </param>
    /// <param name="minHSVValue"> To prevent color from being too dark </param>
    /// <returns></returns>
    private static Color InverseRGBScale(Color originalColor, float minHSVValue = 0)
    {
        float H, S, V;

        Color.RGBToHSV(originalColor, out H, out S, out V);
        float newV = HSVBase - V; //Inverse value

        if (showDebugsLogsStatic)
            Debug.Log("H: " + H + " S: " + S + " V: " + newV);


        newV += minHSVValue;
        if(newV > HSVBase)
            newV = HSVBase;

        if (showDebugsLogsStatic)
            Debug.Log("Calculate H: " + H + " S: " + S + " V: " + newV);

        return Color.HSVToRGB(H, S, newV);
    }

    /// <summary>
    /// Use before using any Color method when working with regular RGB values .
    /// <br/> Unity color is not by 255 but 1.
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    private static Color ScaleColorForUnity(Color c)
    {
        return new Color(c.r / RGBBase, c.g / RGBBase, c.b / RGBBase);
    }

    /// <summary>
    /// Convert the values from ingredients to proper RGB colors
    /// </summary>
    /// <param name="x"> The value from the ingredient </param>
    /// <param name="currentMaxValue"> The ingredient value equivalent to the max RGB value</param>
    /// <returns></returns>
    private static float AsColorRGB(float x, float currentMaxValue)
    {
        return Utils.ProduitEnCroix(x, currentMaxValue, RGBBase);
    }

    

    #endregion


    #region Debug

    [Header("Debug")]

    [SerializeField] private bool showDebugsLogs = false; //Auto set showDebugsLogs in static method
    private static bool showDebugsLogsStatic = false;

    [SerializeField] private Color debugColorTester = new Color();

    /// <summary>
    /// Call from another script to have color result show in debugColorTester
    /// </summary>
    public void DebugTestColor()
    {
        debugColorTester = GetColorMix();
    }
    #endregion

    /// <summary>
    /// Is used when wanting to reach the color already defined by ColorMixing
    /// </summary>
    [Serializable]
    public class SecondaryColors
    {
        [SerializeField] private ColorMixing originalColor;

        [field: SerializeField] public IngredientValue secondaryColorIngredientValue { get; private set; } = new IngredientValue();

        /// <summary>
        /// The ceiling for secondaryColorIngredientValue. Needed to calculate the target color
        /// </summary>
        [field: SerializeField] public IngredientValue maximumIngredientAmount { get; private set; } = new IngredientValue();


        public SecondaryColors(ColorMixing originalColor)
        {
            this.originalColor = originalColor;
        }


        public Color GetColorMix()
        {
            IngredientValue scaledValues; //if the original color has value 3, and the max amount for secondary is 84, then calculate the correct ratio of the current ammount

            float r = Utils.ProduitEnCroix(secondaryColorIngredientValue.rougeValue, maximumIngredientAmount.rougeValue, originalColor.colorIngredientValue.rougeValue);
            float b = Utils.ProduitEnCroix(secondaryColorIngredientValue.bleuValue, maximumIngredientAmount.bleuValue, originalColor.colorIngredientValue.bleuValue);
            float j = Utils.ProduitEnCroix(secondaryColorIngredientValue.jauneValue, maximumIngredientAmount.jauneValue, originalColor.colorIngredientValue.jauneValue);

            scaledValues = new IngredientValue(r, b, j);

            return ColorMixing.GetColorMix(scaledValues, originalColor.minHSVValue, 
                originalColor.maxIngredientValue > 0 ? originalColor.maxIngredientValue : GetMaxColorValueFromIngredient(originalColor.colorIngredientValue)); ;
        }

        private float GetMaxColorValueFromIngredient(IngredientValue ingredientValue)
        {
            float r = ingredientValue.rougeValue + ingredientValue.jauneValue;
            float g = ingredientValue.jauneValue;
            float b = ingredientValue.bleuValue;

            return Utils.MaxFloat(r, g, b);
        }


        #region Debug

        [Header("Debug")]

        [SerializeField] private Color debugColorTester = new Color();

        /// <summary>
        /// Call from another script to have color result show in debugColorTester
        /// </summary>
        public void DebugTestColor()
        {
            debugColorTester = GetColorMix();
        }
        #endregion

    }

}
