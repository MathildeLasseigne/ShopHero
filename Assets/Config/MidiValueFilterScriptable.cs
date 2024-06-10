using MidiPlayerTK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/NoteFilter")]
public class MidiValueFilterScriptable : ScriptableObject
{
    [SerializeField] private int MPTKValueToFilter = 0;
    [SerializeField] private string valueName = "";

    public bool isValid(MPTKEvent mptkEvent)
    {
        if (mptkEvent == null) { return false; }
        
        return mptkEvent.Value != MPTKValueToFilter;
    }
}
