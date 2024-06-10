using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Texture2D _CharacterTexture2D;
    public Sprite _CharacterSprite;

    public List<Dialogue> DialoguesList = new List<Dialogue>();

    private int _nextDialogueindex = 0;

    public Event currentEvent = new Event();

    public Dialogue GetNextDialogue()
    {
        Dialogue dialogue = DialoguesList[_nextDialogueindex];
        _nextDialogueindex += 1;
        return dialogue;
    }

    public bool hasNextDialogue()
    {
        return _nextDialogueindex < DialoguesList.Count;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Serializable]
    public class Dialogue {
        public string text;
        public float duration;
    }

    [Serializable]
    public class Event
    {
        public Ingredient ingredientTarget;
    }
}
