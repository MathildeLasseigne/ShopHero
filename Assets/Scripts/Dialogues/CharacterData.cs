using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CharacterData //Base class for the character. All data about the character can be found in this class
{
    public string characterName;
    public string reputationVarName; //to find and modify it easily 

    public string characterSpritePath; //Sprite is image. *The name of the image* (can add the path relative to the usual repository if not in this repository, but not usefull yet)

    public List<Intervention> interventionList; //The character enter the shop and ask for a weapon

}

[Serializable]
public class Intervention
{
    public string uniqueID; //Unique id, to make sure a dialogue is not played twice ! And to handle saving the game state

    public DialogueSystemVariableConditionList conditionForDialogue; //Conditions to enter the dialogue. The basis of the dialogue system. Default are false, "", 0

    public List<Speech> dialogue; //The dialogue itself

    public Event eventTarget; //The game related info, and how to modify the dialogue system variables

    public List<QuestionFavorability> questionFavorabilityList; //Questions asked at the end of the game. List can be empty

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

[Serializable]
public class QuestionFavorability
{
    public Speech questionText;
    public int answerListIdx; //Start at 0, to create heart reaction when good answer
    public DialogueSystemVariableConditionList conditionForQuestion;
    public List<AnswerFavorability> answersOptionList;
}

[Serializable]
public class AnswerFavorability
{
    public string answerText;
    public Speech characterReactionText;//What will the character answer to your choice ?. Can be empty (characterReactionText = "")
    public DialogueSystemVariableModificationList consequencesIfChoosed;

}