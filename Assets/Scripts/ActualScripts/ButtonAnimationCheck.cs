using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAnimationCheck : MonoBehaviour
{
    public bool ButtonPressAnim = false;
    private int _counter;

    //the check if the animation has ended of pressing a button
    public void StartButtonPress()
    {
        ButtonPressAnim = true;
    }

    void EndButtonPress()
    {
        ButtonPressAnim = false;
    }

}
