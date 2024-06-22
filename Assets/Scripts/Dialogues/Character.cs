using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Texture2D _CharacterTexture2D;
    public Sprite _CharacterSprite;

    private string currentDialogueID;
    public List<Speech> DialoguesList = new List<Speech>();

    private int _nextSpeechindex = 0;

    public Event currentEvent = new Event();

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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
