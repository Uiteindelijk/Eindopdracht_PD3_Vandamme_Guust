using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceBoxOnRightPlace : MonoBehaviour
{
    //for more information check document

    private Transform _platform, _box, _lamp;
    private float _enableDistance, _distance, _counter = 0;
    public bool _boxOnPlace { get; set; }

    private void Start()
    {
        _boxOnPlace = false;
        SetChilds();
    }

    private void Update()
    {
        CheckDistance();
    }

    //to set the childs of the object
    private void SetChilds()
    {
        _platform = transform.GetChild(0);
        _box = transform.GetChild(1);
        _lamp = transform.GetChild(2);
    }

    //to look if the box is on the platform or not
    private void CheckDistance()
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

    //to switch the light color
    public void BoxLightSwitch()
    {
        ButtonLightSwitch lamp = this.transform.GetComponentInChildren<ButtonLightSwitch>();
        if (lamp != null)
        {
            lamp.ChangeTexture();
        }
    }

}
