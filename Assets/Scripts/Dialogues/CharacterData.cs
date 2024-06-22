using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterData
{
    public string characterName;
    public string reputationVarName;

    public string characterSpritePath;

    public List<Intervention> interventionList;

}

[Serializable]
public class Intervention
{
    public string uniqueID;

    public DialogueSystemVariableConditionList conditionForDialogue;

    public List<Speech> dialogue;

    public Event eventTarget;

    public bool Equals(Intervention other)
    {
        return this.uniqueID == other.uniqueID;
    }

}


[Serializable]
public class Speech
{
    public string text;
    public float duration;
}

[Serializable]
public class Event
{
    public IngredientValue ingredientTarget;
    public DialogueSystemVariableModificationList consequencesIfFail;
    public DialogueSystemVariableModificationList consequencesIfWin;

}