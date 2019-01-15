using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAnimationCheck : MonoBehaviour
{
    public bool ButtonPressAnim { get; set; }
    private int _counter;

    private void Start()
    {
        ButtonPressAnim = false;
    }

    //the animation has ended
    private void StartButtonPress()
    {
        ButtonPressAnim = true;
    }

    //the animation is playing
    private void EndButtonPress()
    {
        ButtonPressAnim = false;
    }

}
