using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Button that display a selected appearance as long as it is still selected
/// </summary>
[RequireComponent(typeof(Button))]
public class CheckableButton : MonoBehaviour
{


    public GroupButton _Group = null;
    [SerializeField] private bool _IsSelected = false;

    private Button _Button;

    [Header("Buttons Colors")]
    [SerializeField] private Color _SelectedColor;
    [SerializeField] private Color _UnselectedColor;

    void Start()
    {
        _Button = gameObject.GetComponent<Button>();
        _Button.onClick.AddListener(ButtonClicked);
        if (_Group)
        {
            _Group.Register(this);
        }
        setColor();
    }

    private void ButtonClicked()
    {
        _IsSelected = true;
        if (_Group)
        {
            _Group.NotifySelection(this);
        }
        setColor();
    }

    /// <summary>
    /// Set the color of the button depending on the current selected state of the button
    /// </summary>
    private void setColor()
    {
        if (_IsSelected)
        {
            _Button.image.color = _SelectedColor;
        }
        else
        {
            _Button.image.color = _UnselectedColor;
        }
    }

    public void Unselect()
    {
        if (_IsSelected)
        {
            _IsSelected = false;
            setColor();
        }

    }






}