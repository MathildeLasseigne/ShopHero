using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Profiling;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    [SerializeField] GameObject dialogueObject;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] Animator characterAnimator;
    [SerializeField] RawImage characterImage;

    private Action DialogueFinishedEvent;


    private Character character;

    private bool hasEntrance, hasExit = false;

    private Coroutine mainCoroutine = null;

    private bool informEnd = true;
    

    public void Init()
    {
        dialogueText.SetText("");
        mainCoroutine = null;
    }

    public void SetCharacter(Character character)
    {
        this.character = character;
        characterImage.texture = character._CharacterSprite.textureFromSprite();
    }

    public void SetEntrance(bool hasEntrance, bool hasExit)
    {
        this.hasEntrance = hasEntrance;
        this.hasExit = hasExit;
    }

    public void BeginDialogue(bool informEnd = true)
    {
        this.informEnd = informEnd;
        StartCoroutine(MakeDialogue());
    }

    public void StopDialogue()
    {
        if(mainCoroutine != null)
            StopCoroutine(mainCoroutine);
    }

    IEnumerator MakeDialogue()
    {
        if (hasEntrance)
            CharacterEnter();

        while (character.hasNextDialogue())
        {
            Character.Dialogue dialogue = character.GetNextDialogue();
            dialogueText.SetText(dialogue.text);
            yield return new WaitForSeconds(dialogue.duration);
        }

        if(informEnd)
            DialogueFinishedEvent?.Invoke();

    }

    private void CharacterEnter()
    {
    }


    public void SuscribeToDialogueFinishedEvent(Action callback)
    {
        DialogueFinishedEvent += callback;
    }

}
