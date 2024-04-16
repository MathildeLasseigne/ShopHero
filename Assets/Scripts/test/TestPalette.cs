using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TestPalette : MonoBehaviour
{

    public ColorMixing colorMix = new ColorMixing();

    // Update is called once per frame
    void Update()
    {
        colorMix.DebugTestColor();
        colorMix.SecondaryColor.DebugTestColor();
    }
}
