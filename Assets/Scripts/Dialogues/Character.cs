using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public Sprite _CharacterSprite;

    public List<Dialogue> DialoguesList = new List<Dialogue>();

    private int _nextDialogueindex = 0;

    public Dialogue GetNextDialogue()
    {
        return DialoguesList[_nextDialogueindex++];
    }

    public bool hasNextDialogue()
    {
        return _nextDialogueindex < DialoguesList.Count -1;
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
}
