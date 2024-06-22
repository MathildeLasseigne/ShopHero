using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    /*public static void DoAfterDelay(float delay, Action callback)
    {
        StartCoroutine(Utils.DoAfterDelayCoroutine(delay, callback));
    }

    private static IEnumerator DoAfterDelayCoroutine(float delay, Action callback)
    {
        yield return new WaitForSeconds(delay);
        callback?.Invoke();
    }*/

    public static int MaxInt(params int[] integers)
    {
        if(integers.Length == 0 || integers == null) return 0;
        int max = integers[0];
        foreach(int i in integers)
        {
            if(i > max) max = i;
        }
        return max;
    }

    public static float MaxFloat(params float[] floaters)
    {
        if (floaters.Length == 0 || floaters == null) return 0f;
        float max = floaters[0];
        foreach (int i in floaters)
        {
            if (i > max) max = i;
        }
        return max;
    }

    public static float MinFloat(params float[] floaters)
    {
        if (floaters.Length == 0 || floaters == null) return 0f;
        float min = floaters[0];
        foreach (int i in floaters)
        {
            if (i < min) min = i;
        }
        return min;
    }

    public static float PreventGoingUnder(float var, float min)
    {
        if (var < min)
            var = min;
        return var;
    }

    /// <summary>
    /// Calculate
    /// <br/>
    /// <code>
    ///     refOrigin  -> refCalculated
    ///     varOrigin  ->     ???
    /// </code>
    ///  This code prevent any division by 0
    /// </summary>
    /// <param name="varOrigin"></param>
    /// <param name="refOrigin"></param>
    /// <param name="refCalculated"></param>
    /// <returns> ??? = (varOrigin * refCalculated) /  refOrigin </returns>
    public static float ProduitEnCroix(float varOrigin, float refOrigin, float refCalculated)
    {
        float result = refOrigin == 0 ? 0 : (varOrigin * refCalculated) / refOrigin;
        return result;
    }
}
