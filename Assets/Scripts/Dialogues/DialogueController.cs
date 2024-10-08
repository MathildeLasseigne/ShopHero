using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Profiling;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class DialogueController : MonoBehaviour
{
    [SerializeField] GameObject dialogueObject;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] Animator characterAnimator;
    [SerializeField] RawImage characterImage;

    private CanvasGroup dialogueCanvasGroup;

    private Action DialogueFinishedEvent;


    private Character character;

    private bool hasEntrance, hasExit = false;

    private Coroutine mainCoroutine = null;

    private bool informEnd = true;



    private void Awake()
    {
        dialogueCanvasGroup = GetComponent<CanvasGroup>();
    }

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
        characterImage.texture = character._CharacterSprite.texture;

        if (hasEntrance)
            CharacterEnter();

        while (character.hasNextDialogueSpeech())
        {
            Speech dialogue = character.GetNextDialogueSpeech();
            dialogueText.SetText(dialogue.text);
            yield return new WaitForSeconds(dialogue.duration);
        }

        if(Data.mainInstance.mainConfig.debug)
            yield return new WaitForSeconds(Data.mainInstance.mainConfig.debugDialogueControllerAdditionnalTime);
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
