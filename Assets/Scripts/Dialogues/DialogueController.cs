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

    [SerializeField] CanvasGroup dialogueCanvasGroup;

    private Action DialogueFinishedEvent;


    private Character character;

    private bool hasEntrance, hasExit = false;

    private Coroutine mainCoroutine = null;

    private bool informEnd = true;

    [SerializeField, Min(0)] private float additionnalTime = 0f;
    

    public void Init()
    {
        dialogueText.SetText("");
        mainCoroutine = null;
    }

    public void SetCharacter(Character character)
    {
        this.character = character;
        //characterImage.texture = character._CharacterSprite.textureFromSprite();
    }

    public void SetEntrance(bool hasEntrance = false, bool hasExit = false)
    {
        this.hasEntrance = hasEntrance;
        this.hasExit = hasExit;
        if(hasEntrance) {
            characterImage.color = new Color(characterImage.color.r, characterImage.color.g, characterImage.color.b, 0);
        }
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
        characterImage.texture = character._CharacterSprite;

        if (hasEntrance)
            CharacterEnter();

        while (character.hasNextDialogue())
        {
            Character.Dialogue dialogue = character.GetNextDialogue();
            dialogueText.SetText(dialogue.text);
            yield return new WaitForSeconds(dialogue.duration);
        }

        yield return new WaitForSeconds(additionnalTime);
        if(informEnd)
            DialogueFinishedEvent?.Invoke();

    }

    private void CharacterEnter()
    {
        characterAnimator.SetTrigger("Enter");
    }


    public void SuscribeToDialogueFinishedEvent(Action callback)
    {
        DialogueFinishedEvent += callback;
    }

    public void Hide()
    {
        dialogueCanvasGroup.alpha = 0f;
    }

    public void Show()
    {
        dialogueCanvasGroup.alpha = 1f;
    }

}
