using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceBoxOnRightPlace : MonoBehaviour
{
    private Transform _platform, _box, _lamp;
    private float _enableDistance, _distance, _counter = 0;
    [HideInInspector] public bool _boxOnPlace = false;

    private void Start()
    {
        _platform = transform.GetChild(0);
        _box = transform.GetChild(1);
        _lamp = transform.GetChild(2);
    }

    private void Update()
    {
        _distance = Vector3.Distance(_platform.position, _box.position);
        if (_distance < 1.5 && _counter == 0)
        {
            _counter++;
            BoxLightSwitch();
            _boxOnPlace = true;
            //Debug.Log("Box is on place");
        }
        else if (_distance > 1.5 && _counter == 1)
        {
            _counter--;
            BoxLightSwitch();
            _boxOnPlace = false;
            //Debug.Log("Bos is not on place");
        }



    }

    public void BoxLightSwitch()
    {
        ButtonLightSwitch lamp = this.transform.GetComponentInChildren<ButtonLightSwitch>();
        if (lamp != null)
        {
            lamp.ChangeTexture();
        }
    }

}
