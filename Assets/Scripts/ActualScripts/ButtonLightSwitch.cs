using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonLightSwitch : MonoBehaviour
{
    public Material[] Mat;
    private Renderer _rend;
    private int _counter = 0;

    private void Start()
    {
        _rend = GetComponent<Renderer>();
        _rend.sharedMaterial = Mat[0];
    }

    private void Update()
    {
        LightState();
    }

    //to check if the light is on or off
    private void LightState()
    {
        if (_counter == 0)
        {
            _rend.sharedMaterial = Mat[0];
        }
        else
        {
            _rend.sharedMaterial = Mat[1];
        }
        if (_counter > 1)
        {
            _counter = 0;
        }
    }

    //to change the light color
    public void ChangeTexture()
    {
        _counter++;
    }
    
}
