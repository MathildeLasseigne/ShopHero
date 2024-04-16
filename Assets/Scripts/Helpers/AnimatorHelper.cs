using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatorHelper : MonoBehaviour
{
    /*
     * This scripts contains method to set the animator directly to the end of an anim
     * But the issue is that the gameObject containing the animator must be active in hte scene to play an anim
     * So in the script we either : 
     *  - immediately set animator to the end of the chosen anim if gameObject is active in scene
     *  - set animator to the end of the chosen anim next time that the "OnEnable" method is called
    */

    private Animator _animator;

    private string _animToPlayOnEnable = "";

    private string _animRelatedToCallback = "";
    private Action _callbackAnimDone = null;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _callbackAnimDone = null;
        _animRelatedToCallback = "";
        StartCoroutine(PlayEndOfAnimIfMoreThanOneFrame());
    }

    private IEnumerator PlayEndOfAnimIfMoreThanOneFrame()
    {
        yield return new WaitForEndOfFrame();

        if (!gameObject.activeSelf)
            yield break;

        if (!string.IsNullOrEmpty(_animToPlayOnEnable))
        {
            PlayEndOfAnim(_animToPlayOnEnable);
            _animToPlayOnEnable = "";
        }
    }

    public void SetToNewAnimState(string p_animName)
    {
        _callbackAnimDone = null;
        _animRelatedToCallback = "";
        if (gameObject.activeInHierarchy)
        {
            _animToPlayOnEnable = "";
            PlayEndOfAnim(p_animName);
        }
        else
            _animToPlayOnEnable = p_animName;
    }

    public void Play(string p_animName, bool p_setToEndStateIfAnimatorDisabled = false, Action p_callback = null)
    {
        if (gameObject.activeInHierarchy)
        {
            _animRelatedToCallback = p_animName;
            _callbackAnimDone = p_callback;
            _animator.Play(p_animName);
        }
        else if (p_setToEndStateIfAnimatorDisabled)
        {
            _animRelatedToCallback = "";
            _callbackAnimDone = null;
            SetToNewAnimState(p_animName);
        }
    }

    public void OnAnimDone(string p_animName)
    {
        if (_animRelatedToCallback == p_animName)
        {
            _callbackAnimDone?.Invoke();
        }
        _animRelatedToCallback = "";
        _callbackAnimDone = null;
    }

    private void PlayEndOfAnim(string p_animName)
    {
        _animator.speed = 0;
        _animator.Play(p_animName, -1, 1);
        _animator.speed = 1;
    }

    public float GetFloat(string p_animName)
    {
        return _animator.GetFloat(p_animName);
    }

    public void SetFloat(string p_animName, float p_value)
    {
        _animator.SetFloat(p_animName, p_value);
    }

    public void SetSpeed(float p_speed)
    {
        _animator.speed = p_speed;
    }

    public bool GetBool(string p_iName)
    {
        return _animator.GetBool(p_iName);
    }

    public void SetBool(string p_iName, bool p_val)
    {
        _animator.SetBool(p_iName, p_val);
    }

    public void SetTrigger(string p_trigger)
    {
        _animator.SetTrigger(p_trigger);
    }

    public Animator GetAnimator()
    {
        return _animator;
    }
}