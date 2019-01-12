using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Yikes : StateMachineBehaviour {

    public bool Test = false;
    private int _counter;

    public void ButtonpreButtonEndEvent()
    {

        _counter++;
        if (_counter == 1)
        {
            Test = true;
            _counter++;
            Debug.Log("oof");
        }
        else
        {
            _counter = 0;
            Debug.Log("XD Not Pressed minion ce tres funny haha XDXDXDD");
        }

    }

}
