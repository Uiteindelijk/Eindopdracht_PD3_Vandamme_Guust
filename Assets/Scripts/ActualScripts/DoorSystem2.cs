using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSystem2 : MonoBehaviour
{
    public int ButtonInteractbles = 0, BoxInteractbles = 0, Doors;
    private Transform[] _buttons, _boxes, _doors;
    private bool[] _button, _box, _door;

    private void Start()
    {
        _buttons = new Transform[ButtonInteractbles];
        _boxes = new Transform[BoxInteractbles];
        _doors = new Transform[Doors];

        _button = new bool[ButtonInteractbles];
        _box = new bool[BoxInteractbles];
        _door = new bool[Doors];

        for (int i = 0; i < ButtonInteractbles; i++)
        {
            _buttons[i] = transform.GetChild(i);
            //Debug.Log(_buttons[i].name);
        }

        for (int i = 0; i < BoxInteractbles; i++)
        {
            _boxes[i] = transform.GetChild(i + ButtonInteractbles);
            //Debug.Log(_boxes[i].name);
        }

        for (int i = 0; i < Doors; i++)
        {
            _doors[i] = transform.GetChild(i + ButtonInteractbles + BoxInteractbles);
            //Debug.Log(_doors[i].name);
        }

    }

    private void Update()
    {
        for (int i = 0; i < ButtonInteractbles; i++)
        {
            _button[i] = _buttons[i].GetComponent<Button>().ButtonStatus;
            //Debug.Log(_buttons[i].name + " " + _button[i]);
        }

        for (int i = 0; i < BoxInteractbles; i++)
        {
            _box[i] = _boxes[i].GetComponent<PlaceBoxOnRightPlace>()._boxOnPlace;
            //Debug.Log(_boxes[i].name + " " + _box[i]);
        }

        for (int i = 0; i < Doors; i++)
        {
            if (_button[0] && _button[1])
            {
                _doors[i].gameObject.active = false;
            }
            else
            {
                _doors[i].gameObject.active = true;
            }
        }

    }

}
