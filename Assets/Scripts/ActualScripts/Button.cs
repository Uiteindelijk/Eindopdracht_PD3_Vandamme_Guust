using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    private float _counter = 0;
    [HideInInspector] public bool ButtonStatus = false;

    private void Update()
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

    public void ButtonPressed()
    {
        _counter++;
        ButtonLightSwitch lamp = this.transform.GetComponentInChildren<ButtonLightSwitch>();
        lamp.ChangeTexture();
    }
    
}
