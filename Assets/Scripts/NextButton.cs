using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class NextButton : MonoBehaviour
{
    private Button _Button;
    [SerializeField] private Animator _Animator;


    void Start()
    {
        _Button = gameObject.GetComponent<Button>();
        _Button.onClick.AddListener(ButtonClicked);

    }

    private void ButtonClicked()
    {
        Debug.Log("Next Button Clicked");
        MainGameManager.Instance.NextStep();
        if(_Animator)
            _Animator?.SetTrigger("DoAnim");
    }

}
