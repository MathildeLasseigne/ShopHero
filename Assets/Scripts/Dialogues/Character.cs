using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class Character
{
    [SerializeField, Tooltip("The name to be displayed in the editor")] private string nameForEditor = "";

    public Sprite _CharacterSprite;

    private string currentDialogueID;
    public List<Speech> DialoguesList = new List<Speech>();

    private int _nextSpeechindex = 0;

    [SerializeField] private CharacterData _characterData;

    public Character(CharacterData characterData)
    {
        _characterData = characterData;
        nameForEditor = characterData.characterName;
    }

    public void Load(string characterSpriteFolderPath)
    {
        _CharacterSprite = Resources.Load<Sprite>(Path.Combine(characterSpriteFolderPath, _characterData.characterSpritePath));

    }

    public Speech GetNextDialogueSpeech()
    {
        Speech dialogue = DialoguesList[_nextSpeechindex];
        _nextSpeechindex += 1;
        return dialogue;
    }

    public bool hasNextDialogueSpeech()
    {
        return _nextSpeechindex < DialoguesList.Count;
    }

    public void ArchiveCurrentDialogue()
    {
        //TODO remove current dialogue from dialogue list. To do at the end of a game
    }

    
}
