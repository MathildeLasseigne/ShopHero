using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/Config")]
public class Config : ScriptableObject
{
    [Header("Debug")] // debugs vars must be false by default
    /// <summary>
    /// General debug mode. Always test when using debug variables. 
    /// If false, all other debug values should be considered false.
    /// </summary>
    public bool debug = false;
    [Space(10)]

    public bool debugForgingMiniGameStopAuto = false;
    [Min(0)] public float debugDialogueControllerAdditionnalTime = 0f;



    [Header("Forging lanes")]
    public KeyCode laneRougeTappingKey = KeyCode.A;
    public KeyCode laneBleueTappingKey = KeyCode.S;
    public KeyCode laneJauneTappingKey = KeyCode.D;

    [Space(10)]
    //Not yet used
    [Range(0, 100), Tooltip("Not linked yet")] public int difficultyRateVeryEasy = 20;
    [Range(0, 100), Tooltip("Not linked yet")] public int difficultyRateEasy = 30;
    [Range(0, 100), Tooltip("Not linked yet")] public int difficultyRateMedium = 50;
    [Range(0, 100), Tooltip("Not linked yet")] public int difficultyRateHard = 70;


    [Header("Audio")]
    [Min(0)] public double midiCorrectedTempo = 103;
}
