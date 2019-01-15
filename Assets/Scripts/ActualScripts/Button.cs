using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    private float _counter = 0;
    public bool ButtonStatus { get; set; }

    private void Update()
    {
        ButtonState();
    }

    //when the button gets pressed
    public void ButtonPressed()
    {
        _counter++;
        ButtonLightSwitch lamp = this.transform.GetComponentInChildren<ButtonLightSwitch>();
        lamp.ChangeTexture();
    }

    //checking the state of the button
    private void ButtonState()
    {
        if (_counter == 1)
        {
            _counter++;
            ButtonStatus = true;
        }
        else if (_counter == 3)
        {
            _counter = 0;
            ButtonStatus = false;
        }
    }

}
