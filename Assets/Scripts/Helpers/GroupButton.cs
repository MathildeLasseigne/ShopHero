using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Group checkableButtons and make them exclusive
/// </summary>
public class GroupButton : MonoBehaviour
{

    private List<CheckableButton> _RegisteredButtons = new List<CheckableButton>();


    /// <summary>
    /// Register the button in the group
    /// </summary>
    /// <param name="button"></param>
    public void Register(CheckableButton button)
    {
        _RegisteredButtons.Add(button);
    }


    /// <summary>
    /// Unselect any other button part of the group
    /// </summary>
    /// <param name="button"></param>
    public void NotifySelection(CheckableButton button)
    {
        foreach (CheckableButton b in _RegisteredButtons)
        {
            if (b != button)
            {
                b.Unselect();
            }
        }
    }


}